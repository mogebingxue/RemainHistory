using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
namespace YT
{
    public class ConfigHelper
    {
        /// <summary>
        /// 初始化网络配置信息
        /// </summary>
        /// <returns></returns>
        public static NetConfig GetNetConfig() {
            using (FileStream fsRead = new FileStream(@"..\..\..\Config\NetConfig.json", FileMode.Open)) {
                int fsLen = (int)fsRead.Length;
                byte[] bytes = new byte[fsLen];
                int r = fsRead.Read(bytes, 0, bytes.Length);
                //反序列化网络配置信息
                NetConfig netConfig = JsonConvert.DeserializeObject<NetConfig>(System.Text.Encoding.UTF8.GetString(bytes));
                return netConfig;
            }
        }
    }
}

