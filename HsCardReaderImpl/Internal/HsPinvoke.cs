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

        [DllImport(DllFile, EntryPoint = "CVR_Read_Content", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern int ReadCard(int active);

        [DllImport(DllFile, EntryPoint = "CVR_CloseComm", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern int CloseReader();
        [DllImport(DllFile, EntryPoint = "GetPeopleName", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern int GetPeopleName(StringBuilder name,ref int length);
        [DllImport(DllFile, EntryPoint = "GetPeopleIDCode", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern int GetPeopleIdNum(StringBuilder idNum, ref int length);

    }

    internal class HsReaderInternal
    {
        private const int SuccessCode = 1;
        private static readonly Dictionary<int, string> OpenResult;

        static HsReaderInternal()
        {
            OpenResult = new Dictionary<int, string>() {{0, "动态库加载失败"}, {2, "端口打开失败"}};
            ReadResult=new Dictionary<int, string>(){ { 0, "读数据错误" }, { 99, "读数据异常" } };
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

        public static IMessage<IPersonInfo> ReadCard()
        {
            var name=new StringBuilder(1024);
            var nameLength = 0;
            var idNum = new StringBuilder(1024);
            var idNumLength = 0;
            var ret = HsPinvoke.ReadCard(NotMean);
            if (ret != SuccessCode)
                return CommonDeviceMsg<HsPersonInfo>.CreateFail(ReadResult[ret]);
            var readName = HsPinvoke.GetPeopleName(name, ref nameLength);
            if(readName!=SuccessCode)return CommonDeviceMsg<HsPersonInfo>.CreateFail(ReadResult[readName]);
            var readIdNum= HsPinvoke.GetPeopleIdNum(idNum, ref idNumLength);
            if (readIdNum != SuccessCode) return CommonDeviceMsg<HsPersonInfo>.CreateFail(ReadResult[readIdNum]);
            return CommonDeviceMsg<HsPersonInfo>.CreateSuccess(new HsPersonInfo(name.ToString().Trim(),
                idNum.ToString().Trim()));
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
