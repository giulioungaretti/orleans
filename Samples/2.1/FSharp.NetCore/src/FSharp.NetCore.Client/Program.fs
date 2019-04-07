module FSharp.NetCore

open System
open System.Threading.Tasks
open Microsoft.Extensions.Logging
open Orleans
open Orleans.Runtime.Configuration
open Orleans.Hosting
open FSharp.Control.Tasks

open FSharp.NetCore.Interfaces

let buildClient () =
      let builder = new ClientBuilder()
      builder
        .UseLocalhostClustering()
        .ConfigureApplicationParts(fun parts -> parts.AddApplicationPart((typeof<IHello>).Assembly).WithCodeGeneration() |> ignore )
        .ConfigureLogging(fun logging -> logging.AddConsole() |> ignore)
        .Build()

let worker (client : IClusterClient) =
    let g = System.Guid.NewGuid ()
    task {
        let friend = client.GetGrain<IHello> g
        do printfn "fresh"
        let! response = friend.SayHello ("Good morning, my friend!")
        match response with
        | Some {Value = a } ->
            printfn "%s" a 
        | None ->
            printfn "%s" "none" 
        let s = {Sample.Value = "test"}
        do! friend.SetSample s
        do printfn "fresh"
        let! response = friend.SayHello ("Good morning, my friend!")
        match response with
        | Some {Value = a } ->
            printfn "%s" a 
        | None ->
            printfn "%s" "none" 
    }

[<EntryPoint>]
let main _ =
    let t = task {
        use client = buildClient()
        do! client.Connect( fun (ex: Exception) -> task {
            do! Task.Delay(1000)
            return true
        })
        printfn "Client successfully connect to silo host"
        do! worker client
    }

    t.Wait()

    Console.ReadKey() |> ignore

    0
