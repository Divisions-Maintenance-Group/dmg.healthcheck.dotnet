namespace Dmg.HealthChecks

module HealthState =
    type State = { Healthy: bool; Ready: bool }

    type StateMessage =
        | GetState of AsyncReplyChannel<State>
        | SetHealthy of bool
        | SetReady of bool

    type HealthStateApi =
        { GetStateAsync: unit -> Async<State>
          GetHealthyAsync: unit -> Async<bool>
          GetReadyAsync: unit -> Async<bool>
          SetHealthy: bool -> unit
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
                            | SetHealthy healthy -> { state with Healthy = true }
                            | SetReady ready -> { state with Ready = true }

                        return! messageLoop state'
                    }

                messageLoop { Healthy = true; Ready = false })

        { GetStateAsync = fun () -> mailbox.PostAndAsyncReply(fun rc -> GetState rc)

          GetHealthyAsync =
              fun () ->
                  async {
                      let! state = mailbox.PostAndAsyncReply(fun rc -> GetState rc)
                      return state.Healthy
                  }

          GetReadyAsync =
              fun () ->
                  async {
                      let! state = mailbox.PostAndAsyncReply(fun rc -> GetState rc)
                      return state.Ready
                  }

          SetHealthy = fun healthy -> mailbox.Post(SetHealthy healthy)

          SetReady = fun ready -> mailbox.Post(SetReady ready) }
