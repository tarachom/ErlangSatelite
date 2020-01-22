﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ErlangSatelite
{
	class Program
	{
		static void Main(string[] args)
		{
            Thread threadListener = new Thread(new ThreadStart(GeneralWorker));
            threadListener.IsBackground = true;
            threadListener.Start();

            Console.ReadLine();
        }

        static void GeneralWorker()
        {

            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5555);

            Console.WriteLine(localEndPoint.AddressFamily.ToString());

            Socket soketWork = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            Console.WriteLine("[" + DateTime.Now.ToString() + "] --> Listener [0][Connect] - <OK>");

            soketWork.Bind(localEndPoint);
            soketWork.Listen(100);

            Console.WriteLine("[" + DateTime.Now.ToString() + "] --> Listener [0][Listen] - <OK>");

            while (true)
            {
                Console.WriteLine("[" + DateTime.Now.ToString() + "] --> Listener [0][Accept] - <Accept>");

                try
                {
                    //Очікування підключення
                    Socket soketAccept = soketWork.Accept();
                    soketAccept.ReceiveTimeout = 5000;

                    Console.WriteLine("[" + DateTime.Now.ToString() + "] --> Listener [0][Accept][" + soketAccept.RemoteEndPoint.ToString() + "] - <OK>");

                    //буфер для сокета
                    Byte[] buffer = new Byte[1024];

                    int receiveByte = 0;
                    string receiveXmlText = "";

                    //Зчитую дані
                    do
                    {
                        Console.WriteLine(soketAccept.Available);
                        receiveByte = soketAccept.Receive(buffer);
                        Console.WriteLine(soketAccept.Available);
                        receiveXmlText += Encoding.GetEncoding(1251).GetString(buffer, 0, receiveByte);
                    }
                    while (soketAccept.Available > 0);

                    Console.WriteLine("Receive: " + receiveXmlText);

                    string otvet = receiveXmlText + "<br/><h1>OK<h1/>";

                    Console.WriteLine("Send: " + otvet);
                    soketAccept.Send(Encoding.GetEncoding(1251).GetBytes(Convert.ToString(otvet)));

                    Console.WriteLine("Close");
                    soketAccept.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[" + DateTime.Now.ToString() + "] --> Listener [0][Exception - <" + ex.Message + ">");
                }
            }

            //Console.WriteLine("[" + DateTime.Now.ToString() + "] <-- Listener [0] - <Close>");
        }
    }
}
