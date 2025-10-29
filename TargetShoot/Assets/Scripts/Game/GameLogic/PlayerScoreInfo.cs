using GameDefinition;
using UnityEngine;
public class ScoreInfo
{
    public int AfterScore;
    public int Diff;
}
public class PlayerScoreInfo
{
    public int CurrentScore { get; private set; }
    public void Apply(int score)
    {
        CurrentScore += score;
    }
    public void Clamp0()
    {
        if (CurrentScore < 0)
        {
            CurrentScore = 0;
        }
    }
}
