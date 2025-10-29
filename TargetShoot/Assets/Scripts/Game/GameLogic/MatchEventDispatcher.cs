using System;
using R3;
using UnityEngine;

public class MatchEventDispatcher : SingletonBase<MatchEventDispatcher>
{
    public readonly Subject<TargetBase> OnDispatchBulletHitSubject = new Subject<TargetBase>();
    public Observable<TargetBase> OnDispatchBulletHitObservable() => OnDispatchBulletHitSubject;
    public readonly Subject<TargetBase> OnDispatchCatchTargetSubject = new Subject<TargetBase>();
    public Observable<TargetBase> OnDispatchCatchTargetObservable() => OnDispatchCatchTargetSubject;

    public Subject<ScoreInfo> ScoreUpdateSubject = new Subject<ScoreInfo>();
    public Observable<ScoreInfo> ScoreUpdateObservable() => ScoreUpdateSubject;
    public Subject<TargetStackInfo> StackUpdateSubject = new Subject<TargetStackInfo>();
    public Observable<TargetStackInfo> StackUpdateObservable() => StackUpdateSubject;
}
