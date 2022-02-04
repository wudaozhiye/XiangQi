using Common;
using GateServer.Net;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace GateServer
{
    class Program
    {
        private static IClusterClient client;

        private static TcpServer tcpServer;

        /// <summary>
        /// 从配置文件读取到的NettyPort
        /// </summary>
        public static int nettyPort;

        static async Task Main(string[] args)
        {
            nettyPort = Convert.ToInt32(ConfigurationManager.AppSettings["NettyPort"]);

            Logger.Create("GateServer");

            await ConnectClient();

            Logger.Instance.Information("网关服务器(GateServer)链接游戏服务器(CardServer)成功!");

            tcpServer = new TcpServer(client);

            await tcpServer.StartAsync();
        }

        /// <summary>
        /// 网关服务器(GateServer)链接游戏服务器(CardServer)
        /// </summary>
        /// <returns></returns>
        private static async Task<IClusterClient> ConnectClient()
        {
            client = new ClientBuilder()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "ClusterId";
                    options.ServiceId = "ServiceId";
                })
                //.UseLocalhostClustering()
                .UseAdoNetClustering(options =>
                {
                    options.Invariant = "MySql.Data.MySqlClient";
                    options.ConnectionString = "Server=localhost;Database=songoftheknights;Uid=root;Pwd=123456;";
                })
                .Build();

            await client.Connect();

            return client;
        }
    }
}
