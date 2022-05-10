namespace Dmg.HealthChecks

module HealthCheck =
    open HealthState

    let SetReady ready = HealthState.SetReady ready

    let GetReady = HealthState.GetReady
    let GetReadyAsync = HealthState.GetReadyAsync

    let SetAlive alive = HealthState.SetAlive alive

    let GetAlive = HealthState.GetAlive
    let GetAliveAsync = HealthState.GetAliveAsync
