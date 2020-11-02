using System;

namespace ParameterDeclare
{
    public interface IParameterPass
    {
        void AddOrUpdateStepData(int key, object value);
        void AddObserveByKey(int key, Action<int> observeAction);
        T GetDataByKey<T>(int key);
    }
}
