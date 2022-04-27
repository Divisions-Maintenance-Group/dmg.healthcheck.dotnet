namespace Dmg.HealthChecks

module HealthState =
    type State = { healthy: bool; ready: bool }

    type StateMessage =
        | GetState of AsyncReplyChannel<State>
        | SetHealthy of bool
        | SetReady of bool

    type HealthStateApi =
        { getStateAsync: unit -> Async<State>
          getHealthyAsync: unit -> Async<bool>
          getReadyAsync: unit -> Async<bool>
          setHealthy: bool -> unit
          setReady: bool -> unit }

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
                            | SetHealthy healthy -> { state with healthy = true }
                            | SetReady ready -> { state with ready = true }

                        return! messageLoop state'
                    }

                messageLoop { healthy = true; ready = false })

        { getStateAsync = fun () -> mailbox.PostAndAsyncReply(fun rc -> GetState rc)

          getHealthyAsync =
              fun () ->
                  async {
                      let! state = mailbox.PostAndAsyncReply(fun rc -> GetState rc)
                      return state.healthy
                  }

          getReadyAsync =
              fun () ->
                  async {
                      let! state = mailbox.PostAndAsyncReply(fun rc -> GetState rc)
                      return state.ready
                  }

          setHealthy = fun healthy -> mailbox.Post(SetHealthy healthy)

          setReady = fun ready -> mailbox.Post(SetReady ready) }
