using System.Threading.Tasks;
using ConmonMessage;
using DeviceMessage;
using HxCardReaderImpl.Internal;
using ICardReaderDeclare;
using ICardReaderDeclare.Enum;

namespace HxCardReaderImpl
{
    public class HxCardReader : ICardReader
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
            return HxReaderInternal.ReadCard(_port);
        }

        private void CloseReader()
        {
            HxReaderInternal.CloseReader(_port);
        }

        private int _port;

        private IMessage OpenReader()
        {
            var comOpenResult = OpenReaderByUsb();
            if (comOpenResult.IsSuccess) return comOpenResult;
            return OpenReaderByCom();
        }

        private const int MinComNum = 1;
        private const int MaxComNum = 16;

        private IMessage OpenReaderByCom()
        {
            for (var port = MinComNum; port < MaxComNum; port++)
            {
                var result = HxReaderInternal.OpenReader(port);
                if (result.IsSuccess)
                {
                    _port = port;
                    return result;
                }
            }

            return HxReaderInternal.OpenReader(MaxComNum);
        }

        private const int MinUsbNum = 1001;
        private const int MaxUsbNum = 1016;

        private IMessage OpenReaderByUsb()
        {
            for (var usb = MinUsbNum; usb < MaxUsbNum; usb++)
            {
                var result = HxReaderInternal.OpenReader(usb);
                if (result.IsSuccess)
                {
                    _port = usb;
                    return result;
                }
            }

            return HxReaderInternal.OpenReader(MaxUsbNum);
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
}
