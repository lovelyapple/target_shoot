using System;
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
    private CompositeDisposable _dispatcherDisposable = null;
    private Subject<ScoreInfo> _scoreUpdateSubject = new Subject<ScoreInfo>();
    public Observable<ScoreInfo> ScoreUpdateObservable() => _scoreUpdateSubject;
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
    }
    private void RegisterDispatcher()
    {
        MatchEventDispatcher.Instance.OnDispatchBulletHitObservable()
        .Subscribe(target => OnBulletHitTarget(target))
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
}
