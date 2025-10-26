using UnityEngine;
using R3;
public class MatchUI : MonoBehaviour
{
    [SerializeField] private UIPlayerScoreView ScoreView;

    private void Awake()
    {
        ModelCache.Match.ScoreUpdateObservable()
        .Subscribe(ApplyScore)
        .AddTo(this);
    }
    private void ApplyScore(ScoreInfo scoreInfo)
    {
        ScoreView.ApplyScore(scoreInfo.AfterScore, scoreInfo.Diff);
    }
}
