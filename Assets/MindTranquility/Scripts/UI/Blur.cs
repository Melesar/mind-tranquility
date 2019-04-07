using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Blur : MonoBehaviour
{
    [SerializeField]
    private Material _blurMaterial;
    [SerializeField]
    private RenderTexture _target;

    private Material _materialInstance;
    private float _deviationValue;

    private bool _isBlurring;
    
    public Tween ApplyBlur(float duration)
    {
        _isBlurring = true;
        SetDeviation(_materialInstance, 0f);
        return TweenDeviation(_deviationValue, duration);
    }

    public Tween DisableBlur(float duration)
    {
        _isBlurring = true;
        return TweenDeviation(0, duration);
    }
    
//    private void OnRenderImage(RenderTexture source, RenderTexture destination)
//    {
//        Graphics.Blit(source, destination);
//        
//        if (!_isBlurring)
//        {
//            return;
//        }
//        
////        draws the pixels from the source texture to the destination texture
////        var temporaryTexture = RenderTexture.GetTemporary(source.width, source.height);
////        Graphics.Blit(source, temporaryTexture, _materialInstance, 0);
////        Graphics.Blit(temporaryTexture, _target, _materialInstance, 1);
////        RenderTexture.ReleaseTemporary(temporaryTexture);
//    }

    private Tween TweenDeviation(float endValue, float duration)
    {
        return DOTween.To(() => GetDeviation(_materialInstance), f => SetDeviation(_materialInstance, f), endValue,
            duration).OnComplete(() => _isBlurring = false);
    }

    private float GetDeviation(Material material)
    {
        return material.GetFloat("_StandardDeviation");
    }

    private void SetDeviation(Material material, float value)
    {
        material.SetFloat("_StandardDeviation", value);
    }

    private void Start()
    {
        _materialInstance = Instantiate(_blurMaterial);
        _deviationValue = GetDeviation(_blurMaterial);
        SetDeviation(_materialInstance, 0f);
    }

    private void OnDestroy()
    {
        Destroy(_materialInstance);
    }
}
