using _8Git;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable disable

namespace _8Git
{
    public class PipeServer
    {

        public delegate void MessageReceivedCallback(string message);

        public static event Action<string> MessageReceived;

        public static string PipeName = null;

        public static void SetPipeName(string name)
        {
            PipeName = name;
        }

        public static bool StartServerAsync()
        {


            if (PipeName == null)
            {
                return true;
            }

            try
            {
           
                Task.Run(async () =>
                {
                    await StartServer();
                });

                return true;
            }
            catch (Exception)
            {

            }

            return false;
        }

        public static async System.Threading.Tasks.Task StartServer()
        {
            if (PipeName == null)
            {
                return;
            }

            
            while (true)
            {

                try
                {
                    using (NamedPipeServerStream pipeServer = new NamedPipeServerStream(
                        PipeName, 
                        PipeDirection.InOut, 
                        NamedPipeServerStream.MaxAllowedServerInstances, 
                        PipeTransmissionMode.Message, 
                        PipeOptions.Asynchronous)
                    ) {
                        await pipeServer.WaitForConnectionAsync();
                        await HandleClientAsync(pipeServer);
                    }
                }
                catch (Exception ex)
                {

                    Program.message(ex.Message);
                }
            }
            
        }

        public static async System.Threading.Tasks.Task HandleClientAsync(NamedPipeServerStream pipeServer)
        {

            if (pipeServer == null)
            {
                return;
            }

            try
            {
                using (var reader = new StreamReader(pipeServer))
                using (var writer = new StreamWriter(pipeServer) { AutoFlush = true })
                {
                    string clientMessage = await reader.ReadLineAsync();
                    MessageReceived?.Invoke(clientMessage);
                    await writer.WriteLineAsync($"Message received");
                }

            }
            catch (Exception ex)
            {

                Program.message(ex.Message);
            }
        }

        public static void SendMessageAsync(string message)
        {
            if (PipeName == null)
            {
                return;

            }

            Task.Run(() =>
            {
                PipeServer.SendMessage(message);

            });
        }

        public static void SendMessage(string message)
        {
            if (PipeName == null)
            {
                return;

            }

            try
            {
                using (var pipeClient = new NamedPipeClientStream(".", PipeName, PipeDirection.InOut, PipeOptions.Asynchronous))
                {
                    pipeClient.Connect();

                    try
                    {
                        using (var reader = new StreamReader(pipeClient))
                        using (var writer = new StreamWriter(pipeClient) { AutoFlush = true })
                        {
                            string messageToSend = message;
                            writer.WriteLine(messageToSend);
                            string response = reader.ReadLine();
                            Program.message($"{response}");
                        }
                    }
                    catch (IOException ex)
                    {
                        Program.message(ex.Message);
                    }
                    finally
                    {
                        pipeClient.Close();
                    }
                }
            }
            catch (Exception ex)
            {

                Program.message(ex.Message);
            }


        }
    }
}
