using System;
using R3;
using UnityEngine;

public class MatchEventDispatcher : SingletonBase<MatchEventDispatcher>
{
    public readonly Subject<TargetBase> OnDispatchBulletHitSubject = new Subject<TargetBase>();
    public Observable<TargetBase> OnDispatchBulletHitObservable() => OnDispatchBulletHitSubject;
}
