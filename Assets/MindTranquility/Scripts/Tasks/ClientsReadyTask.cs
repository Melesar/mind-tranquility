using System;

public class ClientsReadyTask : ITask
{
    public void Execute()
    {
        IsExecuting = true;
    }

    public void SetReady()
    {
        IsExecuting = false;
        onFinish?.Invoke();
    }

    public event Action onFinish;
    public bool IsExecuting { get; private set; }
}