using UnityEngine;

public interface IMatch
{
    public bool HasTargetStack { get; }
    public bool HasResult { get; }
    public bool CanFire();
    public void OnFire();
    public void OnRespawnOneTarget();
    public void OnUpdateScoreCombo(int CurrentCombo);
    public void OnReceiveScoreComboPoint(int score);
    public void ApplyScore(int score);
}