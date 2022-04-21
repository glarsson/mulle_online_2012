using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Mulle.Lib.Entities;
using Mulle.Lib.Repositories;
/*
namespace Mulle.Lib.Net
{
    public class Net
    {
        public struct Message
        {
            public int transmission_control { get; set; }
            public string action { get; set; }
            public string message { get; set; }
        }
        public class Server
        {
            private TcpListener tcpListener;
            private TCPMessageRepository tcpMessageRepository;
            private int threadid = 0;

            public Server(TCPMessageRepository tcpMessageRepository)
            {
                this.tcpListener = new TcpListener(IPAddress.Any, 3000);
                this.tcpMessageRepository = tcpMessageRepository;
                Console.WriteLine("Server Main Thread: issuing ListenForClients work item");
                ThreadPool.QueueUserWorkItem(new WaitCallback(ListenForClients), threadid);
            }


            private void ListenForClients(object blueberrypancakes)
            {
                int threadid = (int)blueberrypancakes;
                Console.WriteLine("ListenForClients: thread: " + threadid + " started");
                threadid++;

                this.tcpListener.Start();
                while (true)
                {
                    //blocks until a client has connected to the server
                    TcpClient client = this.tcpListener.AcceptTcpClient();
                    //create a thread to handle communication 
                    //with connected client
                    Console.WriteLine("ListenForClients: issuing HandleCommunications work item");
                    ThreadPool.QueueUserWorkItem(o => HandleCommunications(client));
                }
            }

            public void HandleCommunications(object client)
            {
                Console.WriteLine("HandleCommunications: thread started");
                TcpClient tcpClient = (TcpClient)client;
                Console.WriteLine("HandleCommunications: processing client:" + tcpClient.Client.RemoteEndPoint.ToString());
                NetworkStream clientStream = tcpClient.GetStream();

                byte[] message = new byte[4096];
                int bytesRead;
                while (true)
                {
                    this.tcpMessageRepository = new TCPMessageRepository();
                    bytesRead = 0;
                    try
                    {
                        //blocks until a client sends a message
                        bytesRead = clientStream.Read(message, 0, 4096);
                    }
                    catch
                    {
                        //a socket error has occured
                        Console.WriteLine("HandleCommunications: socket error.");
                        break;
                    }
                    if (bytesRead == 0)
                    {
                        //the client has disconnected from the server
                        Console.WriteLine("HandleCommunications: client disconnected");
                        break;
                    }

                    ASCIIEncoding encoder = new ASCIIEncoding();
                    Console.WriteLine("HandleCommunications: " + encoder.GetString(message, 0, bytesRead) + "\n");

                    // Split message upp in struct and pass out
                    StringBuilder split_all = new StringBuilder();
                    StringBuilder split_ctrl = new StringBuilder();
                    StringBuilder split_action = new StringBuilder();
                    StringBuilder split_message = new StringBuilder();

                    split_all.Append(encoder.GetString(message, 0, bytesRead));


                    for (int i = split_all.Length - 6; i < split_all.Length; i++)
                    {
                        char ch = split_all[i];
                        split_ctrl.Append(ch);
                    }
                    for (int i = 0; i < split_all.Length - 6; i++)
                    {
                        char ch = split_all[i];
                        if (ch != '|')
                        {
                            split_action.Append(ch);
                        }
                        else
                        {
                            while (true)
                            {
                                char ch2 = split_all[i + 1];
                                if (ch2 == '|')
                                    goto outside_loop;
                                split_message.Append(ch2);
                                i++;
                            }
                        }
                    }
                outside_loop:

                    int ctrl_number = -1;
                    try
                    {
                        ctrl_number = Convert.ToInt32(split_ctrl.ToString());
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("HandleCommunications: Convert.ToInt32 error: input NaN");
                    }
                    catch (InvalidCastException)
                    {
                        Console.WriteLine("HandleCommunications: Convert.ToInt32 error: invalid cast exception");
                    }
                    catch (OverflowException)
                    {
                        Console.WriteLine("HandleCommunications: Convert.ToInt32 error: the number is too large");
                    }

                    // Write TCP Message to database! - Funkar inte :( Id ar null och tas inte emot, hajar inte varfor Id blir null...
                    //var tcpmessageRepository = new TCPMessageRepository();
                    //TcpClient tcpClient = (TcpClient)client;
                    //TCPMessageRepository tcpmessagerepository = (TCPMessageRepository)tcpmessagerepository;
                    using (this.tcpMessageRepository)
                    {
                        this.tcpMessageRepository.Add(new TCPMessage { TransmissionControl = ctrl_number, Action = split_action.ToString(), Message = split_message.ToString() });
                        this.tcpMessageRepository.Save();
                    }
                    Console.WriteLine("HandleCommunications: updated tcpmessagerepository");
                    //tcpmessageRepository.Save();


                }
                tcpClient.Close();
            }
        }

        public static bool SendToServer(string action, string message, int ctrl)
        {
            //Random random = new Random();
            //int transmission_control = random.Next(100000, 999999);
            ASCIIEncoding encoder = new ASCIIEncoding();
            StringBuilder action_endresult = new StringBuilder();
            StringBuilder message_endresult = new StringBuilder();
            action_endresult.Append(action);
            message_endresult.Append(message);

            // Add control code "|" and transmission control id
            action_endresult.Append("|");
            message_endresult.Append("|");
            message_endresult.Append(ctrl);
         


            // Convert working data to bytes for sending
            byte[] action_bytes = encoder.GetBytes(action_endresult.ToString());
            byte[] message_bytes = encoder.GetBytes(message_endresult.ToString());

            // Initialize TCP subsystem and send that bitch
            TcpClient client = new TcpClient();
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3000);
            try
            {
                using (client)
                {
                    client.Connect(serverEndPoint);
                    if (client.Connected == true)
                    {
                        NetworkStream clientStream = client.GetStream();
                        clientStream.Write(action_bytes, 0, action_bytes.Length);
                        clientStream.Write(message_bytes, 0, message_bytes.Length);
                        clientStream.Flush();
                        
                    }
                }
                return true;
            }
            catch (System.Exception ex)
            {

                Console.WriteLine(ex);
                return false;
            }
            //finally { client.Close(); }
        }        
    
    }
}
*/