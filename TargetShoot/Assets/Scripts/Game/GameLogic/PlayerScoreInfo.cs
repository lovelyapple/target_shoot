using GameDefinition;
using UnityEngine;

public class PlayerScoreInfo
{
    public int CurrentScore { get; private set; }
    public void Apply(int score)
    {
        CurrentScore += score;
    }
    public void Reset()
    {
        CurrentScore = 0;
    }
}
