using NVRCsharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace HIKPlayer.HIK
{
    internal class NVRInfo
    {

        

        public CHCNetSDK.NET_DVR_DEVICEINFO_V30 DeviceInfo { get; set; }

        public uint dwAChanTotalNum {  get; set; }
        public uint dwDChanTotalNum { get; set; }

        public int UserId { get; set; }


        public int[] ChannelNum { get;set; } = new int[96];


        public List<int> RealHandles { get; set; } = new List<int>();
    }
}
