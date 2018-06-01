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

// The first five bytes of most request and responses on the wire:
let zbx_magic = Array.append (getBytes "ZBXD") [|1uy|]

// zbx_magic = [|90uy; 66uy; 88uy; 68uy; 1uy|]

// Unsigned  long ->  little endian  byte  array. That  is how  Zabbix
// encodes the length  of the JSON Text after the  magic string on the
// wire:
let make_length (x: uint64) =
    [|for i in 0 .. 7 -> byte ((x >>> (i * 8)) &&& 0xFFUL)|]

// (make_length 56UL) = [|56uy; 0uy; 0uy; 0uy; 0uy; 0uy; 0uy; 0uy|]

let make_request json =
    let length_bytes = make_length (uint64 (String.length json))
    printfn "length_bytes = %A" length_bytes
    let json_bytes = getBytes json
    let f = Array.append
    f (f zbx_magic length_bytes) json_bytes

// make_request "" = [|90uy; 66uy; 88uy; 68uy; 1uy; 0uy; 0uy; 0uy; 0uy; 0uy; 0uy; 0uy; 0uy|]

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


