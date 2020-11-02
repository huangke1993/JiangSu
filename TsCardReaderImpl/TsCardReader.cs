using ICardReaderDeclare;
using System.Threading.Tasks;
using ConmonMessage;
using ICardReaderDeclare.Enum;
using TsCardReaderImpl.Internal;

namespace TsCardReaderImpl
{
    public class TsCardReader: ICardReader
    {
        private readonly object _lock = new object();
        public Task<IMessage<IPersonInfo>> ReadIdCardAsync()
        {
            return Task.Run(
                () =>
                {
                    lock (_lock)
                    {
                        return TsReaderInternal.ReadIdCard();
                    }
                }
            );
        }

        public Task<IMessage<IPersonInfo>> ReadSocialCardAsync(CardType cardType)
        {
            return Task.Run(
                () =>
                {
                    lock (_lock)
                    {
                        return TsReaderInternal.ReadSocialCard(cardType);
                    }
                }
            );
        }
    }
}
