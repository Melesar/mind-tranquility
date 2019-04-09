using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallelTask : Task, ITaskContainer
{
    private readonly List<ITask> _tasks;

    public ParallelTask(MonoBehaviour coroutineHolder, params ITask[] tasks) : base(coroutineHolder)
    {
        _tasks = new List<ITask>(tasks);
    }

    public ParallelTask(MonoBehaviour coroutineHolder) : base(coroutineHolder)
    {
        _tasks = new List<ITask>();
    }

    public void Add(ITask task)
    {
        _tasks.Add(task);
    }

    protected override IEnumerator ExecuteCoroutine()
    {
        foreach (var task in _tasks)
        {
            task.Execute();
        }

        yield return new WaitUntil(AreTaskCompleted);
    }

    private bool AreTaskCompleted()
    {
        foreach (var task in _tasks)
        {
            if (task.IsExecuting)
            {
                return false;
            }
        }

        return true;
    }
}