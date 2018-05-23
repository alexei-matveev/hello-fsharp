open System.Net.Sockets

let read n (s: NetworkStream)  =
    let buf = Array.zeroCreate n
    s.Read(buf, 0, n) |> ignore
    buf

let write buf (s: NetworkStream) =
    s.Write(buf, 0, buf.Length)

let makeStream host port =
    let client = new TcpClient(host, port)
    client.GetStream()

let response: byte[] =
  let s = makeStream "google.com" 80
  write "GET / HTTP/1.1\r\n\r\n"B s
  let res = read 256 s
  s.Close()
  res

printfn "%s" (System.Text.Encoding.ASCII.GetString response)


