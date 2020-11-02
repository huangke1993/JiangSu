using System.Threading.Tasks;
using ConmonMessage;
using DkCardReaderImpl.Internal;
using ICardReaderDeclare;
using ICardReaderDeclare.Enum;

namespace DkCardReaderImpl
{
    public class DkCardReader : ICardReader
    {
        private readonly object _lock = new object();

        public Task<IMessage<IPersonInfo>> ReadIdCardAsync()
        {
            return Task.Run(
                () =>
                {
                    lock (_lock)
                    {
                        return DkReaderInternal.ReadIdCard();
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
                        return DkReaderInternal.ReadSocialCard(cardType);
                    }
                }
            );
        }
    }
}
