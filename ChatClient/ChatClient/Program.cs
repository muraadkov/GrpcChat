using ChatService.Protos;
using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Write("Please enter your name: ");
            var username = Console.ReadLine();
            List<string> history = new List<string>();
            var channel = GrpcChannel.ForAddress("http://localhost:5001");
            var client = new ChatRoomService.ChatRoomServiceClient(channel);

            using (var chat = client.Join())
            {
                _ = Task.Run(async () =>
                {
                    while (await chat.ResponseStream.MoveNext())
                    {
                        var response = chat.ResponseStream.Current;
                        Console.WriteLine($"{response.User} : {response.Text}");
                    }
                });
                foreach(var hist in history)
                {
                    Console.WriteLine(hist + "\n");
                }

                await chat.RequestStream.WriteAsync(new Message { User = username, Text = $"{username} has joined the chat!" });

                string line;

                while ((line = Console.ReadLine()) != null)
                {

                    if (line.ToUpper() == "EXIT")
                    {
                        break;
                    }
                    history.Add(line);
                    await chat.RequestStream.WriteAsync(new Message { User = username, Text = line });
                }

                await chat.RequestStream.CompleteAsync();
            }

            Console.WriteLine("Disconnection started!");
            await channel.ShutdownAsync();
        }
    }
}
