using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class MyTcpClient
{
    private static bool keepRunning = true;

    public static void Main()
    {
        Connect("127.0.0.1");
    }

    static void Connect(String server)
    {
        try
        {
            Int32 port = 13000;
            using TcpClient client = new TcpClient(server, port);
            NetworkStream stream = client.GetStream();

            // 수신을 별도의 스레드에서 처리
            Thread receiveThread = new Thread(() => ReceiveData(stream));
            receiveThread.Start();

            // 사용자 입력을 처리하는 부분
            while (keepRunning)
            {
                Console.Write("Enter a message to send (or type 'exit' to quit): ");
                string message = Console.ReadLine();  // 사용자로부터 입력받기

                // 'exit'를 입력하면 루프 종료
                if (message.ToLower() == "exit")
                {
                    keepRunning = false;
                    break;
                }

                // 메시지를 ASCII로 인코딩하여 바이트 배열로 변환
                Byte[] data = Encoding.ASCII.GetBytes(message);

                // 메시지를 서버로 전송
                stream.Write(data, 0, data.Length);
            }

            // 스레드가 끝날 때까지 대기
            receiveThread.Join();
        }
        catch (ArgumentNullException e)
        {
            Console.WriteLine("ArgumentNullException: {0}", e);
        }
        catch (SocketException e)
        {
            Console.WriteLine("SocketException: {0}", e);
        }

        Console.WriteLine("\n Press Enter to continue...");
        Console.Read();
    }

    // 서버로부터 데이터를 받는 메서드
    static void ReceiveData(NetworkStream stream)
    {
        try
        {
            Byte[] data = new Byte[256];
            String responseData = String.Empty;

            while (keepRunning)
            {
                // 서버로부터 데이터 수신
                Int32 bytes = stream.Read(data, 0, data.Length);
                if (bytes > 0)
                {
                    responseData = Encoding.ASCII.GetString(data, 0, bytes);
                    Console.WriteLine("\nReceived: {0}", responseData);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Connection lost: {0}", e.Message);
        }
    }
}
