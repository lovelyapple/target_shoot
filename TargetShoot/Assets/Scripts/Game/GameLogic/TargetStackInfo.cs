using UnityEngine;

public class TargetStackInfo
{
    public int CurrentPoint { get; private set; }
    public void AddPoint(int point)
    {
        CurrentPoint += point;
    }
    public void UseOne()
    {
        CurrentPoint--;

        if(CurrentPoint <= 0)
        {
            CurrentPoint = 0;
        }
    }
}
