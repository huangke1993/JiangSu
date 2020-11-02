using System;
using System.Threading.Tasks;
using ConmonMessage;
using ICardReaderDeclare.Enum;

namespace ICardReaderDeclare
{
    public interface ICardReader
    {
        Task<IMessage<IPersonInfo>> ReadIdCardAsync();
        Task<IMessage<IPersonInfo>> ReadSocialCardAsync(CardType cardType);
    }

    public interface IPersonInfo
    {
        string Name { get;}
        string IdNum { get;}
    }

    public abstract class BaseCmpPersonInfo : IPersonInfo, IEquatable<BaseCmpPersonInfo>
    {

        public string Name { get; protected set; }
        public string IdNum { get; protected set; }

        public bool Equals(BaseCmpPersonInfo other)
        {
            if (other == null) return false;
            return Name == other.Name && IdNum == other.IdNum;
        }
        public override bool Equals(object obj)
        {
            return Equals(obj as BaseCmpPersonInfo);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
        public override string ToString()
        {
            return $"{Name}|{IdNum}";
        }
    }
}
