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
                /*var buffer = new byte[1024];
                int bytesRead;

                // Continuously read from the pipe until the client disconnects
                while ((bytesRead = await pipeServer.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    MessageReceived?.Invoke(message);
                }*/

                /*using (var reader = new StreamReader(pipeServer))
                using (var writer = new StreamWriter(pipeServer) { AutoFlush = true })
                {
                    string clientMessage = await reader.ReadLineAsync();
                    await writer.WriteLineAsync($"Message received: {clientMessage}");
                    
                }*/

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

            Task.Run(async () =>
            {
                await PipeServer.SendMessage(message);

            });
        }

        public static async System.Threading.Tasks.Task SendMessage(string message)
        {
            if (PipeName == null)
            {
                return;

            }

            try
            {
                using (var pipeClient = new NamedPipeClientStream(".", PipeName, PipeDirection.InOut, PipeOptions.Asynchronous))
                {
                    /*await pipeClient.ConnectAsync();
                    byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                    await pipeClient.WriteAsync(messageBytes, 0, messageBytes.Length);*/

                    await pipeClient.ConnectAsync();

                    try
                    {
                        using (var reader = new StreamReader(pipeClient))
                        using (var writer = new StreamWriter(pipeClient) { AutoFlush = true })
                        {
                            string messageToSend = message;
                            await writer.WriteLineAsync(messageToSend);
                            string response = await reader.ReadLineAsync();
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
