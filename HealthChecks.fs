namespace Dmg.HealthChecks

module HealthChecks =
    open HealthState

    let setReady ready = HealthState.setReady ready

    let getReadyAsync = HealthState.getReadyAsync

    let setHealthy healthy = HealthState.setHealthy healthy

    let getHealthyAsync = HealthState.getHealthyAsync
