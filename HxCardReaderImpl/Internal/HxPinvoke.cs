using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using ConmonMessage;
using DeviceMessage;
using ICardReaderDeclare;

namespace HxCardReaderImpl.Internal
{
    internal class HxPinvoke
    {
        private const string DllFile = @"HxDll\sdtapi.dll";

        [DllImport(DllFile, EntryPoint = "SDT_OpenPort", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern int OpenReader(int port);
        [DllImport(DllFile, EntryPoint = "SDT_ClosePort", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern int CloseReader(int port);
        [DllImport(DllFile, EntryPoint = "SDT_StartFindIDCard", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern int StartFindIdCard(int iPort, byte[] pucManaInfo, int iIfOpen);
        [DllImport(DllFile, EntryPoint = "SDT_SelectIDCard", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern int SelectIdCard(int iPort, byte[] pucManaMsg, int iIfOpen);

        [DllImport(DllFile, EntryPoint = "SDT_ReadBaseMsg", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern int ReadCard(int iPort, byte[] pucChMsg, ref uint puiChMsgLen, byte[] pucPhMsg, ref uint puiPhMsgLen, int iIfOpen);

    }

    internal class HxReaderInternal
    {
        private const int SuccessCode = 0x90;
        private static readonly Dictionary<int, string> Result;

        static HxReaderInternal()
        {
            Result = new Dictionary<int, string>() {{0x91, "居民身份证中无此项内容"}, { 0x01, "端口打开失败"}, { 0x02, "PC接收超时，在规定的时间内未接收到规定长度的数据" },{ 0x03, "数据传输错误" }, { 0x05, "SAM_A串口不可用" }, { 0x09, "打开文件失败" }, { 0x10, "接收业务终端数据的校验和错" }, { 0x11, "接收业务终端数据的长度错" }, { 0x21, "接收业务终端的命令错误，包括命令中的各种数值或逻辑搭配错误" }, { 0x23, "越权操作" }, { 0x24, "无法识别的错误" }, { 0x80, "寻找居民身份证失败" }, { 0x81, "选取居民身份证失败" }, { 0x31, "居民身份证认证SAM_A失败" }, { 0x32, "SAM_A认证居民身份证失败" }, { 0x33,"信息验证失败" }, { 0x37, "指纹信息验证错误" }, { 0x3F, "信息长度错误" }, { 0x40, "无法识别的居民身份证类型" }, { 0x41, "读居民身份证操作失败" }, { 0x47, "取随机数失败" }, { 0x60, "SAM_A自检失败，不能接收命令" }, { 0x66, "SAM_A没经过授权 无法使用" } };
        }

        public static IMessage OpenReader(int port)
        {
           var ret= HxPinvoke.OpenReader(port);
           return ret == SuccessCode
               ? CommonDeviceMsg.CreateSuccess()
               : CommonDeviceMsg.CreateFail(Result[ret]);
        }
        public static void CloseReader(int port)
        {
            HxPinvoke.CloseReader(port);
        }

        private const int OpenReaderOutside = 0;
        private const int FindIdSuccessCode = 0x9f;
        public static IMessage<IPersonInfo> ReadCard(int port)
        {
            var findIdRet = HxPinvoke.StartFindIdCard(port, new byte[4], OpenReaderOutside);
            if (findIdRet != FindIdSuccessCode)
                return CommonDeviceMsg<HxPersonInfo>.CreateFail(Result[findIdRet]);
            var selectIdRet = HxPinvoke.SelectIdCard(port, new byte[8], OpenReaderOutside);
            if (selectIdRet != SuccessCode)
                return CommonDeviceMsg<HxPersonInfo>.CreateFail(Result[selectIdRet]);
            var byChMsg = new byte[257];        //个人基本信息
            uint uiChMsgSize = 0;                       //个人基本信息字节数
            var byPhMsg = new byte[1025];       //照片信息
            uint uiPhMsgSize = 0;	                    //照片信息字节数
            var readRet = HxPinvoke.ReadCard(port, byChMsg, ref uiChMsgSize, byPhMsg, ref uiPhMsgSize, OpenReaderOutside);
            if (readRet != SuccessCode)
                return CommonDeviceMsg<HxPersonInfo>.CreateFail(Result[readRet]);
            return CommonDeviceMsg<HxPersonInfo>.CreateSuccess(HxPersonInfo.CreateHxPersonInfo(byChMsg));
        }
    }

    internal class HxPersonInfo : BaseCmpPersonInfo
    {
        private static readonly Dictionary<byte, Func<byte[], HxPersonInfo>> GetInfoById;

        static HxPersonInfo()
        {
            GetInfoById = new Dictionary<byte, Func<byte[], HxPersonInfo>>
            {
                {0x49, CreateByFgnCard},
                {0x4A, CreateBySpecialCard}
            };
        }

        private HxPersonInfo(string name, string idNum)
        {
            Name = name;
            IdNum = idNum;
        }

        public static HxPersonInfo CreateHxPersonInfo(byte[] data)
        {
            const int cardTypeStartIndex = 248;
            const int cardTypeLength = 2;
            var cardTypeFlag = data.Skip(cardTypeStartIndex).Take(cardTypeLength).ToArray();
            if(GetInfoById.ContainsKey(cardTypeFlag.First()))return GetInfoById[cardTypeFlag.First()](data);
            return CreateByIdCard(data);
        }

        private static HxPersonInfo CreateByFgnCard(byte[] data)
        {
            const int cnNameStartIndex = 158;
            const int cnNameLength = 30;
            const int cardNoStartIndex = 122;
            const int cardNoLength = 30;
            var cnName = GetStringByByteArrary(data.Skip(cnNameStartIndex).Take(cnNameLength).ToArray());
            var cardNo= GetStringByByteArrary(data.Skip(cardNoStartIndex).Take(cardNoLength).ToArray());
            return new HxPersonInfo(cnName, cardNo);
        }
        private static HxPersonInfo CreateBySpecialCard(byte[] data)
        {
            const int nameStartIndex = 0;
            const int nameLength = 30;
            const int cardNoStartIndex = 122;
            const int cardNoLength = 36;
            var cnName = GetStringByByteArrary(data.Skip(nameStartIndex).Take(nameLength).ToArray());
            var cardNo = GetStringByByteArrary(data.Skip(cardNoStartIndex).Take(cardNoLength).ToArray());
            return new HxPersonInfo(cnName, cardNo);
        }
        private static HxPersonInfo CreateByIdCard(byte[] data)
        {
            const int nameStartIndex = 0;
            const int nameLength = 30;
            const int cardNoStartIndex = 122;
            const int cardNoLength = 36;
            var cnName = GetStringByByteArrary(data.Skip(nameStartIndex).Take(nameLength).ToArray());
            var cardNo = GetStringByByteArrary(data.Skip(cardNoStartIndex).Take(cardNoLength).ToArray());
            return new HxPersonInfo(cnName, cardNo);
        }
        private static string GetStringByByteArrary(byte[]data)
        {
            return Encoding.Unicode.GetString(data).Trim();
        }
    }
}
