using System;
using System.Text;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CountdownView : MonoBehaviour
{
    [SerializeField]
    private TaskHandle _taskHandle;

    [SerializeField]
    private GameObject[] _numberHolders;
    [SerializeField]
    private Graphic _goText;
    [SerializeField]
    private float _dotsDelay = 0.25f;
    [SerializeField]
    private float _goDuration = 0.5f;
    
    
    private Sequence CreateAnimation()
    {
        var sequence = DOTween.Sequence();

        var number = _numberHolders.Length;
        foreach (var numberHolder in _numberHolders)
        {
            sequence.AppendCallback(() => numberHolder.SetActive(true));
            var text = numberHolder.GetComponentInChildren<TMP_Text>();
            sequence.Append(AnimateNumber(text, number--));
            sequence.AppendInterval(_dotsDelay);
            sequence.AppendCallback(() => numberHolder.SetActive(false));
        }
        sequence.Append(AnimateGo());

        return sequence;
    }

    private Sequence AnimateNumber(TMP_Text numberText, int numberVale)
    {
        var sequence = DOTween.Sequence();

        var sb =  new StringBuilder(numberVale.ToString());
        var text = sb.ToString();
        sequence.AppendCallback(() => numberText.text = text);
        sequence.AppendInterval(_dotsDelay);
        for (int i = 0; i < 3; i++)
        {
            sb.Append(".");
            var innerText = sb.ToString();
            sequence.AppendCallback(() => numberText.text = innerText);
            sequence.AppendInterval(_dotsDelay);
        }
        
        return sequence;
    }

    private Sequence AnimateGo()
    {
        var sequence = DOTween.Sequence();
        sequence.AppendCallback(() => _goText.gameObject.SetActive(true));
        sequence.Append(_goText.transform.DOScale(1f, _goDuration));
        sequence.AppendInterval(0.5f);
        sequence.Append(_goText.DOFade(0f, _goDuration));
        sequence.AppendCallback(() => _goText.gameObject.SetActive(false));
        
        return sequence;
    }

    [ContextMenu("Test animation")]
    private void TestAnimation()
    {
        CreateAnimation().Play();
    }

    private void Awake()
    {
        var sequence = CreateAnimation();
        var task = new AnimationTask(sequence);
        
        _taskHandle.AddTask(task, TaskHandle.PRIORITY_SCENE_LOADED);
    }
    
    private class AnimationTask : ITask
    {
        private readonly Sequence _sequence;
        
        public void Execute()
        {
            _sequence.OnComplete(() => onFinish?.Invoke());
            _sequence.Play();
        }

        public AnimationTask(Sequence sequence)
        {
            _sequence = sequence;
        }

        public event Action onFinish;
        public bool IsExecuting => _sequence.IsPlaying();
    }
}