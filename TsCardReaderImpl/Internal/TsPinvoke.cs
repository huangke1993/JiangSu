using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using ConmonMessage;
using DeviceMessage;
using ICardReaderDeclare;
using ICardReaderDeclare.Enum;

namespace TsCardReaderImpl.Internal
{
    internal class TsPinvoke
    {
        private const string DllFile = @"TsDll\\SSCardDriver.dll";

        [DllImport(DllFile, EntryPoint = "iReadCertInfo", CharSet = CharSet.Ansi, SetLastError = false,
            CallingConvention = CallingConvention.StdCall)]
        public static extern int ReadIdCard(int type,string photoPath,StringBuilder photoData,StringBuilder message);
        [DllImport(DllFile, EntryPoint = "iReadCardBas", CharSet = CharSet.Ansi, SetLastError = false,
            CallingConvention = CallingConvention.StdCall)]
        public static extern int ReadCardBas(int type, StringBuilder message);
    }
    internal class TsReaderInternal
    {
        private const int SocialSuccessCode = 0;
        private static readonly List<int> IdSuccessCode = new List<int>() {0, 1, 2};
        private const int NoCreatePhoto = 0;
        public static IMessage<IPersonInfo> ReadIdCard()
        {
            var info = new StringBuilder(2048);
            var ret = TsPinvoke.ReadIdCard(NoCreatePhoto, string.Empty, new StringBuilder(1024*10), info);
            return IdSuccessCode.Contains(ret)
                ? CommonDeviceMsg<TsPersonInfo>.CreateSuccess(TsPersonInfo.CreateByIdResult(ret,info.ToString()))
                : CommonDeviceMsg<TsPersonInfo>.CreateFail(info.ToString());
        }
        public static IMessage<IPersonInfo> ReadSocialCard(CardType cardType)
        {
            var info = new StringBuilder(1024);
            var ret = TsPinvoke.ReadCardBas((int)cardType, info);
            return ret == SocialSuccessCode
                ? CommonDeviceMsg<TsPersonInfo>.CreateSuccess(TsPersonInfo.CreateBySocialResult(info.ToString()))
                : CommonDeviceMsg<TsPersonInfo>.CreateFail(info.ToString());
        }
    }

    internal class TsPersonInfo : BaseCmpPersonInfo
    {
        private static readonly Dictionary<int, Func<string[], TsPersonInfo>> GetInfoById;

        static TsPersonInfo()
        {
            GetInfoById = new Dictionary<int, Func<string[], TsPersonInfo>>
            {
                {0, arrary => new TsPersonInfo() {Name = arrary[0], IdNum = arrary[5]}},
                {1, arrary => new TsPersonInfo() {Name = arrary[1], IdNum = arrary[5]}},
                {2, arrary => new TsPersonInfo() {Name = arrary[0], IdNum = arrary[4]}}
            };
        }

        public static TsPersonInfo CreateByIdResult(int code,string data)
        {
            var dataArrary = data.Trim('|').Split('|');
            return GetInfoById[code](dataArrary);
        }

        public static TsPersonInfo CreateBySocialResult(string data)
        {
            var dataArrary = data.Trim('|').Split('|');
            return new TsPersonInfo()
            {
                Name = dataArrary[4],
                IdNum = dataArrary[1]
            };
        }
    }
}
