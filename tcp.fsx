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

// The first five bytes of most Zabbix messages between the server and
// the agent:
let zbx_magic = Array.append (getBytes "ZBXD") [|1uy|]

// zbx_magic = [|90uy; 66uy; 88uy; 68uy; 1uy|]

// Unsigned  long ->  little endian  byte  array. That  is how  Zabbix
// encodes the length  of the JSON Text after the  magic string on the
// wire:
let make_bytes (x: uint64) =
    [|for i in 0 .. 7 -> byte ((x >>> (i * 8)) &&& 0xFFUL)|]

// (make_bytes 56UL) = [|56uy; 0uy; 0uy; 0uy; 0uy; 0uy; 0uy; 0uy|]

let make_uint64 (bs: byte[]) =
    Array.sum [|for i in 0 .. 7 -> ((uint64 bs.[i]) <<< (i * 8))|]

// (make_uint64 (make_bytes 1234567890UL)) = 1234567890UL

let make_request json =
    let text_length_le = make_bytes (uint64 (String.length json))
    let json_bytes = getBytes json
    let f = Array.append
    f (f zbx_magic text_length_le) json_bytes

// make_request "" = [|90uy; 66uy; 88uy; 68uy; 1uy; 0uy; 0uy; 0uy; 0uy; 0uy; 0uy; 0uy; 0uy|]

let ping host port json =
    use client = new TcpClient(host, port)
    use stream = client.GetStream()
    let body = make_request json
    write stream body
    let magic = read stream (4 + 1)     // ZBXD\1
    let text_length_le = read stream 8  // little endian
    let length = make_uint64 text_length_le
    let text_bytes = read stream (int length)
    getString text_bytes

// This is how an active Zabbix client requests definitions of metrics
// the server wants to know.  You will likely get "host ... not found"
// back:
let json =  """{"request": "active checks", "host": "host.example.com"}"""
let response = ping "localhost" 10051 json
// response = """{"response":"failed","info":"host [host.example.com] not found"}"""

printfn "%s" response


