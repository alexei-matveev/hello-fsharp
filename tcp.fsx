open System.Net.Sockets

let read (s: NetworkStream) n =
    let buf = Array.zeroCreate n
    s.Read(buf, 0, n) |> ignore
    buf

let write (s: NetworkStream) buf =
    s.Write(buf, 0, buf.Length)

let makeStream host port =
    let client = new TcpClient(host, port)
    client.GetStream()

let response: byte[] =
  let s = makeStream "google.com" 80
  write s "GET / HTTP/1.1\r\n\r\n"B
  let res = read s 256
  s.Close()
  res

printfn "%s" (System.Text.Encoding.ASCII.GetString response)


