using System.Net;
using System.Net.NetworkInformation;

namespace WakeOnLan
{
    internal class Host
    {
        private const int NAME_INDEX = 0;
        private const int IP_ADDR_INDEX = 1;
        private const int MAC_ADDR_INDEX = 2;

        public string Name { get; set; } = string.Empty;
        public IPAddress IPAddress { get; set; } = IPAddress.None;
        public PhysicalAddress MACAddress { get; set; } = PhysicalAddress.None;

        public Host()
        { }

        public Host(string csvLine)
        {
            if (string.IsNullOrEmpty(csvLine))
                throw new ArgumentException("CSV Line cannot be null or empty.");

            var split = csvLine.Split(',');

            if (split.Length < 3)
                throw new ArgumentException($"Invalid CSV Line. Must contain three parameters. Given: {csvLine}");

            if (string.IsNullOrEmpty(split[NAME_INDEX]))
                throw new ArgumentException($"Invalid CSV Line. Name cannot be null or empty.");

            Name = split[NAME_INDEX];

            if (string.IsNullOrEmpty(split[IP_ADDR_INDEX]) || !IPAddress.TryParse(split[IP_ADDR_INDEX], out var ipAddress))
                throw new ArgumentException($"Invalid CSV Line. Failed to parse IP Address. Given: {csvLine}");

            IPAddress = ipAddress;

            if (string.IsNullOrEmpty(split[MAC_ADDR_INDEX]) || !PhysicalAddress.TryParse(split[MAC_ADDR_INDEX], out var macAddress))
                throw new ArgumentException($"Invalid CSV Line. Failed to parse MAC Address. Given: {csvLine}");

            MACAddress = macAddress;
        }
    }
}
