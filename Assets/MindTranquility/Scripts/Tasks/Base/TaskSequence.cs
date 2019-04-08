using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskSequence : Task
{
    private readonly List<ITask> _tasks;

    public TaskSequence(MonoBehaviour coroutineHolder, params ITask[] tasks) : base(coroutineHolder)
    {
        _tasks = new List<ITask>(tasks);
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
            if (task.IsExecuting)
            {
                yield return new WaitUntil(() => !task.IsExecuting);
            }
        }
    }
}