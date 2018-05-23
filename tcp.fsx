open System.Net
open System.Text 
open System.Net.Sockets

type stream = NetworkStream

let read n (s: stream) =
    let a = Array.zeroCreate n
    s.Read(a, 0, n) |> ignore
    a

let write b (s : stream) =
    s.Write(b, 0, b.Length)

let connect host port = TcpClient(host, port).GetStream()

let response: byte[] =
  let s = connect "google.com" 80
  write "GET / HTTP/1.1\r\n\r\n"B s
  let res = read 256 s
  s.Close()
  res

printfn "%s" (System.Text.Encoding.ASCII.GetString response)


