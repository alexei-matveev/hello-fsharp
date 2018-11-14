// Learn more about F# at http://fsharp.org

open System
// open ZabbixProto

[<EntryPoint>]
let main argv =
    printfn "Hello World from F#!"
    ZabbixProto.Test()
    0 // return an integer exit code
