using System.Net;
using System.Net.Sockets;

class MyTcpClient()
{
    public static void Main()
    {
        Connect("127.0.0.1");
    }

    static void Connect(String server)
    {
        try
        {
            // Create a TcpClient.
            // Note, for this client to work you need to have a TcpServer
            // connected to the same address as specified by the server, port
            // combination.
            Int32 port = 13000;

            // Prefer a using declaration to ensure the instance is Disposed later.
            using TcpClient client = new TcpClient(server, port);

            // Translate the passed message into ASCII and store it as a Byte array.

            // Get a client stream for reading and writing.
            NetworkStream stream = client.GetStream();

            Byte[] data = new Byte[256];

            String responseData = String.Empty;

            Int32 bytes = stream.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
            Console.WriteLine("Received: {0}", responseData);


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
}
