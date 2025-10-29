using System;
using GameDefinition;
using R3;
using UnityEngine;

public class ScoreComboManager
{
    public ScoreComboInfo ScoreCombo { get; private set; }
    public DateTime ComboEndAt { get; private set; }
    private IMatch _match;
    private IDisposable _countdownSubscription;
    public ScoreComboManager(IMatch match)
    {
        ScoreCombo = new ScoreComboInfo();
        _match = match;
    }
    public void AddComobo(int hitStep)
    {
        if (hitStep == 1)
        {
            return;
        }
        else if (hitStep == 2)
        {
            ScoreCombo.OnAddOne();
        }
        else if (hitStep == 3)
        {
            ScoreCombo.OnAddTwo();
        }

        TryToStartCountDown();
    }
    private void TryToStartCountDown()
    {
        var endTime = DateTime.UtcNow.AddSeconds(GameConstant.ComboLastSec);
        double unixTimeMs = new DateTimeOffset(endTime).ToUnixTimeMilliseconds();
        ComboEndAt = DateTimeOffset.FromUnixTimeMilliseconds((long)unixTimeMs).UtcDateTime;

        _match.OnUpdateScoreCombo(ScoreCombo.CurrentCombo);

        if (_countdownSubscription == null)
        {
            _countdownSubscription = Observable.Interval(System.TimeSpan.FromSeconds(1))
                .TakeWhile(_ => DateTime.UtcNow < ComboEndAt)
                .Subscribe(
                    onNext: _ =>
                    {
                        // UI次第で何かする
                    },
                    onCompleted: _ =>
                    {
                        ComboTimeOut();
                    });
        }
    }
    private void ComboTimeOut()
    {
        _match.OnReceiveScoreComboPoint(ScoreCombo.GetScore());
        ScoreCombo.OnReset();
        _match.OnUpdateScoreCombo(ScoreCombo.CurrentCombo);
        FinishCountDown();
    }
    public void OnBulletMissedAll()
    {
        ComboEndAt = new(0);
    }
    public void FinishCountDown()
    {
        _countdownSubscription?.Dispose();
        _countdownSubscription = null;
    }
}
