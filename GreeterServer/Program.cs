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
            return Task.FromResult(
                new HelloReply { Message = $"hello, {request.Name}!" });
        }
    }

    class Program
    {
        public static void Main(string[] args) => MainAsync(9001).GetAwaiter().GetResult();

        public static async Task MainAsync(int port)
        {
            await Task.Delay(3000);

            var channel = new Channel("127.0.0.1", 9002, ChannelCredentials.Insecure);
            var client  = new Greeter.GreeterClient(channel);
            var result  = await client.SayHelloAsync(new HelloRequest() { Name = "me" }); // <-- comment out this call and the solution works as expected
            var server  = new Server
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
