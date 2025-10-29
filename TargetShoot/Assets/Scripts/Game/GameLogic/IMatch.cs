using UnityEngine;

public interface IMatch
{
    public bool HasTargetStack { get; }
    public bool HasResult { get; }
    public void Init(PlayerScoreInfo playerScoreInfo,
    TargetStackInfo targetStackInfo,
    ScoreComboManager scoreComboManager);
    public bool CanFire();
    public void OnFire();
    public void OnBulletHitTarget(ITarget target);
    public void OnCatchFallTarget(ITarget iTarget);
    public void OnRespawnOneTarget();
    public void OnReceiveScoreComboPoint(int score);
    public void ApplyScore(int score);
    public void SetupResult(int result);
}