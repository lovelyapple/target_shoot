using UnityEngine;

public class BulletComboInfo
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
