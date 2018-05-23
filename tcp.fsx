open System.Net
open System.Text 
open System.Net.Sockets

type stream = NetworkStream
let curry g b n = g(b,0,n) |> ignore; b
let read  n (s : stream) = curry s.Read (Array.zeroCreate n) n,s
let write b (s : stream) = curry s.Write b b.Length; s

let connect host port = TcpClient(host, port).GetStream()

let response: byte[] =
  let s = connect "google.com" 80
  write "GET / HTTP/1.1\r\n\r\n"B s
  let res, _ = read 256 s
  s.Close()
  res

printfn "%s" (System.Text.Encoding.ASCII.GetString response)


