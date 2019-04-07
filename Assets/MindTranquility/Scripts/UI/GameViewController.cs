using TMPro;
using UnityEngine;

public class GameViewController : MonoBehaviour
{
    [SerializeField]
    private SynchronizationData _synchronizationData;

    [SerializeField]
    private TMP_Text _opponentName;
    [SerializeField]
    private TMP_Text _playerName;

    [SerializeField]
    private TMP_Text _opponentMeditation;
    [SerializeField]
    private TMP_Text _playerMeditation;    

    private void UpdatePlayerName(string oldName, string newName)
    {
        _playerName.text = newName;
    }

    private void UpdatePlayerName()
    {
        _playerName.text = _synchronizationData.myName;
    }

    private void UpdatePlayerMeditation(float oldValue, float newValue)
    {
        _playerMeditation.text = FormatMeditation(newValue);
    }
    
    private void UpdatePlayerMeditation()
    {
        _playerMeditation.text = FormatMeditation(_synchronizationData.myMeditation.Value);
    }
    
    private void UpdateOpponentName(string oldName, string newName)
    {
        _opponentName.text = newName;
    }

    private void UpdateOpponentName()
    {
        _opponentName.text = _synchronizationData.opponentName;
    }
    
    private void UpdateOpponentMeditation(float oldValue, float newValue)
    {
        _opponentMeditation.text = FormatMeditation(newValue);
    }
    
    private void UpdateOpponentMeditation()
    {
        _opponentMeditation.text = FormatMeditation(_synchronizationData.opponentMeditation.Value);
    }

    private string FormatMeditation(float meditationValue)
    {
        var v = (int) (meditationValue * 100f);
        return $"{v}%";
    }

    private void Start()
    {
        _synchronizationData.myName.valueChanged += UpdatePlayerName;
        _synchronizationData.opponentName.valueChanged += UpdateOpponentName;
        _synchronizationData.myMeditation.valueChanged += UpdatePlayerMeditation;
        _synchronizationData.opponentMeditation.valueChanged += UpdateOpponentMeditation;
        
        UpdateOpponentName();
        UpdatePlayerName();
        UpdatePlayerMeditation();
        UpdateOpponentMeditation();
    }

    private void OnDestroy()
    {
        _synchronizationData.myName.valueChanged -= UpdatePlayerName;
        _synchronizationData.opponentName.valueChanged -= UpdateOpponentName;
        _synchronizationData.myMeditation.valueChanged -= UpdatePlayerMeditation;
        _synchronizationData.opponentMeditation.valueChanged -= UpdateOpponentMeditation;
    }
}