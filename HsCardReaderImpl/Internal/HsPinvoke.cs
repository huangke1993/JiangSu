using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using ConmonMessage;
using DeviceMessage;
using ICardReaderDeclare;

namespace HsCardReaderImpl.Internal
{
    internal class HsPinvoke
    {
        private const string DllFile = @"HsDll\termb.dll";

        [DllImport(DllFile, EntryPoint = "CVR_InitComm", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern int OpenReader(int port);
        [DllImport(DllFile, EntryPoint = "CVR_Authenticate", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern int Authenticate();
        [DllImport(DllFile, EntryPoint = "CVR_Read_Content", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern int ReadCard(int active);

        [DllImport(DllFile, EntryPoint = "CVR_CloseComm", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern int CloseReader();
        [DllImport(DllFile, EntryPoint = "GetPeopleName", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern int GetPeopleName(ref byte strTmp, ref int strLen);
        [DllImport(DllFile, EntryPoint = "GetPeopleIDCode", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern int GetPeopleIdNum(ref byte strTmp, ref int strLen);

    }

    internal class HsReaderInternal
    {
        private const int SuccessCode = 1;
        private static readonly Dictionary<int, string> OpenResult;

        static HsReaderInternal()
        {
            OpenResult = new Dictionary<int, string>() {{0, "动态库加载失败"}, {2, "端口打开失败"}};
            ReadResult=new Dictionary<int, string>(){ { 0, "读数据错误" }, { 99, "读数据异常" } };
            AuthResult=new Dictionary<int, string>() { { 0, "初始化失败" }, { 2, "寻卡失败" }, { 3, "选卡失败" } };
        }

        public static IMessage OpenReader(int port)
        {
           var ret= HsPinvoke.OpenReader(port);
           return ret == SuccessCode
               ? CommonDeviceMsg.CreateSuccess()
               : CommonDeviceMsg.CreateFail(OpenResult[ret]);
        }
        public static void CloseReader()
        {
            HsPinvoke.CloseReader();
        }

        private const int NotMean = 0;
        private static readonly Dictionary<int, string> ReadResult;
        private static readonly Dictionary<int, string> AuthResult;
        private const int BufferSize = 1024;
        public static IMessage<IPersonInfo> ReadCard()
        {
            var name=new byte[BufferSize];
            var nameLength = 0;
            var idNum = new byte[BufferSize];
            var idNumLength = 0;
            var authRet = HsPinvoke.Authenticate();
            if (authRet != SuccessCode)
                return CommonDeviceMsg<HsPersonInfo>.CreateFail(AuthResult[authRet]);
            var readRet = HsPinvoke.ReadCard(NotMean);
            if (readRet != SuccessCode)
                return CommonDeviceMsg<HsPersonInfo>.CreateFail(ReadResult[readRet]);
            var readName = HsPinvoke.GetPeopleName(ref name[0], ref nameLength);
            if(readName!=SuccessCode)return CommonDeviceMsg<HsPersonInfo>.CreateFail(ReadResult[readName]);
            var readIdNum= HsPinvoke.GetPeopleIdNum(ref idNum[0], ref idNumLength);
            if (readIdNum != SuccessCode) return CommonDeviceMsg<HsPersonInfo>.CreateFail(ReadResult[readIdNum]);
            return CommonDeviceMsg<HsPersonInfo>.CreateSuccess(new HsPersonInfo(GetInfo(name),
                GetInfo(idNum)));
        }

        private static string GetInfo(byte[] oriData)
        {
            return Encoding.GetEncoding("GB2312").GetString(oriData).Replace("\0", "").Trim();
        }
    }

    internal class HsPersonInfo : BaseCmpPersonInfo
    {
        public HsPersonInfo(string name, string idNum)
        {
            Name = name;
            IdNum = idNum;
        }
    }
}
