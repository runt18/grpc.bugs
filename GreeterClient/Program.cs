using System;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Helloworld;

namespace GreeterClient
{
    class Program
    {
        static int[] progress;
        static Channel[] channels;

        public static void Main(string[] args) => MainAsync().GetAwaiter().GetResult();

        public static async Task MainAsync()
        {
            await Task.Delay(5000);

            var concurrency = 10;

            progress = new int[concurrency];
            channels = new Channel[concurrency];

            await Task.WhenAll(
                Enumerable.Range(0, concurrency).Select(WorkAsync));

            Console.WriteLine("press enter to exit.");
            Console.ReadLine();
        }
        
        static async Task WorkAsync(int thread)
        {
            var channel = channels[thread] = new Channel("127.0.0.1:9001", ChannelCredentials.Insecure);
            var client  = new Greeter.GreeterClient(channel);
            var i       = 0;

            while (true)
            {
                try
                {
                    var reply = await client.SayHelloAsync(new HelloRequest() { Name = $"{thread}.{i}" });
                    Console.WriteLine($"    thread #{thread}: {reply}");

                    progress[thread] = ++i;
                    Console.Title = string.Join(", ", progress);

                    // break on the below line to inspect variables. all `channels`, including the non-responsive ones,
                    // indicate `status == ready`. alternatively, remove this line to continuously count invocations.
                    while (true)
                        await Task.Delay(100);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"    thread #{thread}: {e.GetType().Name} - {e.Message}");
                }
            }

            await channel.ShutdownAsync();
        }
    }
}
