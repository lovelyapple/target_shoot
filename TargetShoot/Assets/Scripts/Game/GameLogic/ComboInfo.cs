using UnityEngine;

public class ComboInfo
{
    public int CurrentCombo;
    public void AddCombo()
    {
        CurrentCombo++;
    } 
    public void Reset()
    {
        CurrentCombo = 0;
    }
}
