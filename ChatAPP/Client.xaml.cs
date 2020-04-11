using System.Windows;
using System.Net;
using System.Net.Sockets;
using System;
using System.Text;
using System.Threading;
using System.Windows.Input;

namespace ChatAPP
{
	/// <summary>
	/// Interação lógica para MainWindow.xam
	/// </summary>
	public partial class Client : Window
	{

		#region Campos
		int port;
		TcpClient client;
		int byteCount;
		byte[] sendData;
		static TcpListener tcpServer;
		#endregion

		#region Propriedades
		public TcpClient TcpClient { get => client; set => client = value; }
		public int ByteCount { get => byteCount; set => byteCount = value; }
		public byte[] SendData { get => sendData; set => sendData = value; }
		public int Port { get => port; set => port = value; }
		#endregion

		public Client(string name, int port)
		{
			InitializeComponent();
			UserNameLbl.Text = name;
			Port = port; 
		}
		

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			SendMsgBttn.IsEnabled = true;
			userInputTxtBox.IsEnabled = true;
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
			catch (Exception)
			{
				MessageBox.Show("Não Foi Possível Adquirir IP Local");
				return null;

			}
		}


		private void SendMsgBttn_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				TcpClient = new TcpClient();
				TcpClient.Connect(GetLocalIp(), Port);

				if (TcpClient.Connected)
				{
					string msg = UserNameLbl.Text + ": " + userInputTxtBox.Text;
					ByteCount = Encoding.UTF8.GetByteCount(msg);
					SendData = new byte[ByteCount];
					SendData = Encoding.UTF8.GetBytes(msg);

					NetworkStream networkStream = TcpClient.GetStream();
					networkStream.Write(SendData, 0, SendData.Length);

					networkStream.Close();
					TcpClient.Close();
				}
				else
				{
					MessageBox.Show("Não foi possível conectar o Usuário");
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
				this.DragMove();
		}

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
	}
}
