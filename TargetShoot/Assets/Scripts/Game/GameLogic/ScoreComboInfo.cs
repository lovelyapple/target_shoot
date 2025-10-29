using System;
using UnityEngine;

public class ScoreComboInfo
{
    public int CurrentCombo { get; private set; }

    public void OnReset()
    {
        CurrentCombo = 0;
    }
    public void OnAddOne()
    {
        CurrentCombo += 1;
    }
    public void OnAddTwo()
    {
        CurrentCombo += 2;
    }
    public int GetScoreOnTimeOut()
    {
        return CurrentCombo * 5;
    }
}
