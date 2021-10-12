using Grpc.Net.ClientFactory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using static GrpcService.Greeter;

namespace ConsoleAppNetCore
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            services
                .AddGrpcClient<GreeterClient>(
                    "WinHttp",
                    (options) =>
                    {
                        options.Address = new Uri("https://localhost:44323/");
                    })
                .ConfigurePrimaryHttpMessageHandler(
                    () => new WinHttpHandler()
                    {
                        EnableMultipleHttp2Connections = true
                    });
            services
                .AddGrpcClient<GreeterClient>(
                    "Sockets",
                    (options) =>
                    {
                        options.Address = new Uri("https://localhost:44323/");
                    });

            var serviceProvider = services.BuildServiceProvider();

            var factory = serviceProvider.GetRequiredService<GrpcClientFactory>();
            var winHttpClient = factory.CreateClient<GreeterClient>("WinHttp");
            var socketsClient = factory.CreateClient<GreeterClient>("Sockets");

            var resp1 = socketsClient.SayHello(
                new GrpcService.HelloRequest
                {
                    Name = "ConsoleAppNetCore"
                });
            try
            {
                var resp2 = winHttpClient.SayHello(
                    new GrpcService.HelloRequest
                    {
                        Name = "ConsoleAppNetCore"
                    });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
