using ICardReaderDeclare;
using System.Threading.Tasks;
using ConmonMessage;
using DeviceMessage;
using HsCardReaderImpl.Internal;
using ICardReaderDeclare.Enum;

namespace HsCardReaderImpl
{
    public class HsCardReader : ICardReader
    {
        private readonly object _lock = new object();
        public Task<IMessage<IPersonInfo>> ReadIdCardAsync()
        {
            return Task.Run(
                () =>
                {
                    lock (_lock)
                    {
                        var openResult = OpenReader();
                        if (!openResult.IsSuccess) return openResult.FailToMessageT<IPersonInfo>();
                        var readResult = ReadCard();
                        CloseReader();
                        return readResult;
                    }
                }
            );
        }

        private IMessage<IPersonInfo> ReadCard()
        {
            return HsReaderInternal.ReadCard();
        }

        private void CloseReader()
        {
            HsReaderInternal.CloseReader();
        }

        private IMessage OpenReader()
        {
            var comOpenResult = OpenReaderByCom();
            if (comOpenResult.IsSuccess) return comOpenResult;
            return OpenReaderByUsb();
        }

        private const int MinComNum = 1001;
        private const int MaxComNum = 1016;
        private IMessage OpenReaderByCom()
        {
            for (var port = MinComNum; port < MaxComNum; port++)
            {
                var result = HsReaderInternal.OpenReader(port);
                if (result.IsSuccess) break;
            }
            return HsReaderInternal.OpenReader(MaxComNum);
        }
        private const int MinUsbNum = 1;
        private const int MaxUsbNum = 4;
        private IMessage OpenReaderByUsb()
        {
            for (var usb = MinUsbNum; usb < MaxUsbNum; usb++)
            {
                var result = HsReaderInternal.OpenReader(usb);
                if (result.IsSuccess) break;
            }
            return HsReaderInternal.OpenReader(MaxUsbNum);
        }
        public Task<IMessage<IPersonInfo>> ReadSocialCardAsync(CardType cardType)
        {
            return Task.Run(
                () =>
                {
                    lock (_lock)
                    {
                        return CommonDeviceMsg<IPersonInfo>.CreateFail("该设备不支持读社保卡！");
                    }
                }
            );
        }
    }

    internal static class MessageExtent
    {
        public static IMessage<T> FailToMessageT<T>(this IMessage message)
        {
            return CommonDeviceMsg<T>.CreateFail(message.Message);
        }
    }
}
