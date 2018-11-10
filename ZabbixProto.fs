// #!/usr/bin/dotnet /usr/share/dotnet/sdk/2.1.403/FSharp/fsi.exe
// #!/usr/bin/env fsharpi

(*
   For Zabbix Protocoll in C# see a few libraries on Zabbix Share [1]
   und Nabbix [2] in particluar.

   [1] https://share.zabbix.com/dir-libraries/c
   [2] https://github.com/marksl/nabbix
*)

module ZabbixProto
open System.Net.Sockets
open FSharp.Data
open FSharp.Data.JsonExtensions

let Read (s: NetworkStream) n =
    let buf = Array.zeroCreate n
    s.Read(buf, 0, n) |> ignore
    buf

let Write (s: NetworkStream) buf =
    s.Write(buf, 0, buf.Length)

let FromBytes (b: byte[]) =
    System.Text.Encoding.UTF8.GetString b

let MakeBytes (s: string) =
    System.Text.Encoding.UTF8.GetBytes s

// The first five bytes of most Zabbix messages between the server and
// the agent. And, yes \u0001 character is a single byte in UTF-8:
let ZBX_MAGIC = MakeBytes "ZBXD\u0001"
//  ZBX_MAGIC = [|90uy; 66uy; 88uy; 68uy; 1uy|]

// Unsigned  long ->  little endian  byte  array. That  is how  Zabbix
// encodes the length  of the JSON Text after the  magic string on the
// wire:
let MakeLittleEndian (x: uint64) =
    [|for i in 0 .. 7 -> byte ((x >>> (i * 8)) &&& 0xFFUL)|]
// (MakeLittleEndian 56UL) = [|56uy; 0uy; 0uy; 0uy; 0uy; 0uy; 0uy; 0uy|]

let FromLittleEndian (bytes: byte[]) =
    Array.fold (fun acc b -> (acc <<< 8) + (uint64 b)) 0UL (Array.rev bytes)
// (FromLittleEndian (MakeLittleEndian 1234567890UL)) = 1234567890UL

let MakeRequest json =
    let bytes = MakeBytes json
    // Byte count <> string length!
    let length = uint64 (Array.length bytes)
    let f = Array.append
    f (f ZBX_MAGIC (MakeLittleEndian length)) bytes

// make_request "" = [|90uy; 66uy; 88uy; 68uy; 1uy; 0uy; 0uy; 0uy; 0uy; 0uy; 0uy; 0uy; 0uy|]

let SendReceive host port json =
    use client = new TcpClient(host, port)
    use stream = client.GetStream()
    let body = MakeRequest json
    Write stream body
    let magic = Read stream (4 + 1)     // ZBXD\1
    let length = FromLittleEndian (Read stream 8)
    let text_bytes = Read stream (int length)
    FromBytes text_bytes

let SafeSendReceive host port json =
    try
        SendReceive host port json
    with
        // Server may report errors in similar shape, e.g.:
        // """{"response":"failed","info":"host [host.example.com] not found"}"""
        | _ -> """{"response":"failed","info":"Exception occured"}"""

let SendReceiveJsonValue host port json: JsonValue =
    let inp = json.ToString()
    let out = SafeSendReceive host port inp
    JsonValue.Parse(out)

let Test () =
    // This is how an active Zabbix client requests definitions of metrics
    // the server wants to know.  You will likely get "host ... not found"
    // back:
    // """{"request": "active checks", "host": "host.example.com"}"""
    let request =
        JsonValue.Record [|
            "request",  JsonValue.String "active checks";
            "host",     JsonValue.String "хост.example.com"|]
    printfn "request = %A" request
    let response = SendReceiveJsonValue "localhost" 10051 request
    printfn "%A" response
    match response.TryGetProperty("data") with
    | None -> printfn "No data!"
    | Some items ->
        for i in items do
            printfn "key = %s, delay = %d" (i.["key"].AsString()) (i?delay.AsInteger())

