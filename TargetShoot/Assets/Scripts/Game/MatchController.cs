using GameDefinition;
using UnityEngine;
using R3;

public interface IMatch
{
    public bool CanFire();
    public void OnFire();
    public void OnRespawnOneTarget();
    public bool HasTargetStack { get; }
}
public class MatchController : MonoBehaviour, IMatch
{
    [SerializeField] FieldController Field;
    [SerializeField] PlayerController Player;

    public PlayerScoreInfo PlayerScore { get; private set; }
    public TargetStackInfo TargetStackInfo { get; private set; }
    public ScoreComboManager ScoreComboManager { get; private set; }
    private void Awake()
    {
        ScoreComboManager = new ScoreComboManager();

        PlayerScore = new PlayerScoreInfo();
        PlayerScore.Apply(GameConstant.DefaultScore);
        TargetStackInfo = new TargetStackInfo();

        MatchEventDispatcher.Instance.OnDispatchBulletHitObservable()
        .Subscribe(target => OnBulletHitTarget(target))
        .AddTo(this);

        MatchEventDispatcher.Instance.OnDispatchCatchTargetObservable()
        .Subscribe(target => OnCatchFallTarget(target))
        .AddTo(this);

        Field.Initialize(this);
        Player.Initialize(this);
    }
    public void OnStart()
    {
        var scoreInfo = new ScoreInfo()
        {
            Diff = PlayerScore.CurrentScore,
            AfterScore = PlayerScore.CurrentScore,
        };
        MatchEventDispatcher.Instance.ScoreUpdateSubject.OnNext(scoreInfo);
        MatchEventDispatcher.Instance.StackUpdateSubject.OnNext(TargetStackInfo);
    }
    public void Update()
    {
        Field.OnUpdate();
    }
    private void OnBulletHitTarget(TargetBase targetBase)
    {
        var applyScore = targetBase.Score * targetBase.HitCombo * GameConstant.ComboBonusTimes;
        PlayerScore.Apply(applyScore);

        var scoreInfo = new ScoreInfo()
        {
            Diff = applyScore,
            AfterScore = PlayerScore.CurrentScore,
        };

        MatchEventDispatcher.Instance.ScoreUpdateSubject.OnNext(scoreInfo);
    }
    private void OnCatchFallTarget(TargetBase targetBase)
    {
        TargetStackInfo.AddPoint(targetBase.CatchStackCount);
        MatchEventDispatcher.Instance.StackUpdateSubject.OnNext(TargetStackInfo);
    }
    #region IMatchの公開機能
    public bool HasTargetStack => TargetStackInfo != null && TargetStackInfo.CurrentPoint > 0;
    public void OnRespawnOneTarget()
    {
        TargetStackInfo.UseOne();
        MatchEventDispatcher.Instance.StackUpdateSubject.OnNext(TargetStackInfo);
    }
    public bool CanFire()
    {
        return PlayerScore.CurrentScore > 0;
    }
    public void OnFire()
    {
        PlayerScore.DecreaseOne();

        var scoreInfo = new ScoreInfo()
        {
            Diff = -GameConstant.FireCost,
            AfterScore = PlayerScore.CurrentScore,
        };

        MatchEventDispatcher.Instance.ScoreUpdateSubject.OnNext(scoreInfo);
    }
    #endregion
}
