using System;
using R3;
using UnityEngine;

public class MatchModel : IModel
{
    public PlayerScoreInfo PlayerScore { get; private set; }
    private CompositeDisposable _dispatcherDisposable = null;
    private Subject<int> _scoreUpdateSubject = new Subject<int>();
    public Observable<int> ScoreUpdateObservable() => _scoreUpdateSubject;
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
        _scoreUpdateSubject.OnNext(PlayerScore.CurrentScore);
    }
}
