namespace FSharp.NetCore


module Grains =

    open System.Threading.Tasks
    open FSharp.NetCore.Interfaces

    open Orleans
    open Orleans.Providers

     open FSharp.Control.Tasks


    [<StorageProvider(ProviderName="OrleansStorage")>]
    type HelloGrain() =
        inherit Grain<SampleHolder> ()
        interface IHello with
            member this.GetSample(arg1: System.Guid): Task<Sample option> = 
                Task.FromResult this.State.Sample 
            member this.SetSample(sample: Sample): Task = 
                this.State.Sample <- Some sample
                printf "scheduling wite"
                let write =  this.WriteStateAsync()
                printf "write scheduled"
                task {
                     printf "sleeping"
                     do! Task.Delay 1000
                     printf "slept"
                     do! write
                     printf "wrote"
                } |> ignore
                Task.CompletedTask
            member this.SayHello (greeting : string) : Task<Sample option> = 
                Task.FromResult this.State.Sample 

