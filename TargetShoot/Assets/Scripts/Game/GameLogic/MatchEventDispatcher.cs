using System;
using R3;
using UnityEngine;

public class MatchEventDispatcher : SingletonBase<MatchEventDispatcher>
{
    public readonly Subject<ITarget> OnDispatchBulletHitSubject = new Subject<ITarget>();
    public Observable<ITarget> OnDispatchBulletHitObservable() => OnDispatchBulletHitSubject;
    public readonly Subject<TargetBase> OnDispatchCatchTargetSubject = new Subject<TargetBase>();
    public Observable<TargetBase> OnDispatchCatchTargetObservable() => OnDispatchCatchTargetSubject;

    public Subject<ScoreInfo> ScoreUpdateSubject = new Subject<ScoreInfo>();
    public Observable<ScoreInfo> ScoreUpdateObservable() => ScoreUpdateSubject;
    public Subject<TargetStackInfo> StackUpdateSubject = new Subject<TargetStackInfo>();
    public Observable<TargetStackInfo> StackUpdateObservable() => StackUpdateSubject;

    public Subject<int> OnScoreComboUpdateSubject = new Subject<int>();
    public Subject<int> OnScoreComboUpdateObservable() => OnScoreComboUpdateSubject;
}
