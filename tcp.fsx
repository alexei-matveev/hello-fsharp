open System.Net.Sockets

let read (s: NetworkStream) n =
    let buf = Array.zeroCreate n
    s.Read(buf, 0, n) |> ignore
    buf

let write (s: NetworkStream) buf =
    s.Write(buf, 0, buf.Length)

let makeStream host port =
    // Dont s/let/use/ here! Disposing of of client
    // also closes the stream:
    let client = new TcpClient(host, port)
    client.GetStream()

let getString (b: byte[]) = System.Text.Encoding.UTF8.GetString b
let getBytes (s:string) = System.Text.Encoding.UTF8.GetBytes s

let response: byte[] =
  use s = makeStream "google.com" 80
  write s (getBytes "GET / HTTP/1.1\r\n\r\n")
  let res = read s 256
  res

printfn "%s" (getString response)


