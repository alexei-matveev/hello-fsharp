#!/usr/bin/env fsharpi

(*
   For Zabbix Protocoll in C# see a few libraries on Zabbix Share [1]
   und Nabbix [2] in particluar.

   [1] https://share.zabbix.com/dir-libraries/c
   [2] https://github.com/marksl/nabbix
*)

open System.Net.Sockets

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

let ping host port request =
    use client = new TcpClient(host, port)
    use stream = client.GetStream()
    write stream (getBytes request)
    let res = read stream 256
    getString res

let response = ping "localhost" 80 "GET / HTTP/1.1\r\n\r\n" in
    printfn "%s" response


