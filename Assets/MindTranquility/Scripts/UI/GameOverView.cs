using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameOverView : MonoBehaviour
{
    [SerializeField]
    private RawImage _background;
    [SerializeField]
    private CanvasGroup _playerWin;
    [SerializeField]
    private CanvasGroup _playerLose;

    public void OnGameOver(Winner winner)
    {
        switch (winner)
        {
            case Winner.Player:
                Animate(_playerWin);
                break;
            case Winner.Opponent:
                Animate(_playerLose);
                break;
        }
    }

    private void Animate(CanvasGroup canvasGroup)
    {
        _background.raycastTarget = true;

        var tr = canvasGroup.transform;
        
        canvasGroup.gameObject.SetActive(true);
        canvasGroup.alpha = 0f;
        tr.localScale = new Vector3(0.3f, 0.2f, 1f);

        var sequence = DOTween.Sequence();
        sequence.Append(tr.DOScale(1f, 0.8f));
        sequence.Join(canvasGroup.DOFade(1f, 1f));
        sequence.Play();
    }
}