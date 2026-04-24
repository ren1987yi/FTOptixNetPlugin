using NVRCsharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIKPlayer.HIK
{
    internal class HIKHelper
    {
        public static void Login(string ip,Int16 port,string user,string pwd,ref NVRInfo info)
        {
            CHCNetSDK.NET_DVR_DEVICEINFO_V30 dev = info.DeviceInfo;
            var lUserID = CHCNetSDK.NET_DVR_Login_V30(ip, port, user, pwd, ref dev);

            info.UserId = lUserID;

            if (lUserID < 0)
            {

            }
            else
            {
                var dwAChanTotalNum = (uint)dev.byChanNum;
                var dwDChanTotalNum = (uint)dev.byIPChanNum + 256 * (uint)dev.byHighDChanNum;
                info.dwAChanTotalNum = dwAChanTotalNum;
                info.dwDChanTotalNum=dwDChanTotalNum;
                if (dwDChanTotalNum > 0)
                {
                    InfoIPChannel(ref info);
                }
                else
                {
                    info.ChannelNum = new int[dwAChanTotalNum];

                    for (var i = 0; i < dwAChanTotalNum; i++)
                    {
                        //ListAnalogChannel(i + 1, 1);
                        info.ChannelNum[i] = i + (int)dev.byStartChan;
                    }

                    //comboBoxView.SelectedItem = 1;
                    // MessageBox.Show("This device has no IP channel!");
                }
            }


        }



        public static void InfoIPChannel(ref NVRInfo info)
        {
            CHCNetSDK.NET_DVR_IPPARACFG_V40 m_struIpParaCfgV40 = default;

            uint dwSize = (uint)Marshal.SizeOf(m_struIpParaCfgV40);

            IntPtr ptrIpParaCfgV40 = Marshal.AllocHGlobal((Int32)dwSize);
            Marshal.StructureToPtr(m_struIpParaCfgV40, ptrIpParaCfgV40, false);

            uint dwReturn = 0;
            int iGroupNo = 0;  //该Demo仅获取第一组64个通道，如果设备IP通道大于64路，需要按组号0~i多次调用NET_DVR_GET_IPPARACFG_V40获取

            if (!CHCNetSDK.NET_DVR_GetDVRConfig(info.UserId, CHCNetSDK.NET_DVR_GET_IPPARACFG_V40, iGroupNo, ptrIpParaCfgV40, dwSize, ref dwReturn))
            {
                //iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                //str = "NET_DVR_GET_IPPARACFG_V40 failed, error code= " + iLastErr;
                //获取IP资源配置信息失败，输出错误号 Failed to get configuration of IP channels and output the error code
                //DebugInfo(str);
            }
            else
            {
                //DebugInfo("NET_DVR_GET_IPPARACFG_V40 succ!");

                m_struIpParaCfgV40 = (CHCNetSDK.NET_DVR_IPPARACFG_V40)Marshal.PtrToStructure(ptrIpParaCfgV40, typeof(CHCNetSDK.NET_DVR_IPPARACFG_V40));

                for (var i = 0; i < info.dwAChanTotalNum; i++)
                {
                    //ListAnalogChannel(i + 1, m_struIpParaCfgV40.byAnalogChanEnable[i]);
                    info.ChannelNum[i] = i + (int)info.DeviceInfo.byStartChan;
                }

                byte byStreamType = 0;
                uint iDChanNum = 64;

                if (info.dwDChanTotalNum < 64)
                {
                    iDChanNum = info.dwDChanTotalNum; //如果设备IP通道小于64路，按实际路数获取
                }

                for (var i = 0; i < iDChanNum; i++)
                {
                    info.ChannelNum[i + info.dwAChanTotalNum] = i + (int)m_struIpParaCfgV40.dwStartDChan;
                    byStreamType = m_struIpParaCfgV40.struStreamMode[i].byGetStreamType;

                    dwSize = (uint)Marshal.SizeOf(m_struIpParaCfgV40.struStreamMode[i].uGetStream);
                    switch (byStreamType)
                    {
                        //目前NVR仅支持直接从设备取流 NVR supports only the mode: get stream from device directly
                        case 0:
                            IntPtr ptrChanInfo = Marshal.AllocHGlobal((Int32)dwSize);
                            Marshal.StructureToPtr(m_struIpParaCfgV40.struStreamMode[i].uGetStream, ptrChanInfo, false);
                            var m_struChanInfo = (CHCNetSDK.NET_DVR_IPCHANINFO)Marshal.PtrToStructure(ptrChanInfo, typeof(CHCNetSDK.NET_DVR_IPCHANINFO));

                            //列出IP通道 List the IP channel
                            //ListIPChannel(i + 1, m_struChanInfo.byEnable, m_struChanInfo.byIPID);
                            //iIPDevID[i] = m_struChanInfo.byIPID + m_struChanInfo.byIPIDHigh * 256 - iGroupNo * 64 - 1;

                            Marshal.FreeHGlobal(ptrChanInfo);
                            break;
                        case 4:
                            IntPtr ptrStreamURL = Marshal.AllocHGlobal((Int32)dwSize);
                            Marshal.StructureToPtr(m_struIpParaCfgV40.struStreamMode[i].uGetStream, ptrStreamURL, false);
                            var m_struStreamURL = (CHCNetSDK.NET_DVR_PU_STREAM_URL)Marshal.PtrToStructure(ptrStreamURL, typeof(CHCNetSDK.NET_DVR_PU_STREAM_URL));

                            //列出IP通道 List the IP channel
                            //ListIPChannel(i + 1, m_struStreamURL.byEnable, m_struStreamURL.wIPID);
                            //iIPDevID[i] = m_struStreamURL.wIPID - iGroupNo * 64 - 1;

                            Marshal.FreeHGlobal(ptrStreamURL);
                            break;
                        case 6:
                            IntPtr ptrChanInfoV40 = Marshal.AllocHGlobal((Int32)dwSize);
                            Marshal.StructureToPtr(m_struIpParaCfgV40.struStreamMode[i].uGetStream, ptrChanInfoV40, false);
                            var m_struChanInfoV40 = (CHCNetSDK.NET_DVR_IPCHANINFO_V40)Marshal.PtrToStructure(ptrChanInfoV40, typeof(CHCNetSDK.NET_DVR_IPCHANINFO_V40));

                            //列出IP通道 List the IP channel
                            //ListIPChannel(i + 1, m_struChanInfoV40.byEnable, m_struChanInfoV40.wIPID);
                            //iIPDevID[i] = m_struChanInfoV40.wIPID - iGroupNo * 64 - 1;

                            Marshal.FreeHGlobal(ptrChanInfoV40);
                            break;
                        default:
                            break;
                    }
                }
            }
            Marshal.FreeHGlobal(ptrIpParaCfgV40);

        }


     


        public static void Preview(NVRInfo info,int chNo,PictureBox player)
        {




            CHCNetSDK.NET_DVR_PREVIEWINFO lpPreviewInfo = new CHCNetSDK.NET_DVR_PREVIEWINFO();
            lpPreviewInfo.hPlayWnd = player.Handle;//预览窗口 live view window
            lpPreviewInfo.lChannel = info.ChannelNum[chNo];//预览的设备通道 the device channel number
            lpPreviewInfo.dwStreamType = 1;//码流类型：0-主码流，1-子码流，2-码流3，3-码流4，以此类推
            lpPreviewInfo.dwLinkMode = 0;//连接方式：0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4-RTP/RTSP，5-RSTP/HTTP 
            lpPreviewInfo.bBlocked = false; //0- 非阻塞取流，1- 阻塞取流
            lpPreviewInfo.dwDisplayBufNum = 15; //播放库显示缓冲区最大帧数

            IntPtr pUser = IntPtr.Zero;//用户数据 user data 

            info.RealHandles.Add(CHCNetSDK.NET_DVR_RealPlay_V40(info.UserId, ref lpPreviewInfo, null/*RealData*/, pUser));

        }

    }
}
