using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.NetworkInformation;

namespace iFactory.CommonLibrary
{
    public class NetworkHelper
    {
        /// <summary>
        /// 通过ping命令测试网络是否连通
        /// </summary>
        /// <param name="IpAddr"></param>
        /// <returns></returns>
        public static bool IsNetWorkConnect(string IpAddr)
        {
            try
            {
                Ping p1 = new Ping();
                PingReply reply = p1.Send(IpAddr, 1000); //发送主机名或Ip地址,2S超时
                StringBuilder sbuilder;
                if (reply.Status == IPStatus.Success)
                {
                    sbuilder = new StringBuilder();
                    sbuilder.AppendLine(string.Format("Address: {0} ", reply.Address.ToString()));
                    sbuilder.AppendLine(string.Format("RoundTrip time: {0} ", reply.RoundtripTime));
                    sbuilder.AppendLine(string.Format("Time to live: {0} ", reply.Options.Ttl));
                    sbuilder.AppendLine(string.Format("Don't fragment: {0} ", reply.Options.DontFragment));
                    sbuilder.AppendLine(string.Format("Buffer size: {0} ", reply.Buffer.Length));
                    //Console.WriteLine(sbuilder.ToString());
                    return true;
                }
                else if (reply.Status == IPStatus.TimedOut)
                {
                    //Console.WriteLine("超时");
                    return false;
                }
                else
                {
                    //Console.WriteLine("失败");
                    return false;
                }
            }
            catch(Exception ex)
            { }
            return false;
        }
    }
}
