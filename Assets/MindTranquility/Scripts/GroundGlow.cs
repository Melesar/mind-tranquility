using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class GroundGlow : MonoBehaviour
{
    [SerializeField]
    private Material _glowMaterial;
    [SerializeField]
    private Color _glowFilter;
    [SerializeField]
    private Transform _destination;

    private Transform _playerTransform;
    private float _maxDistanceSqr;

    private void Update()
    {
        if (_playerTransform == null)
        {
            return;
        }

        var currentDistanceSqr = Vector3.SqrMagnitude(_playerTransform.position - _destination.position);
        var ratio = 1f - currentDistanceSqr / _maxDistanceSqr;
        var filter = Color.Lerp(Color.clear, _glowFilter, ratio);

        _glowMaterial.SetColor("_GlowFilter", filter);
    }

    private void Awake()
    {
        var collider = GetComponent<BoxCollider>();
        _maxDistanceSqr = collider.size.z * collider.size.z;
    }

    private void OnTriggerEnter(Collider other)
    {
        _playerTransform = other.transform;
    }

    private void OnTriggerExit(Collider other)
    {
        _playerTransform = null;
    }

    private void OnDestroy()
    {
        _glowMaterial.SetColor("_GlowFilter", Color.clear);
    }
}