using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ChatAPP
{
	/// <summary>
	/// Lógica interna para Server.xaml
	/// </summary>
	public partial class Server : Window
	{
		#region Campos
		TcpListener serverListener;
		IPAddress iPAddress;
		Thread listeningThread;
		List<Client> clients;
		delegate void UpdateChats(string msg);
		int port;
		#endregion

		#region Propriedades
		public TcpListener ServerListener { get => serverListener; set => serverListener = value; }
		public IPAddress IPAddress { get => iPAddress; set => iPAddress = value; }
		public Thread ListeningThread { get => listeningThread; set => listeningThread = value; }
		public List<Client> Clients { get => clients; set => clients = value; }
		public int Port { get => port; set => port = value; }
		#endregion

		public Server()
		{
			InitializeComponent();
			IPAddress = GetLocalIp();

			TestConnection();

			ServerLog.Items.Add("Inicializando...");
			
			Clients = new List<Client>();

			ListeningThread = new Thread(Listen);
			ListeningThread.Start();

		}

		private void TestConnection()
		{
			ServerListener = new TcpListener(IPAddress, port);
			ServerLog.Items.Add("Tentando acessar a porta " + port);

			
			try
			{
				ServerListener.Start();
				TcpClient tcpClient = new TcpClient();
				tcpClient.Connect(GetLocalIp(), Port);
				ServerListener.Stop();
			}
			catch (Exception)
			{

				Port++;
				TestConnection();
			}


		}

		public static IPAddress GetLocalIp()
		{
			try
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
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				throw;
			}
		}

		private void NewUserBttn_Click(object sender, RoutedEventArgs e)
		{
			if (UserNameInput.Text != "")
			{

				#region Gerenciamento do Novo Controle
				Client newChat = new Client(UserNameInput.Text, Port);
				newChat.Show();
				Clients.Add(newChat);
				#endregion

				ServerLog.Items.Add(new TextBlock() { Text = "Novo usuário Conectado!" });

				UserNameInput.Text = "";  
			}
			else
			{
				MessageBox.Show("O nome do usuário precisa ser preenchido!");
			}
		}

		private void Listen()
		{
			Console.WriteLine();
			while (true)
			{
				serverListener.Start();

				var tcp = ServerListener.AcceptTcpClient();

				NetworkStream nstream = tcp.GetStream();
				byte[] Buffer = new byte[6000];

				var bytes = nstream.Read(Buffer, 0, Buffer.Length);
				string msg = Encoding.UTF8.GetString(Buffer, 0, bytes);

				UpdateChats chatUpdater = new UpdateChats(SendToAll);
				Application
					.Current
					.Dispatcher
					.BeginInvoke(DispatcherPriority.Background, chatUpdater,msg);

				serverListener.Stop();
			}
		}


		void SendToAll(object msg)
		{
			foreach (var chat in Clients)
			{
				TextBlock textBlock = new TextBlock()
				{
					Text = msg.ToString(),
					Foreground = Brushes.White
				};
				chat.ChatListBox.Items.Add(textBlock);
			}
		}


		#region TitleBarMethods
		private void CloseBttn_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void MaximizeBttn_Click(object sender, RoutedEventArgs e)
		{
			if (WindowState == WindowState.Maximized)
			{
				WindowState = WindowState.Normal;
			}
			else
			{
				WindowState = WindowState.Maximized;
			}
		}

		private void MinimizeBttn_Click(object sender, RoutedEventArgs e)
		{
			WindowState = WindowState.Minimized;
		}

		#endregion
	}

}
