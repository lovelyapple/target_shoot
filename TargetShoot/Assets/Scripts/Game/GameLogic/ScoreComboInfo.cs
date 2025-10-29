using System;
using UnityEngine;

public class ScoreComboInfo
{
    public int CurrentCombo { get; private set; }
    public DateTime ComboStartTime { get; private set; }
    public void OnReset()
    {
        CurrentCombo = 0;
    }
    public void OnAddOne()
    {
    }
    public void OnAddTwo()
    {
    }
}
