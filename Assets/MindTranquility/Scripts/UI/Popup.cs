using System.Collections;
using UnityEngine;

public abstract class Popup : MonoBehaviour
{
    private ParallelTask _onOpen;
    private ParallelTask _onClose;

    [SerializeField]
    private TaskHandle _blurTask;
    [SerializeField]
    private Animation _animation;
    
    private const string DISAPPEAR_ANIMATION = "popup_disappear";

    protected ITask onOpen => _onOpen;
    protected ITask onClose => _onClose;
    
    public void Close()
    {
        _onClose?.Execute();
    }

    public void AddOnOpen(ITask task)
    {
        _onOpen.Add(task);
    }

    public void AddOnClose(ITask task)
    {
        _onClose.Add(task);
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
    }

    protected virtual void Awake()
    {
        _onOpen = new ParallelTask(this);
        _onClose = new ParallelTask(this);

        AddOnOpen(new ActionTask(OnOpen));
        AddOnOpen(_blurTask.ToTask());
        
        AddOnClose(new ActionTask(OnClose));
        AddOnClose(DisappearAnimation().DoInSequenceWith(this, new ActionTask(Clear)));
    }

    protected virtual void OnOpen()
    {
        
    }

    protected virtual void OnClose()
    {
    }

    protected virtual ITask DisappearAnimation()
    {
        return new DisappearAnimationTask(this, _animation, DISAPPEAR_ANIMATION);
    }

    protected virtual void Clear()
    {
        Destroy(gameObject);
    }
    
    private class DisappearAnimationTask : Task
    {
        private readonly Animation _animation;
        private readonly string _animationName;

        public DisappearAnimationTask(MonoBehaviour holder, Animation animation, string animationName) : base(holder)
        {
            _animation = animation;
            _animationName = animationName;
        }

        protected override IEnumerator ExecuteCoroutine()
        {
            _animation.Play(_animationName);
            var clip = _animation.GetClip(_animationName);

            yield return new WaitForSeconds(clip.length);
        }
    }
}