using System;
using System.Collections.Generic;
using Jayrock.Json.Conversion.Converters;
using UnityEngine;

[CreateAssetMenu(menuName = "Meditation/Task handle")]
public class TaskHandle : ScriptableObject
{
    [SerializeField]
    private bool _clearOnFinish = true;
    
    public const int PRIORITY_NETWORK = 0;
    public const int PRIORITY_SCENE_LOADED = 10;
    public const int PRIORITY_UI_EFFECTS = 25;

    private readonly SortedDictionary<int, List<ITask>> _tasks =
        new SortedDictionary<int, List<ITask>>();

    private MonoBehaviour _holder;

    public void SetCoroutineHolder(MonoBehaviour holder)
    {
        _holder = holder;
    }

    public void AddTask(ITask task, int priority)
    {
        List<ITask> list;
        if (!_tasks.TryGetValue(priority, out list))
        {
            list = new List<ITask>();
            _tasks.Add(priority, list);
        }

        list.Add(task);
    }

    public void ExecuteTasks(Action onComplete = null)
    {
        var sequence = new TaskSequence(_holder);
        foreach (var taskList in _tasks.Values)
        {
            var paralell = new ParallelTask(_holder);
            foreach (var task in taskList)
            {
                paralell.Add(task);
            }

            sequence.Add(paralell);
        }

        if (onComplete != null)
        {
            sequence.onFinish += onComplete;
        }

        if (_clearOnFinish)
        {
            sequence.onFinish += Clear;
        }
        sequence.Execute();
    }

    public void Clear()
    {
        _tasks.Clear();
    }
}