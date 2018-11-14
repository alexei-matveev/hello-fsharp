// Learn more about F# at http://fsharp.org

open System
// So called local module declaration:
module ZP = ZabbixProto

[<EntryPoint>]
let main argv =
    printfn "Hello World from F#!"
    ZP.Test()
    0 // return an integer exit code
