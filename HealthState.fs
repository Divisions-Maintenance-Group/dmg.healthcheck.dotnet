namespace Dmg.HealthChecks

module HealthState =
    type State = { Alive: bool; Ready: bool }

    type StateMessage =
        | GetState of AsyncReplyChannel<State>
        | SetAlive of bool
        | SetReady of bool

    type HealthStateApi =
        { GetStateAsync: unit -> Async<State>
          GetAliveAsync: unit -> Async<bool>
          GetReadyAsync: unit -> Async<bool>
          SetAlive: bool -> unit
          SetReady: bool -> unit }

    let HealthState =
        let mailbox =
            MailboxProcessor.Start (fun mailbox ->
                let rec messageLoop (state: State) =
                    async {
                        let! message = mailbox.Receive()

                        let state' =
                            match message with
                            | GetState rc ->
                                rc.Reply state
                                state
                            | SetAlive alive -> { state with Alive = true }
                            | SetReady ready -> { state with Ready = true }

                        return! messageLoop state'
                    }

                messageLoop { Alive = true; Ready = false })

        { GetStateAsync = fun () -> mailbox.PostAndAsyncReply(fun rc -> GetState rc)

          GetAliveAsync =
              fun () ->
                  async {
                      let! state = mailbox.PostAndAsyncReply(fun rc -> GetState rc)
                      return state.Alive
                  }

          GetReadyAsync =
              fun () ->
                  async {
                      let! state = mailbox.PostAndAsyncReply(fun rc -> GetState rc)
                      return state.Ready
                  }

          SetAlive = fun alive -> mailbox.Post(SetAlive alive)

          SetReady = fun ready -> mailbox.Post(SetReady ready) }
