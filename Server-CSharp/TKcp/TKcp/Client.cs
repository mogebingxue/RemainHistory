using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;

namespace TKcp
{
    public class Client
    {
        /// <summary>
        /// 重连时间
        /// </summary>
        public int Interval = 10;
        public Peer Peer;
        /// <summary>
        /// 客户端udp
        /// </summary>
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        IPEndPoint localIpep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8890);
        long connectTime;
        IPEndPoint serverIpep;

        Thread updateThread;
        Thread updatePeerThread;

        public Client() {
            Peer = new Peer(null, 0, null);
            socket.Bind(localIpep);
            updateThread = new Thread(Update);
            updatePeerThread = new Thread(UpdatePeer);
        }

        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        public long GetTimeStamp() {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);
        }

        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="server">服务器的IPEndPoint</param>
        public void Connect(IPEndPoint server) {
            Socket tcpSocket=new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            tcpSocket.Bind(localIpep);
            tcpSocket.Connect(server);

            byte[] buf = new byte[1024];
            int n = tcpSocket.Receive(buf);
            byte[] convBytes = new byte[n];
            Array.Copy(buf, convBytes, n);
            uint conv = System.BitConverter.ToUInt32(convBytes, 0);
            if (conv == 0) {
                Console.WriteLine("连接已满，请稍后重试");
            }
            else {
                serverIpep = server;
                Peer.LocalSocket = socket;
                Peer.Conv = conv;
                Peer.Remote = server;
                Peer.InitKcp();
                if (Peer.AcceptHandle != null) {
                    Peer.AcceptHandle(System.BitConverter.GetBytes(conv), 4);
                }
                updateThread.Start();
                updatePeerThread.Start();
                tcpSocket.Close();
                return;
                //Thread.Sleep(Timeout.Infinite);


            }
        }
        /// <summary>
        /// 客户端发送数据
        /// </summary>
        /// <param name="sendbuffer">发送的数据</param>
        public void Send(byte[] sendbuffer) {
            if (Peer == null) {
                Console.WriteLine("未与服务器建立连接");
                return;
            }
            Peer.Send(sendbuffer);
            
        }

        /// <summary>
        /// 更新接收信息
        /// </summary>
        void Update() {
            while (true) {

                if (socket.Available > 0) {
                    byte[] recvBuffer = new byte[socket.ReceiveBufferSize];
                    EndPoint remote = new IPEndPoint(IPAddress.Any, 0);
                    socket.ReceiveFrom(recvBuffer, ref remote);

                    Peer.Kcp.Input(recvBuffer);
                }
            }
        }

        /// <summary>
        /// 更新Peer
        /// </summary>
        void UpdatePeer() {
            
            while (true) {
                if (Peer == null) {
                    continue;
                }
                Peer.PeerUpdate();
                
            }

        }
        #region 注册回调

        public void AddReceiveHandle(Action<uint,byte[],int> method) {
            Peer.ReceiveHandle += method;
        }

        public void AddAcceptHandle(Action<byte[], int> method) {
            Peer.AcceptHandle += method;
        }

        public void AddTimeoutHandle(Action method) {
            Peer.TimeoutHandle += method;
        }

        #endregion

    }


}
