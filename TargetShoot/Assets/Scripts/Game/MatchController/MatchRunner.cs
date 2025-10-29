using System;
using GameDefinition;
using R3;
using UnityEngine;

public class MatchRunner : MonoBehaviour
{
    [SerializeField] FieldController Field;
    [SerializeField] PlayerController Player;

    public PlayerScoreInfo PlayerScore { get; private set; }
    public TargetStackInfo TargetStackInfo { get; private set; }
    public ScoreComboManager ScoreComboManager { get; private set; }

    public IMatch Match { get; private set; }

    private IDisposable _countdownSubscription;
    private DateTime _matchEndAt;

    private void Awake()
    {
        PlayerScore = new PlayerScoreInfo();
        PlayerScore.Apply(GameConstant.DefaultScore);
        TargetStackInfo = new TargetStackInfo();

        MatchEventDispatcher.Instance.OnDispatchBulletHitObservable()
        .Where(_ => !Match.HasResult)
        .Subscribe(target => OnBulletHitTarget(target))
        .AddTo(this);

        MatchEventDispatcher.Instance.OnDispatchCatchTargetObservable()
        .Where(_ => !Match.HasResult)
        .Subscribe(target => OnCatchFallTarget(target))
        .AddTo(this);

        MatchEventDispatcher.Instance.OnBulletMissedAllObservable()
        .Where(_ => !Match.HasResult)
        .Subscribe(_ => ScoreComboManager?.OnBulletMissedAll())
        .AddTo(this);

        Field.Initialize(Match);
        Player.Initialize(Match);
        ScoreComboManager = new ScoreComboManager(Match);
    }

    private void OnDestroy()
    {
        ScoreComboManager?.FinishCountDown();

        _countdownSubscription?.Dispose();
        _countdownSubscription = null;
    }
    public void Update()
    {
        if (!Match.HasResult)
            Field.OnUpdate();
    }
    private void OnBulletHitTarget(ITarget iTarget)
    {
        var comboBonus = GameConstant.GetComboTimes(iTarget.HitCombo);
        var applyScore = (int)(iTarget.Score * comboBonus);
        Match.ApplyScore(applyScore);
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
}
