using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BallController : MonoBehaviour
{
    [SerializeField]
    private SynchronizationData _synchronizationData;
    [SerializeField]
    private float _maxForce;
    [SerializeField]
    private TaskHandle _gameLoadedHandle;
    
    private Rigidbody _rigidbody;
    private bool _isMoving;
    
    public void OnGameOver(Winner winner)
    {
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.isKinematic = true;
    }

    private void FixedUpdate()
    {
        if (!_isMoving)
        {
            return;
        }
        
        var myMeditation = _synchronizationData.myMeditation.Value;
        var opponentMeditation = _synchronizationData.opponentMeditation.Value;
        var myForce = myMeditation * _maxForce * Vector3.forward;
        var opponentForce = -opponentMeditation * _maxForce * Vector3.forward;

        _rigidbody.AddForce(myForce + opponentForce);
        _rigidbody.velocity = Vector3.ClampMagnitude(_rigidbody.velocity, _maxForce);
    }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _gameLoadedHandle.AddTask(new ActionTask(() => _isMoving = true), TaskHandle.PRIORITY_UI_EFFECTS + 5);
    }
} 