using GameDefinition;


public class ArcadeMatch : IMatch
{
    public PlayerScoreInfo PlayerScore { get; private set; }
    public TargetStackInfo TargetStackInfo { get; private set; }
    public ScoreComboManager ScoreComboManager { get; private set; }
    private int? _matchResult;
    public bool HasResult => _matchResult.HasValue;
    public bool HasTargetStack => TargetStackInfo != null && TargetStackInfo.CurrentPoint > 0;
    public void Init(PlayerScoreInfo playerScoreInfo,
        TargetStackInfo targetStackInfo,
        ScoreComboManager scoreComboManager)
    {
        PlayerScore = playerScoreInfo;
        TargetStackInfo = targetStackInfo;
        ScoreComboManager = scoreComboManager;
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
        // アーケードモードでは発砲は消費しない
        //ApplyScore(-GameConstant.FireCost);
    }
    public void OnBulletHitTarget(ITarget iTarget)
    {
        var comboBonus = GameConstant.GetComboTimes(iTarget.HitCombo);
        var applyScore = (int)(iTarget.Score * comboBonus);
        ApplyScore(applyScore);
        ScoreComboManager.AddComobo(iTarget.HitCombo);
    }
    public void OnCatchFallTarget(ITarget iTarget)
    {
        TargetStackInfo.AddPoint(iTarget.CatchStackCount);
        MatchEventDispatcher.Instance.StackUpdateSubject.OnNext(TargetStackInfo);

        if (iTarget.IsFrameTarget)
            ScoreComboManager.AddComobo(GameConstant.ScoreComboOnCatchTargetStep);
    }
    public void OnReceiveScoreComboPoint(int score)
    {
        // アーケードモードではスコアコンボがTimeOutする際のボーナスはない
        // ApplyScore(score);
    }
    public void ApplyScore(int score)
    {
        if (HasResult)
        {
            return;
        }

        PlayerScore.Apply(score);

        // アーケードは0以下にはならない
        PlayerScore.Clamp0();

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
}
