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

let ping host port request =
    use client = new TcpClient(host, port)
    use stream = client.GetStream()
    write stream (getBytes request)
    let res = read stream 256
    getString res

let response = ping "localhost" 80 "GET / HTTP/1.1\r\n\r\n" in
    printfn "%s" response


