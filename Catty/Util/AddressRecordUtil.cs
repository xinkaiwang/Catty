using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Catty.Core.Util
{
    public class AddressRecordUtil
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AddressRecordUtil));

        public static IPAddress GetIPAddressFromString(string hostIPString)
        {
            IPAddress ip = null;
            // Step 1, try to resolve it as numbers "10.23.112.113"
            try
            {
                ip = IPAddress.Parse(hostIPString);
            }
            catch (FormatException)
            {
                // hmm... it does not looks like a ip address
            }
            if (ip != null) return ip;

            // step 2, hostName == self ?
            if (hostIPString.Equals("self", StringComparison.CurrentCultureIgnoreCase)) // we consider "self" as a special case to resolve to local ip addr, Note: this works well when we only have 1 local ip address.
            {
                IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());
                foreach (IPAddress addr in localIPs)
                {
                    if (addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        byte[] addrBytes = addr.GetAddressBytes();
                        if (addrBytes[0] == 192 && addrBytes[1] == 168 && addrBytes[2] == 56)
                        {
                            // xinkai: exclude special address 192.168.56.1 that's the virutalbox Host-Only network
                            continue;
                        }
                        if (addrBytes[0] == 169 && addrBytes[1] == 254 && addrBytes[2] == 70)
                        {
                            // xinjun: exclude special address 169.254.70.235 that's the virutalbox Host-Only network
                            continue;
                        }
                        // This addr looks good, use it.
                        ip = addr;
                        break;
                    }
                }

                //some machine which installs vmware or virtualpc may not get the right ipv4 address by above way
                //ip = IPAddress.Parse(System.Net.Dns.GetHostByName(Environment.MachineName).AddressList[0].ToString());
            }

            if (ip != null) return ip;

            // Step 3, try to resolve it as a dns name, like "A004.stracking.com", "localhost", etc.
            IPAddress[] addresslist = Dns.GetHostAddresses(hostIPString);
            foreach (IPAddress addr in addresslist)
            {
                if (addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    ip = addr;
                    break;
                }
            }
            if (ip != null) return ip;

            return null;
        }
        public static IPEndPoint GetIPEndPointFromString(string hostAndPort) // "10.23.112.113:3921", "A004.stracking.com:3921" or "self:3921"
        {
            hostAndPort = hostAndPort.Trim();
            IPEndPoint endpoint = null;
            try
            {
                int seperator = hostAndPort.LastIndexOf(':');
                if (seperator < 0)
                    goto exit;

                string hostStr = hostAndPort.Substring(0, seperator);
                string portStr = hostAndPort.Substring(seperator + 1);
                IPAddress ip = GetIPAddressFromString(hostStr);
                if (ip == null)
                    goto exit;
                int portNum = UInt16.Parse(portStr);

                endpoint = new IPEndPoint(ip, portNum);
            }
            catch (Exception e)
            {
                //Console.WriteLine(e.ToString());
                if (log.IsErrorEnabled)
                {
                    log.Error("Exception happend when we trying to resolve hostAndPort'" + hostAndPort + "' " + e.ToString());
                }
            }
        exit:
            if (endpoint == null && log.IsErrorEnabled)
            {
                log.Error("GetIPEndPointFromString() unable to resolve hostAndPort:'" + hostAndPort + "' ");
            }
            return endpoint;
        }
    }
}
