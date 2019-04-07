using System.Runtime.InteropServices.WindowsRuntime;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CatsMeditationController : MonoBehaviour
{
    [SerializeField]
    private Image _meditationBar;
    [SerializeField]
    private Texture2D[] _cats;
    [SerializeField]
    private Material _catsMaterial;
    [SerializeField]
    private MindReaderAsset _mindReader;
    
    private float _currentMeditationValue;
    private int _currentCatIndex;
    private bool _isBlending;

    private float Blend
    {
        get { return _catsMaterial.GetFloat("_BlendParam"); }
        set { _catsMaterial.SetFloat("_BlendParam", value); }
    }

    private void Update()
    {
        if (!Mathf.Approximately(_currentMeditationValue, _mindReader.Meditation))
        {
            var meditation = _mindReader.Meditation;
            _meditationBar.DOFillAmount(meditation, 0.35f).SetEase(Ease.OutCubic);
            UpdateCatImage(meditation);
            _currentMeditationValue = meditation;
        }
    }

    private void UpdateCatImage(float meditation)
    {
        var catIndex = GetCatIndex(meditation);
        if (_currentCatIndex != catIndex)
        {
            if (!_isBlending)
            {
                _catsMaterial.SetTexture("_MainTex", _cats[_currentCatIndex]);
                _catsMaterial.SetTexture("_SecondTex", _cats[catIndex]);

                _isBlending = true;

                Blend = 0f;
                DOTween.To(() => Blend, f => Blend = f, 1f, 1.5f).OnComplete(() => _isBlending = false);
            }
            else
            {
                _catsMaterial.SetTexture("_SecondTex", _cats[catIndex]);
            }

            _currentCatIndex = catIndex;
        }
    }

    private int GetCatIndex(float meditation)
    {
        if (meditation <= 0.25f)
        {
            return 0;
        }

        if (meditation > 0.25f && meditation <= 0.75f)
        {
            return 1;
        }

        if (meditation > 0.75f)
        {
            return 2;
        }

        return 0;
    }

    private void Start()
    {
        _mindReader.Init(new TimeMindReader());
    }

    private void OnDestroy()
    {
        _mindReader.Dispose();
    }
}
