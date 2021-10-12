using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using static GrpcService.Greeter;

namespace ConsoleAppNetFramework
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            services
                .AddGrpcClient<GreeterClient>(
                    (options) =>
                    {
                        options.Address = new Uri("https://localhost:44323/");
                    })
                .ConfigurePrimaryHttpMessageHandler(
                    () => new WinHttpHandler()
                    {
                        EnableMultipleHttp2Connections = true
                    });

            var serviceProvider = services.BuildServiceProvider();

            var client = serviceProvider.GetRequiredService<GreeterClient>();
            var resp1 = client.SayHello(
                new GrpcService.HelloRequest
                {
                    Name = "ConsoleAppNetFramework"
                });
            try
            {
                var resp2 = client.SayHello(
                    new GrpcService.HelloRequest
                    {
                        Name = "ConsoleAppNetFramework"
                    });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
