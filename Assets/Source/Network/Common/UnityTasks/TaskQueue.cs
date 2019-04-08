using System;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
    public class TaskQueue : ITaskQueue
    {
        private readonly HashSet<Type> _logIgnoreTasks = new HashSet<Type>
        {
            typeof(EmptyTask)
        };
        
        private readonly Queue<IUnityTask> _queue = new Queue<IUnityTask>();
        
        public void AddTask(IUnityTask newTask)
        {
            _queue.Enqueue(newTask);
        }

        public void ExecuteNextTask()
        {
            if (_queue.Count == 0)
            {
                return;
            }

            var task = _queue.Dequeue();
            if (task == null)
            {
                ExecuteNextTask();
                return;
            }
            
            try
            {
                task.Execute();
            }
            catch (Exception e)
            {
                Log(task, e);
            }
        }

        public void Clear()
        {
            _queue.Clear();
        }

        private void Log(IUnityTask task, Exception e = null)
        {
            var type = task.GetType();
            if (_logIgnoreTasks.Contains(type))
            {
                return;
            }

            if (e != null)
            {
                Debug.LogError($"Failed to execute {type}:\n{e}");
            }
            else
            {
                Debug.Log($"Executed task {type}");
            }
        }
    }
}