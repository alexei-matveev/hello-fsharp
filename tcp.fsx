#!/usr/bin/env fsharpi

(*
   For Zabbix Protocoll in C# see a few libraries on Zabbix Share [1]
   und Nabbix [2] in particluar.

   [1] https://share.zabbix.com/dir-libraries/c
   [2] https://github.com/marksl/nabbix
*)

open System.Net.Sockets
// open FSharp.Data

let read (s: NetworkStream) n =
    let buf = Array.zeroCreate n
    s.Read(buf, 0, n) |> ignore
    buf

let write (s: NetworkStream) buf =
    s.Write(buf, 0, buf.Length)

let getString (b: byte[]) =
    System.Text.Encoding.UTF8.GetString b

let getBytes (s: string) =
    System.Text.Encoding.UTF8.GetBytes s

let zbx_magic = Array.append (getBytes "ZBXD") [|1uy|]
// printfn "%A" zbx_magic

// FIXME: very imperative.
let make_length (x: uint64) =
    let mutable y = x
    let (buf: byte[]) = Array.zeroCreate 8
    for i = 0 to 7 do
        let b = byte (y &&& 0xFFUL)
        printfn "byte = %A" b
        buf.[i] <- b
        y <- (y >>> 8)
    buf

printfn "%A" (make_length (uint64 256))

let make_request json =
    let length_bytes = make_length (uint64 (String.length json))
    printfn "length_bytes = %A" length_bytes
    let json_bytes = getBytes json
    let f = Array.append
    f (f zbx_magic length_bytes) json_bytes

let ping host port json =
    use client = new TcpClient(host, port)
    use stream = client.GetStream()
    let body = make_request json
    printfn "body = %A" body
    write stream body
    let res = read stream (256 + 195)
    getString res

// This ist  how an active Zabbix  client asks for the  definitions of
// mertrics and intervals the server  wants to know. The response will
// likely be  {"response":"failed","info":"host [host.example.com] not
// found"}
let json =  """{"request": "active checks", "host": "host.example.com"}"""
let response = ping "localhost" 10051 json
printfn "%s" response


