using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using ConmonMessage;
using DeviceMessage;
using ICardReaderDeclare;
using ICardReaderDeclare.Enum;

namespace DkCardReaderImpl.Internal
{
    internal class DkPInvoke
    {
        private const string DllFile = @"DkDll\\SSCardDriver.dll";

        [DllImport(DllFile, EntryPoint = "iReadIdentityCard", CharSet = CharSet.Ansi, SetLastError = false,
            CallingConvention = CallingConvention.StdCall)]
        public static extern int ReadIdCard(ref int fingerLength,StringBuilder fingerData,StringBuilder message);
        [DllImport(DllFile, EntryPoint = "iReadCardBas", CharSet = CharSet.Ansi, SetLastError = false,
            CallingConvention = CallingConvention.StdCall)]
        public static extern int ReadCardBas(int type,StringBuilder message);
    }

    internal class DkReaderInternal
    {
        private const int SuccessCode = 0;
        public static IMessage<IPersonInfo> ReadIdCard()
        {
            var info = new StringBuilder(1024);
            var notGetFingerDataLength = 0;
            var ret = DkPInvoke.ReadIdCard(ref notGetFingerDataLength, new StringBuilder(1024), info);
            return ret == SuccessCode
                ? CommonDeviceMsg<DkPersonInfo>.CreateSuccess(DkPersonInfo.CreateByIdResult(info.ToString()))
                : CommonDeviceMsg<DkPersonInfo>.CreateFail(info.ToString());
        }
        public static IMessage<IPersonInfo> ReadSocialCard(CardType cardType)
        {
            var info = new StringBuilder(1024);
            var ret = DkPInvoke.ReadCardBas((int)cardType, info);
            return ret == SuccessCode
                ? CommonDeviceMsg<DkPersonInfo>.CreateSuccess(DkPersonInfo.CreateBySocialResult(info.ToString()))
                : CommonDeviceMsg<DkPersonInfo>.CreateFail(info.ToString());
        }
    }

    internal class DkPersonInfo : BaseCmpPersonInfo
    {
        private static readonly Dictionary<string, Func<string[], DkPersonInfo>> GetInfoById;

        static DkPersonInfo()
        {
            GetInfoById = new Dictionary<string, Func<string[], DkPersonInfo>>
            {
                {"", arrary => new DkPersonInfo() {Name = arrary[0], IdNum = arrary[5]}},
                {"I", arrary => new DkPersonInfo() {Name = arrary[1], IdNum = arrary[5]}},
                {"J", arrary => new DkPersonInfo() {Name = arrary[0], IdNum = arrary[4]}}
            };
        }
        public static DkPersonInfo CreateByIdResult(string data)
        {
            var dataArrary = data.Trim('|').Split('|');
            return GetInfoById[dataArrary[0]](dataArrary.Skip(1).ToArray());
        }

        public static DkPersonInfo CreateBySocialResult(string data)
        {
            var dataArrary = data.Trim('|').Split('|');
            return new DkPersonInfo()
            {
                Name = dataArrary[4],
                IdNum = dataArrary[1]
            };
        }
    }
}
