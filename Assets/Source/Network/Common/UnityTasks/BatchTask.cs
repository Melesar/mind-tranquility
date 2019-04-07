namespace Common
{
    public class BatchTask : IUnityTask
    {
        private readonly IUnityTask[] _batch;

        public BatchTask(params IUnityTask[] tasks)
        {
            _batch = tasks;
        }
        
        public void Execute()
        {
            foreach (var task in _batch)
            {
                task?.Execute();
            }
        }
    }
}