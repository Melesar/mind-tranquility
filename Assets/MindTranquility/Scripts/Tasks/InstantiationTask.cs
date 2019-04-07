using System.Collections;
using UnityEngine;

public class InstantiationTask : Task
{
    private readonly GameObject _prefab;
    private readonly Transform _parent;

    public InstantiationTask(MonoBehaviour coroutineHolder, GameObject prefab, Transform parent = null) : base(coroutineHolder)
    {
        _prefab = prefab;
        _parent = parent;
    }

    protected override IEnumerator ExecuteCoroutine()
    {
        Object.Instantiate(_prefab, _parent);
        yield break;
    }
}