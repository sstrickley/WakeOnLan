using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;

namespace WakeOnLan
{
    internal class Program
    {
        private const string HOSTS_FILE_NAME = "Hosts.csv";

        static void Main(string[] args)
        {
            Console.WriteLine("Starting Application...");

            var isRunning = true;

            while(isRunning)
            {
                var packet = new byte[102];

                // 0xFF must appear six times in the packet.
                for (var i = 0; i < 6; i++)
                    packet[i] = 0xFF;

                var ipAddress = IPAddress.None;

                var hosts = LoadHosts();

                Console.WriteLine();

                Console.WriteLine("--------------------------------------------------------------");
                Console.WriteLine("Hosts:");

                for (var i = 0; i < hosts.Count; i++)
                    Console.WriteLine($"{i}: {hosts[i].Name} - {hosts[i].IPAddress} - {hosts[i].MACAddress}");

                Console.WriteLine("--------------------------------------------------------------");
                Console.WriteLine();

                Console.Write("Enter Index of Host to Wake: ");

                if (!int.TryParse(Console.ReadLine(), out var index) || index < 0 || index > hosts.Count - 1)
                    Console.WriteLine("Invalid Index.");
                else
                {
                    AddMACToPacket(hosts[index].MACAddress, packet);
                    SendPacket(hosts[index].IPAddress, 0, packet);
                }

                Console.Write("Exit Application? (Y/N) ");
                var exit = Console.ReadLine();

                if (string.IsNullOrEmpty(exit) || exit.Equals("Y", StringComparison.InvariantCultureIgnoreCase))
                    isRunning = false;
                
            }

            Console.WriteLine("Application Complete");
            Console.ReadLine();
        }

        static IList<Host> LoadHosts()
        {
            var executingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (string.IsNullOrEmpty(executingDirectory))
                throw new InvalidOperationException("Unable to get executing directory.");

            var path = Path.Combine(executingDirectory, HOSTS_FILE_NAME);

            if (!File.Exists(path))
                throw new FileNotFoundException($"Unable to find configuration file at: {path}");

            var output = new List<Host>();

            using (var reader = new StreamReader(path))
            {
                while(!reader.EndOfStream)
                {
                    var line = reader.ReadLine();

                    if (string.IsNullOrEmpty(line))
                        continue;

                    try
                    {
                        output.Add(new Host(line));
                    }
                    catch(Exception exp)
                    {
                        Console.WriteLine($"Exception on loading configuration. {exp.GetType().Name}: {exp.Message}");
                    }
                }
            }

            return output;
        }

        static void AddMACToPacket(PhysicalAddress address, byte[] packet)
        {
            var addressBytes = address.GetAddressBytes();

            // MAC Address is repeated 16 times in the packet.
            var idx = 6;

            for (var i = 0; i < 16; i++)
            {
                for (var j = 0; j < addressBytes.Length; j++)
                    packet[idx++] = addressBytes[j];
            }
        }

        static void SendPacket(IPAddress ipAddress, int port, byte[] packet)
        {
            Console.WriteLine("Sending UDP Packet...");
            Console.WriteLine(BitConverter.ToString(packet));

            var client = new UdpClient();
            client.Connect(ipAddress, port);
            Console.WriteLine("Connected at {0}:{1}", ipAddress, port);
            client.Send(packet);

            Console.WriteLine("Packet Sent.");
        }
    }
}
