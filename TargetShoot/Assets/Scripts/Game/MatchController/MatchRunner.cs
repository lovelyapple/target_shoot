using System;
using GameDefinition;
using R3;
using UnityEngine;

public class MatchRunner : MonoBehaviour
{
    [SerializeField] FieldController Field;
    [SerializeField] PlayerController Player;

    private IMatch _match;
    private bool HasResult => _match != null && _match.HasResult;

    private PlayerScoreInfo _playerScore;
    private TargetStackInfo _targetStackInfo;
    private ScoreComboManager _scoreComboManager;

    private IDisposable _countdownSubscription;
    private DateTime _matchEndAt;
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
        .Subscribe(_ => _scoreComboManager?.OnBulletMissedAll())
        .AddTo(this);
    }
    public void Start()
    {
        Init();
        StartCountDown();
        ModelCache.Match.OnMatchStart();
    }
    private void OnDestroy()
    {
        _scoreComboManager?.FinishCountDown();
        _scoreComboManager = null;

        _countdownSubscription?.Dispose();
        _countdownSubscription = null;
    }
    public void Init()
    {
        _match = new ArcadeMatch();

        _playerScore = new PlayerScoreInfo();
        _targetStackInfo = new TargetStackInfo();
        _scoreComboManager = new ScoreComboManager(_match);

        _match.Init(_playerScore, _targetStackInfo, _scoreComboManager);

        Field.Initialize(_match);
        Player.Initialize(_match);

        // 必要なUI初期化
        _match.ApplyScore(GameConstant.DefaultScore);
        MatchEventDispatcher.Instance.StackUpdateSubject.OnNext(_targetStackInfo);
    }

    public void Update()
    {
        if (!HasResult)
            Field.OnUpdate();
    }
    private void OnBulletHitTarget(ITarget iTarget)
    {
        _match.OnBulletHitTarget(iTarget);
    }
    private void OnCatchFallTarget(ITarget iTarget)
    {
        _match.OnCatchFallTarget(iTarget);
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
        var result = _playerScore.CurrentScore;
        _match.SetupResult(result);
        _countdownSubscription?.Dispose();
        _countdownSubscription = null;
        ModelCache.Match.OnMatchFinished(result);
    }
}
