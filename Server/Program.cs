using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server
{
	class Program
	{
		static Thread thread;
		static TcpListener server;
		public static IPAddress GetLocalIp()
		{
			IPHostEntry host;
			host = Dns.GetHostEntry(Dns.GetHostName());
			foreach (IPAddress iPAddress in host.AddressList)
			{
				if (iPAddress.AddressFamily == AddressFamily.InterNetwork)
				{
					return iPAddress;
				}
			}
			return Dns.GetHostEntry("localhost").AddressList[0];
		}

		static void Main(string[] args)
		{
			//Loal Ip
			IPAddress ip = GetLocalIp();
			//Server TCP listener
			server = new TcpListener(ip, 8080);
			thread = new Thread(ConnectionListener);
			

			TcpClient client = default(TcpClient);

			try
			{
				server.Start();
				Console.WriteLine("Server Started"); thread.Start();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Console.ReadLine();
			}
			finally { Console.WriteLine("Sucesso!"); }

			while (true)
			{
				client = server.AcceptTcpClient();
				byte[] receivedBuffer = new byte[1500];
				NetworkStream stream = client.GetStream();

				stream.Read(receivedBuffer, 0, receivedBuffer.Length);
				string msg = Encoding.ASCII.GetString(receivedBuffer, 0, receivedBuffer.Length);
				Console.WriteLine(msg); 
			}
		}

		private static void ConnectionListener(object obj)
		{
			Console.WriteLine("Verificando conexões pendentes...");
			while (true)
			{
				Thread.Sleep(500);
				if (server.Pending())
				{
					Console.WriteLine("Há conexões Pendentes");
				}
				Console.WriteLine("Não há Conexões Pendentes"); 
			}
		}
	}
}


