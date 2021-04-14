using ConmonMessage;

namespace DeviceMessage
{
    public class CommonDeviceMsg : IMessage
    {
        internal CommonDeviceMsg(bool isSuccess,string message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }
        public static IMessage CreateSuccess()
        {
            return new CommonDeviceMsg(true,string.Empty);
        }
        public static IMessage CreateFail(string errorMessage)
        {
            return new CommonDeviceMsg(false, errorMessage);
        }

        public string Message { get; }

        public bool IsSuccess { get; }
    }

    public class CommonDeviceMsg<T> : CommonDeviceMsg, IMessage<T>
    {
        private CommonDeviceMsg(bool isSuccess, T data, string message)
            : base(isSuccess, message)
        {
            Data = data;
        }

        public static IMessage<T> CreateSuccess(T data)
        {
            return new CommonDeviceMsg<T>(true, data, string.Empty);
        }

        public new static IMessage<T> CreateFail(string errorMessage)
        {
            return new CommonDeviceMsg<T>(false, default, errorMessage);
        }
        public T Data { get; }
    }
    public static class MessageExtent
    {
        public static IMessage<T> FailToMessageT<T>(this IMessage message)
        {
            return CommonDeviceMsg<T>.CreateFail(message.Message);
        }
    }
}
