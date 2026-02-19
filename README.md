# WakeOnLan
This is a simple Console Application that sends a "Wake on LAN" packet to a specified IP Address.  I wrote this application so that I could
wake a PC on my network so I could make a remote connection.

## Hosts.csv
Hosts.csv contains information about the PC's on your network that you wish to send this packet to.  The format is
<PC Name>,<IP Addresss>,<MAC Address>

Example:
Test_Computer,192.168.1.1,01-02-03-04-05-06

## PC Setup
The BIOS must be configured on the target PC to allow it to respond to a Wake on LAN packet.