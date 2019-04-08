using UnityEngine;

public abstract class Popup : MonoBehaviour
{
    private ITask _onOpen;
    private ITask _onClose;
    
    public virtual void Init(ITask onOpen = null, ITask onClose = null)
    {
        _onOpen = onOpen;
        _onClose = onClose;
    }

    public void Close()
    {
        OnClose();
        _onClose?.Execute();
    }

    protected virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Close();
        }
    }

    protected virtual void Start()
    {
        _onOpen?.Execute();
        OnOpen();
    }

    protected virtual void OnOpen()
    {
        
    }

    protected virtual void OnClose()
    {
    }

    protected virtual void Clear()
    {
        Destroy(gameObject);
    }
}