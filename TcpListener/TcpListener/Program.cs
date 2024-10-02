using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;

class MyTcpListener
{
    // 연결된 클라이언트들을 추적하기 위한 리스트
    private static List<TcpClient> connectedClients = new List<TcpClient>();

    public static void Main()
    {
        TcpListener server = null;
        try
        {
            // TcpListener를 특정 포트에 바인딩
            Int32 port = 13000;
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");
            server = new TcpListener(IPAddress.Any, port);

            // 클라이언트 요청 수신 시작
            server.Start();
            Console.WriteLine("Server started... Waiting for clients.");

            // 클라이언트를 수신하는 무한 루프
            while (true)
            {
                Console.Write("Waiting for a connection... ");

                // 클라이언트 연결 수락 (블로킹 호출)
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Connected!");

                // 연결된 클라이언트 목록에 추가
                lock (connectedClients)
                {
                    connectedClients.Add(client);
                }

                // 클라이언트마다 새로운 작업 스레드에서 처리
                System.Threading.Thread clientThread = new System.Threading.Thread(() => HandleClient(client));
                clientThread.Start();
            }
        }
        catch (SocketException e)
        {
            Console.WriteLine("SocketException: {0}", e);
        }
        finally
        {
            if (server != null)
                server.Stop();
        }
    }

    // 각 클라이언트의 통신을 처리하는 메서드
    private static void HandleClient(TcpClient client)
    {
        try
        {
            NetworkStream stream = client.GetStream();
            Byte[] bytes = new Byte[256];
            String data = null;

            int i;
            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                // 클라이언트로부터 받은 메시지를 문자열로 변환
                data = Encoding.ASCII.GetString(bytes, 0, i);
                Console.WriteLine("Received: {0}", data);

                // 받은 데이터를 다른 클라이언트에게 중계
                BroadcastMessage(data, client);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("UnConnected");
            //Console.WriteLine("Exception: {0}", e);
        }
        finally
        {
            // 클라이언트 연결이 종료되면 리스트에서 제거
            lock (connectedClients)
            {
                connectedClients.Remove(client);
            }
            client.Close();
        }
    }

    // 다른 클라이언트에게 메시지를 전송하는 메서드
    private static void BroadcastMessage(string message, TcpClient senderClient)
    {
        byte[] msg = Encoding.ASCII.GetBytes(message.ToUpper());

        lock (connectedClients)
        {
            foreach (var client in connectedClients)
            {
                // 메시지를 보낸 클라이언트를 제외하고 전송
                if (client != senderClient)
                {
                    NetworkStream stream = client.GetStream();
                    stream.Write(msg, 0, msg.Length);
                }
            }
        }
        Console.WriteLine("Broadcasted message: {0}", message.ToUpper());
    }
}
