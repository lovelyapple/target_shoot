using System;
using GameDefinition;
using R3;
using UnityEngine;
public class ScoreInfo
{
    public int AfterScore;
    public int Diff;
}
public class MatchModel : IModel
{
    public PlayerScoreInfo PlayerScore { get; private set; }
    public TargetStackInfo TargetStackInfo { get; private set; }
    private CompositeDisposable _dispatcherDisposable = null;
    private Subject<ScoreInfo> _scoreUpdateSubject = new Subject<ScoreInfo>();
    public Observable<ScoreInfo> ScoreUpdateObservable() => _scoreUpdateSubject;
    private Subject<TargetStackInfo> _stackUpdateSubject = new Subject<TargetStackInfo>();
    public Observable<TargetStackInfo> StackUpdateObservable() => _stackUpdateSubject;
    public void Reset()
    {
        if (_dispatcherDisposable != null)
        {
            _dispatcherDisposable.Dispose();
            _dispatcherDisposable = null;
        }

        _dispatcherDisposable = new CompositeDisposable();
        RegisterDispatcher();
        PlayerScore = new PlayerScoreInfo();
        PlayerScore.Apply(GameConstant.DefaultScore);
        TargetStackInfo = new TargetStackInfo();
    }
    private void RegisterDispatcher()
    {
        MatchEventDispatcher.Instance.OnDispatchBulletHitObservable()
        .Subscribe(target => OnBulletHitTarget(target))
        .AddTo(_dispatcherDisposable);

        MatchEventDispatcher.Instance.OnDispatchCatchTargetObservable()
        .Subscribe(target => OnCatchFallTarget(target))
        .AddTo(_dispatcherDisposable);
    }
    private void OnBulletHitTarget(TargetBase targetBase)
    {
        PlayerScore.Apply(targetBase.Score);

        var scoreInfo = new ScoreInfo()
        {
            Diff = targetBase.Score,
            AfterScore = PlayerScore.CurrentScore,
        };

        _scoreUpdateSubject.OnNext(scoreInfo);
    }
    private void OnCatchFallTarget(TargetBase targetBase)
    {
        TargetStackInfo.AddPoint(targetBase.Score);
        _stackUpdateSubject.OnNext(TargetStackInfo);
    }
    public void OnRespawnOneTarget()
    {
        TargetStackInfo.UseOne();
        _stackUpdateSubject.OnNext(TargetStackInfo);
    }
    public void OnStart()
    {
        var scoreInfo = new ScoreInfo()
        {
            Diff = PlayerScore.CurrentScore,
            AfterScore = PlayerScore.CurrentScore,
        };
        _scoreUpdateSubject.OnNext(scoreInfo);
        _stackUpdateSubject.OnNext(TargetStackInfo);
    }
}
