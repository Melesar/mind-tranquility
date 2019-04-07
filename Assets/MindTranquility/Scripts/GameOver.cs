using UnityEngine;

public enum Winner { Player, Opponent }

public class GameOver : MonoBehaviour
{
    [SerializeField]
    private Winner _winner;
    [SerializeField]
    private WinnerEvent _winEvent;

    private void OnCollisionEnter(Collision other)
    {
        _winEvent.Raise(_winner);
    }
}