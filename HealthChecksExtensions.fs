namespace Dmg.HealthChecks

open System.Runtime.CompilerServices

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Diagnostics.HealthChecks
open Microsoft.Extensions.Diagnostics.HealthChecks
open Microsoft.Extensions.DependencyInjection

module HealthCheckNames =
    [<Literal>]
    let ALIVE_CHECK = "Alive_Check"

    [<Literal>]
    let READY_CHECK = "Ready_Check"

type ReadyCheck() =
    interface IHealthCheck with
        member _.CheckHealthAsync(ctx, cancellationToken) =
            async {
                let! ready = HealthChecks.GetReadyAsync ()

                return
                    if ready then
                        HealthCheckResult.Healthy("Service is Ready")
                    else
                        HealthCheckResult.Unhealthy("Service is not Ready")
            }
            |> Async.StartAsTask


type HealthCheck() =
    interface IHealthCheck with
        member _.CheckHealthAsync(ctx, cancellationToken) =
            async {
                let! alive = HealthChecks.GetAliveAsync ()

                return
                    if alive then
                        HealthCheckResult.Healthy("Service is Alive")
                    else
                        HealthCheckResult.Unhealthy("Service is Not Alive")
            }
            |> Async.StartAsTask


[<Extension>]
type IEnumerableExtensions =

    [<Extension>]
    static member inline UseHealthAndReadyChecks(app: IApplicationBuilder) =
        app
            .UseRouting()
            .UseEndpoints(fun endpoints ->
                endpoints.MapHealthChecks(
                    "/healthcheck/liveness",
                    new HealthCheckOptions(Predicate = (fun (check) -> check.Name = HealthCheckNames.ALIVE_CHECK))
                )
                |> ignore

                endpoints.MapHealthChecks(
                    "/healthcheck/readiness",
                    new HealthCheckOptions(Predicate = (fun (check) -> check.Name = HealthCheckNames.READY_CHECK))
                )
                |> ignore

                )

    [<Extension>]
    static member inline AddHealthAndReadyChecks(services: IServiceCollection) =
        services
            .AddHealthChecks()
            .AddCheck<HealthCheck>(HealthCheckNames.ALIVE_CHECK)
            .AddCheck<ReadyCheck>(HealthCheckNames.READY_CHECK)
        |> ignore

        services
