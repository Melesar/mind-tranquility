namespace Common
{
    public interface ITaskQueue
    {
        void AddTask(IUnityTask newTask);
        void ExecuteNextTask();
        void Clear();
    }
}