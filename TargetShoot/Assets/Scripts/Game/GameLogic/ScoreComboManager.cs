using UnityEngine;

public class ScoreComboManager
{
    public ScoreComboInfo ScoreCombo { get; private set; }
    public void Reset()
    {
        ScoreCombo = new ScoreComboInfo();
    }
}
