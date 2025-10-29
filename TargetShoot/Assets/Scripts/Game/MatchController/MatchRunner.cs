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
    private bool HasResult => Match != null && Match.HasResult;
    private void Awake()
    {
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
    }
    public void Start()
    {
        Init();
        MatchEventDispatcher.Instance.StackUpdateSubject.OnNext(TargetStackInfo);

        ModelCache.Match.OnMatchStart();
        StartCountDown();
    }
    private void OnDestroy()
    {
        ScoreComboManager?.FinishCountDown();
        _countdownSubscription?.Dispose();
        _countdownSubscription = null;
    }
    public void Init()
    {
        Match = new ArcadeMatch();
        Match.Init(PlayerScore, TargetStackInfo);

        PlayerScore = new PlayerScoreInfo();
        TargetStackInfo = new TargetStackInfo();

        Field.Initialize(Match);
        Player.Initialize(Match);

        ScoreComboManager = new ScoreComboManager(Match);

        // 必要なUI初期化
        Match.ApplyScore(0);
        PlayerScore.Apply(GameConstant.DefaultScore);
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
        var result = PlayerScore.CurrentScore;
        Match.SetupResult(result);
        _countdownSubscription?.Dispose();
        _countdownSubscription = null;
        ModelCache.Match.OnMatchFinished(result);
    }
}
