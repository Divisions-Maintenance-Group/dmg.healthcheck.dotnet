namespace Dmg.HealthChecks

module HealthChecks =
    open HealthState

    let SetReady ready = HealthState.SetReady ready

    let GetReadyAsync = HealthState.GetReadyAsync

    let SetAlive alive = HealthState.SetAlive alive

    let GetAliveAsync = HealthState.GetAliveAsync
