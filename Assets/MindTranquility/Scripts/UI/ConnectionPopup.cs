using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class ConnectionPopup : Popup
{
    [SerializeField]
    private Animation _animation;
    [SerializeField]
    private TMP_InputField _nameInput;
    [FormerlySerializedAs("_connectionHandler")]
    [SerializeField]
    private ConnectionHandle _connectionHandle;
    [SerializeField]
    private MindReaderAsset _mindReader;

    [SerializeField]
    private GameObject _connectionPanel;
    [SerializeField]
    private GameObject _noConnectionPanel;
    [SerializeField]
    private GameObject _waitingPanel;
    [SerializeField]
    private GameObject _mindReaderPanel;

    [SerializeField]
    private InputValidationAnimation _validationAnimation;

    [SerializeField]
    private bool _checkForProblems;

    private bool IsMindReaderAvailable => _mindReader.SignalStrength != SignalStrength.NoSignal;
    private bool IsNetworkAvailable => Application.internetReachability != NetworkReachability.NotReachable;
    
    private bool _isHost;
    private bool _isConnected;
    private bool _isProblem;

    private string PlayerName
    {
        get { return _nameInput.text; }
        set { _nameInput.text = value; }
    }
    
    private const string NAME_PREFS = "player_name";
    private const string DISAPPEAR_ANIMATION = "popup_disappear";

    public void SetHost()
    {
        _isHost = true;
    }

    public void SetClient()
    {
        _isHost = false;
    }

    public void OnOkPressed()
    {
        if (_isConnected)
        {
            return;
        }
        
        if (ValidateData())
        {
            _connectionPanel.SetActive(false);
            _waitingPanel.SetActive(true);
            OnReadyToConnect();
        }
        else
        {
            _validationAnimation.AnimateValidationError();
        }
    }

    public void OnCancelPressed()
    {
        if (_isConnected)
        {
            return;
        }
        
        Close();
        _connectionHandle.AbortConnection();
    }

    private bool ValidateData()
    {
        return !string.IsNullOrEmpty(PlayerName);
    }

    private void OnReadyToConnect()
    {
        PlayerPrefs.SetString(NAME_PREFS, PlayerName);
        PlayerPrefs.Save();
        
        _connectionHandle.IsHost = _isHost;
        _connectionHandle.PlayerName = PlayerName;
        
        var closeTask = CreateCloseTask();
        _connectionHandle.AddTask(closeTask, TaskHandle.PRIORITY_UI_EFFECTS);
        _connectionHandle.ReadyToConnect();
    }

    protected override void Update()
    {
        base.Update();

        if (!_checkForProblems || _isConnected || _isProblem) 
        {
            return;
        }

        if (!IsNetworkAvailable)
        {
            SetNoConnectionState();
        }
        else if (!IsMindReaderAvailable)
        {
            SetMindReaderState();
        }
    }

    protected override void OnOpen()
    {
        base.OnOpen();
        SetConnectionState();
    }

    protected override void OnClose()
    {
        base.OnClose();
        CreateCloseTask().Execute();
    }

    private ITask CreateCloseTask()
    {
        var closeTask = new TaskSequence(this);
        closeTask.Add(new DisappearAnimationTask(this, _animation, DISAPPEAR_ANIMATION));
        closeTask.Add(new ActionTask(() =>
        {
            Clear();
            _isConnected = true;
        }));
        return closeTask;
    }

    private void SetConnectionState()
    {
        _connectionPanel.SetActive(true);
        _noConnectionPanel.SetActive(false);
        _mindReaderPanel.SetActive(false);
        
        PlayerName = PlayerPrefs.HasKey(NAME_PREFS) ? PlayerPrefs.GetString(NAME_PREFS) : string.Empty;
    }

    private void SetNoConnectionState()
    {
        _connectionPanel.SetActive(false);
        _mindReaderPanel.SetActive(false);
        _noConnectionPanel.SetActive(true);
        
        StartCoroutine(ProblemCoroutine(() => IsNetworkAvailable));
    }

    private void SetMindReaderState()
    {
        _mindReaderPanel.SetActive(true);
        _connectionPanel.SetActive(false);
        _noConnectionPanel.SetActive(false);

        StartCoroutine(ProblemCoroutine(() => IsMindReaderAvailable));
    }

    private IEnumerator ProblemCoroutine(Func<bool> waitUntil)
    {
        _isProblem = true;
        yield return new WaitUntil(waitUntil);
        _isProblem = false;
        
        SetConnectionState();
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