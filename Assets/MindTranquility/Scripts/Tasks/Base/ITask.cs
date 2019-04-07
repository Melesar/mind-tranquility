using System;
using System.Collections;

public interface ITask
{
    void Execute();

    event Action onFinish;
    bool IsExecuting { get; }
}