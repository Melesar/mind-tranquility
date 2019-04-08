using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MainMenuAnimation : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup _gameTitleGroup;
    
    [SerializeField]
    private float _gameTitleAppearDuration;
    [SerializeField]
    private float _buttonsAppearDuration;
    [SerializeField]
    private float _buttonsDelay;
    [SerializeField]
    private float _afterTitleDelay;
    
    [SerializeField]
    private Transform _firstButtonTransform;
    [SerializeField]
    private CanvasGroup _firstButtonGroup;
    [SerializeField]
    private Button _firstButton;
    
    [SerializeField]
    private Transform _secondButtonTransform;
    [SerializeField]
    private CanvasGroup _secondButtonGroup;
    [SerializeField]
    private Button _secondButton;
    
    [SerializeField]
    private CanvasGroup _globalCanvasGroup;
    [SerializeField]
    private float _globalFadeDuration;

    [FormerlySerializedAs("_connectionHandler")]
    [SerializeField]
    private ConnectionHandle _connectionHandle;

    private void InitialAnimation()
    {
        _gameTitleGroup.alpha = 0f;
        
        var buttonsScale = new Vector2(0.3f, 0.3f);
        _firstButtonTransform.localScale = buttonsScale;
        _secondButtonTransform.localScale = buttonsScale;
        _firstButtonGroup.alpha = 0f;
        _secondButtonGroup.alpha = 0f;
        _firstButton.interactable = false;
        _secondButton.interactable = false;
        
        var sequence = DOTween.Sequence();
        sequence.Append(_gameTitleGroup.DOFade(1f, _gameTitleAppearDuration));
        sequence.AppendInterval(_afterTitleDelay);
        sequence.Append(_firstButtonTransform.DOScale(1f, _buttonsAppearDuration));
        sequence.Join(_firstButtonGroup.DOFade(1f, _buttonsAppearDuration));
        var secondButtonAppearTime = _gameTitleAppearDuration + _afterTitleDelay + _buttonsDelay;
        sequence.Insert(secondButtonAppearTime, _secondButtonTransform.DOScale(1f, _buttonsAppearDuration));
        sequence.Join(_secondButtonGroup.DOFade(1f, _buttonsAppearDuration));
        sequence.OnComplete(() =>
        {
            _firstButton.interactable = true;
            _secondButton.interactable = true;
        });

        sequence.Play();
    }

    private class FadeOutTask : ITask
    {
        private readonly CanvasGroup _group;
        private readonly float _globalFadeDuration;
        private Tween _tween;

        public void Execute()
        {
            _tween = _group.DOFade(0f, _globalFadeDuration).OnComplete(() =>
            {
                _tween = null;
                onFinish?.Invoke();
            })
            .Play();
        }

        public FadeOutTask(CanvasGroup group, float globalFadeDuration)
        {
            _group = group;
            _globalFadeDuration = globalFadeDuration;
        }

        public event Action onFinish;
        public bool IsExecuting => _tween?.IsPlaying() ?? false;
    }

    private void Start()
    {
        InitialAnimation();
        _connectionHandle.AddTask(new FadeOutTask(_globalCanvasGroup, _globalFadeDuration), TaskHandle.PRIORITY_UI_EFFECTS);
    }
}