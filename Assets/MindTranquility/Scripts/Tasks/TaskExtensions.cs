using UnityEngine;

public static class TaskExtensions 
{
    public static ITask DoInParallelWith(this ITask original, MonoBehaviour holder, params ITask[] tasks)
    {
        var result = new ParallelTask(holder);
        AddTo(result, original, tasks);

        return result;
    }

    public static ITask DoInSequenceWith(this ITask original, MonoBehaviour holder, params ITask[] tasks)
    {
        var result = new TaskSequence(holder);
        AddTo(result, original, tasks);

        return result;
    }

    private static void AddTo(ITaskContainer container, ITask original, ITask[] tasks)
    {
        container.Add(original);
        foreach (var task in tasks)
        {
            container.Add(task);
        }
    }
}