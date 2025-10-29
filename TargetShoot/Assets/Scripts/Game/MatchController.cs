using GameDefinition;
using UnityEngine;
using R3;
using System;
public class ScoreInfo
{
    public int AfterScore;
    public int Diff;
}
public interface IMatch
{
    public bool HasTargetStack { get; }
    public bool HasResult { get; }
    public bool CanFire();
    public void OnFire();
    public void OnRespawnOneTarget();
    public void OnUpdateScoreCombo(int CurrentCombo);
    public void OnReceiveScoreComboPoint(int score);
}
public class MatchController : MonoBehaviour, IMatch
{
    [SerializeField] FieldController Field;
    [SerializeField] PlayerController Player;

    public PlayerScoreInfo PlayerScore { get; private set; }
    public TargetStackInfo TargetStackInfo { get; private set; }
    public ScoreComboManager ScoreComboManager { get; private set; }

    private IDisposable _countdownSubscription;
    private DateTime _matchEndAt;
    private long? _matchResult = null;
    public bool HasResult => _matchResult.HasValue;
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
        Field.OnUpdate();
    }
    private void OnBulletHitTarget(ITarget iTarget)
    {
        var comboBonus = iTarget.HitCombo > 1 ? (iTarget.HitCombo - 1) * GameConstant.BulletComboBonusScoreTimes : 1;
        var applyScore = iTarget.Score * comboBonus;
        ApplyScore(applyScore);
        ScoreComboManager.AddComobo(iTarget.HitCombo);
    }
    private void OnCatchFallTarget(TargetBase targetBase)
    {
        TargetStackInfo.AddPoint(targetBase.CatchStackCount);
        MatchEventDispatcher.Instance.StackUpdateSubject.OnNext(TargetStackInfo);

        // add one score combo too
        // 全部取るのは良くないから、方法を考える
        if (targetBase.CatchStackCount > GameConstant.HighScoreTargetScore)
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
    public bool HasTargetStack => TargetStackInfo != null && TargetStackInfo.CurrentPoint > 0;
    public void OnRespawnOneTarget()
    {
        TargetStackInfo.UseOne();
        MatchEventDispatcher.Instance.StackUpdateSubject.OnNext(TargetStackInfo);
    }
    public bool CanFire()
    {
        return !HasResult && PlayerScore.CurrentScore > 0;
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
    #endregion

    private void ApplyScore(int score)
    {
        if (HasResult)
        {
            return;
        }

        PlayerScore.Apply(score);

        var scoreInfo = new ScoreInfo()
        {
            Diff = score,
            AfterScore = PlayerScore.CurrentScore,
        };

        MatchEventDispatcher.Instance.ScoreUpdateSubject.OnNext(scoreInfo);

        // 残り０なので強制終了
        if (score < 0 && PlayerScore.CurrentScore <= 0)
        {
            _matchEndAt = new(0);
        }
    }
}
