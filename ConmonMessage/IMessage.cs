namespace ConmonMessage
{
    public interface IMessage
    {
        string Message { get; }
        bool IsSuccess { get; }
    }
   
    public interface IMessage<out T> : IMessage
    {
        T Data { get; }
    }
}
