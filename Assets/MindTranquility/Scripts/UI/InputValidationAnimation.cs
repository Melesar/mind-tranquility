using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InputValidationAnimation : MonoBehaviour
{
    [SerializeField]
    private float _duration;
    [SerializeField]
    private AnimationCurve _alphaCurve;
    [SerializeField]
    private Image _image;
    
    private float _currentTime;

    private Coroutine _animationCoroutine;
    
    public void AnimateValidationError()
    {
        if (_animationCoroutine != null)
        {
            StopCoroutine(_animationCoroutine);
        }

        _animationCoroutine = StartCoroutine(AnimationRoutine());
    }
    
    private IEnumerator AnimationRoutine()
    {
        _currentTime = 0f;
        
        while (_currentTime < _duration)
        {
            var t = _currentTime / _duration;
            var color = _image.color;
            color.a = _alphaCurve.Evaluate(t);
            _image.color = color;

            _currentTime += Time.deltaTime;

            yield return null;
        }
    }
}