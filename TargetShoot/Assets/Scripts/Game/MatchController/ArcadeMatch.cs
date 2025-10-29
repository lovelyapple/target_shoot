using GameDefinition;
using UnityEngine;
using R3;
using System;

public class ArcadeMatch : IMatch
{
    #region IMatchの公開機能
    public PlayerScoreInfo PlayerScore { get; private set; }
    public TargetStackInfo TargetStackInfo { get; private set; }
    private int? _matchResult;
    public bool HasResult => _matchResult.HasValue;
    public bool HasTargetStack => TargetStackInfo != null && TargetStackInfo.CurrentPoint > 0;
    public void Init(PlayerScoreInfo playerScoreInfo, TargetStackInfo targetStackInfo)
    {
        PlayerScore = playerScoreInfo;
        TargetStackInfo = targetStackInfo;
    }
    public void OnRespawnOneTarget()
    {
        TargetStackInfo.UseOne();
        MatchEventDispatcher.Instance.StackUpdateSubject.OnNext(TargetStackInfo);
    }
    public bool CanFire()
    {
        // サバイバルモードで0になると終わり
        // if (ModelCache.Match.IsSurvieMode)
        // {
        //     return !HasResult && PlayerScore.CurrentScore > 0;
        // }

        return true;
    }
    public void OnFire()
    {
        ApplyScore(-GameConstant.FireCost);
    }
    public void OnUpdateScoreCombo(int CurrentCombo)
    {
        MatchEventDispatcher.Instance.OnScoreComboUpdateSubject.OnNext(CurrentCombo);
    }
    public void OnReceiveScoreComboPoint(int score)
    {
        ApplyScore(score);
    }
    public void ApplyScore(int score)
    {
        if (HasResult)
        {
            return;
        }

        PlayerScore.Apply(score);

        // アーケードは0以下にはならない
        // if (ModelCache.Match.IsArcadeMode)
        {
            PlayerScore.Clamp0();
        }

        var scoreInfo = new ScoreInfo()
        {
            Diff = score,
            AfterScore = PlayerScore.CurrentScore,
        };

        MatchEventDispatcher.Instance.ScoreUpdateSubject.OnNext(scoreInfo);

        // サバイバルモードで残り０なので強制終了
        // if (ModelCache.Match.IsSurvieMode && score < 0 && PlayerScore.CurrentScore <= 0)
        // {
        //     _matchEndAt = new(0);
        // }
    }
    public void SetupResult(int result)
    {
        _matchResult = result;
    }
    #endregion
}
