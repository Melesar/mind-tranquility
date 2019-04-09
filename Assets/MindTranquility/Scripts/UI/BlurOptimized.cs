using System;
using UnityEngine;
using UnityEngine.Serialization;

public class BlurOptimized : MonoBehaviour
{
    [SerializeField, Range(0, 2)]
    private int _downsample = 1;
    [SerializeField, Range(0.0f, 10.0f)]
    private float _blurSize = 3.0f;
    [SerializeField, Range(1, 4)]
    private int _blurIterations = 2;
    [SerializeField]
    private RenderTexture _blurTexture;
    [FormerlySerializedAs("blurShader")]
    [SerializeField]
    private Shader _blurShader;
    [SerializeField]
    private TaskHandle _taskHandle;

    private Camera _camera;
    private Material _blurMaterial;
    private RenderTexture _savedRenderTarget;

    public void SwitchTarget()
    {
        _camera.targetTexture = _blurTexture;
        _savedRenderTarget = RenderTexture.active;
        RenderTexture.active = _blurTexture;
    }

    public void RestoreTarget()
    {
        _camera.targetTexture = null;
        RenderTexture.active = _savedRenderTarget;
        _savedRenderTarget = null;
    }
    
    private void Awake()
    {
        _blurMaterial = new Material(_blurShader);
        _camera = GetComponent<Camera>();

        _taskHandle.SetCoroutineHolder(this);
        _taskHandle.AddTask(new BlurTask(this, this), 0);
    }

    private void Start()
    {
        enabled = false;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        var widthMod = 1.0f / (1.0f * (1 << _downsample));

        _blurMaterial.SetVector("_Parameter", new Vector4(_blurSize * widthMod, -_blurSize * widthMod, 0.0f, 0.0f));
        source.filterMode = FilterMode.Bilinear;

        var rtW = source.width >> _downsample;
        var rtH = source.height >> _downsample;

        var rt = RenderTexture.GetTemporary(rtW, rtH, 0, source.format);

        rt.filterMode = FilterMode.Bilinear;
        rt.MarkRestoreExpected();
        Graphics.Blit(source, rt, _blurMaterial, 0);

        for (var i = 0; i < _blurIterations; i++)
        {
            var iterationOffs = (i * 1.0f);
            _blurMaterial.SetVector("_Parameter",
                new Vector4(_blurSize * widthMod + iterationOffs, -_blurSize * widthMod - iterationOffs, 0.0f, 0.0f));

            // vertical blur
            var rt2 = RenderTexture.GetTemporary(rtW, rtH, 0, source.format);
            rt2.filterMode = FilterMode.Bilinear;
            Graphics.Blit(rt, rt2, _blurMaterial, 1);
            RenderTexture.ReleaseTemporary(rt);
            rt = rt2;

            // horizontal blur
            rt2 = RenderTexture.GetTemporary(rtW, rtH, 0, source.format);
            rt2.filterMode = FilterMode.Bilinear;
            Graphics.Blit(rt, rt2, _blurMaterial, 2);
            RenderTexture.ReleaseTemporary(rt);
            rt = rt2;
        }

        Graphics.Blit(rt, destination);

        RenderTexture.ReleaseTemporary(rt);
    }

    private void OnDestroy()
    {
        if (_blurMaterial)
        {
            DestroyImmediate(_blurMaterial);
        }
    }
}