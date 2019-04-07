using Framework.References;
using UnityEngine;

[CreateAssetMenu(menuName = "Meditation/Synchronization data")]
public class SynchronizationData : ScriptableObject
{
    public StringReference myName;
    public StringReference opponentName;

    public FloatReference myMeditation;
    public FloatReference opponentMeditation;

    public void Clear()
    {
        myName.Value = "";
        opponentName.Value = "";
        myMeditation.Value = 0;
        opponentMeditation.Value = 0;
    }
}