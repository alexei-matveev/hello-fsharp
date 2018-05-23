open System.Net.Sockets

let read (s: NetworkStream) n =
    let buf = Array.zeroCreate n
    s.Read(buf, 0, n) |> ignore
    buf

let write (s: NetworkStream) buf =
    s.Write(buf, 0, buf.Length)

let getString (b: byte[]) = System.Text.Encoding.UTF8.GetString b
let getBytes (s:string) = System.Text.Encoding.UTF8.GetBytes s

let response: byte[] =
  use client = new TcpClient("google.com", 80)
  use s = client.GetStream()
  write s (getBytes "GET / HTTP/1.1\r\n\r\n")
  let res = read s 256
  res

printfn "%s" (getString response)


