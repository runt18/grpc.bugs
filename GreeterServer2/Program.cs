using System;
using System.Threading.Tasks;
using Grpc.Core;
using Helloworld;

namespace GreeterServer
{
    class GreeterImplementation : Greeter.GreeterBase
    {
        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            Console.WriteLine($"::got-request {request.Name}");

            return Task.FromResult(
                new HelloReply { Message = $"hello, {request.Name}!" });
        }
    }

    class Program
    {
        public static void Main(string[] args) => MainAsync(9002).GetAwaiter().GetResult();

        public static async Task MainAsync(int port)
        {
            var server = new Server
            {
                Services = { Greeter.BindService(new GreeterImplementation()) },
                Ports = { new ServerPort("localhost", port, ServerCredentials.Insecure) }
            };

            server.Start();

            Console.WriteLine($"listening on port {port}. press enter to exit.");
            Console.ReadLine();

            await server.ShutdownAsync();
        }
    }
}
