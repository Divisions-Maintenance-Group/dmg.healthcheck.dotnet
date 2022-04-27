namespace Dmg.HealthChecks

module HealthChecks =
    open HealthState

    let SetReady ready = HealthState.SetReady ready

    let GetReadyAsync = HealthState.GetReadyAsync

    let SetHealthy healthy = HealthState.SetHealthy healthy

    let GetHealthyAsync = HealthState.GetHealthyAsync
