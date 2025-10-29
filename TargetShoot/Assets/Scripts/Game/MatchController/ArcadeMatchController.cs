using GameDefinition;
using UnityEngine;
using R3;
using System;

public class ArcadeMatchController : MonoBehaviour, IMatch
{
    [SerializeField] FieldController Field;
    [SerializeField] PlayerController Player;

    public PlayerScoreInfo PlayerScore { get; private set; }
    public TargetStackInfo TargetStackInfo { get; private set; }
    public ScoreComboManager ScoreComboManager { get; private set; }

    private IDisposable _countdownSubscription;
    private DateTime _matchEndAt;
    private long? _matchResult = null;

    private void Awake()
    {
        PlayerScore = new PlayerScoreInfo();
        PlayerScore.Apply(GameConstant.DefaultScore);
        TargetStackInfo = new TargetStackInfo();

        MatchEventDispatcher.Instance.OnDispatchBulletHitObservable()
        .Where(_ => !HasResult)
        .Subscribe(target => OnBulletHitTarget(target))
        .AddTo(this);

        MatchEventDispatcher.Instance.OnDispatchCatchTargetObservable()
        .Where(_ => !HasResult)
        .Subscribe(target => OnCatchFallTarget(target))
        .AddTo(this);

        MatchEventDispatcher.Instance.OnBulletMissedAllObservable()
        .Where(_ => !HasResult)
        .Subscribe(_ => ScoreComboManager?.OnBulletMissedAll())
        .AddTo(this);

        Field.Initialize(this);
        Player.Initialize(this);
        ScoreComboManager = new ScoreComboManager(this);
    }
    private void OnDestroy()
    {
        ScoreComboManager?.FinishCountDown();

        _countdownSubscription?.Dispose();
        _countdownSubscription = null;
    }
    public void Start()
    {
        ApplyScore(0);
        MatchEventDispatcher.Instance.StackUpdateSubject.OnNext(TargetStackInfo);

        _matchResult = null;
        ModelCache.Match.OnMatchStart();
        StartCountDown();
    }
    public void Update()
    {
        if (!HasResult)
            Field.OnUpdate();
    }
    private void OnBulletHitTarget(ITarget iTarget)
    {
        var comboBonus = GameConstant.GetComboTimes(iTarget.HitCombo);
        var applyScore = (int)(iTarget.Score * comboBonus);
        ApplyScore(applyScore);
        ScoreComboManager.AddComobo(iTarget.HitCombo);
    }
    private void OnCatchFallTarget(TargetBase targetBase)
    {
        TargetStackInfo.AddPoint(targetBase.CatchStackCount);
        MatchEventDispatcher.Instance.StackUpdateSubject.OnNext(TargetStackInfo);

        if (targetBase.IsFrameTarget)
            ScoreComboManager.AddComobo(GameConstant.ScoreComboOnCatchTargetStep);
    }
    private void StartCountDown()
    {
        var endTime = DateTime.UtcNow.AddSeconds(GameConstant.MatchTime);
        double unixTimeMs = new DateTimeOffset(endTime).ToUnixTimeMilliseconds();
        _matchEndAt = DateTimeOffset.FromUnixTimeMilliseconds((long)unixTimeMs).UtcDateTime;

        MatchEventDispatcher.Instance.OnUpdateTimeLeftSubject.OnNext(GameConstant.MatchTime);

        if (_countdownSubscription == null)
        {
            _countdownSubscription = Observable.Interval(System.TimeSpan.FromSeconds(1))
                .TakeWhile(_ => DateTime.UtcNow < _matchEndAt)
                .Subscribe(
                    onNext: _ =>
                    {
                        var remainSeconds = (int)(_matchEndAt - DateTime.UtcNow).TotalSeconds;
                        MatchEventDispatcher.Instance.OnUpdateTimeLeftSubject.OnNext(remainSeconds);
                    },
                    onCompleted: _ =>
                    {
                        MatchEventDispatcher.Instance.OnUpdateTimeLeftSubject.OnNext(0);
                        OnMatchEnd();
                    });
        }
        else
        {
            throw new InvalidOperationException("前のCountDownが終了していない");
        }
    }
    private void OnMatchEnd()
    {
        _matchResult = PlayerScore.CurrentScore;
        _countdownSubscription?.Dispose();
        _countdownSubscription = null;
        ModelCache.Match.OnMatchFinished(_matchResult.Value);
    }
    #region IMatchの公開機能
    public bool HasResult => _matchResult.HasValue;
    public bool HasTargetStack => TargetStackInfo != null && TargetStackInfo.CurrentPoint > 0;
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
    #endregion
}
