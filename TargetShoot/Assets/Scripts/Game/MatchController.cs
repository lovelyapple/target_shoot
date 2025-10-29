using GameDefinition;
using UnityEngine;
using R3;

public interface IMatch
{
    public bool HasTargetStack { get; }
    public bool CanFire();
    public void OnFire();
    public void OnRespawnOneTarget();
    public void OnUpdateScoreCombo(int CurrentCombo);
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
        PlayerScore = new PlayerScoreInfo();
        PlayerScore.Apply(GameConstant.DefaultScore);
        TargetStackInfo = new TargetStackInfo();

        MatchEventDispatcher.Instance.OnDispatchBulletHitObservable()
        .Subscribe(target => OnBulletHitTarget(target))
        .AddTo(this);

        MatchEventDispatcher.Instance.OnDispatchCatchTargetObservable()
        .Subscribe(target => OnCatchFallTarget(target))
        .AddTo(this);

        MatchEventDispatcher.Instance.OnBulletMissedAllObservable()
        .Subscribe(_ => ScoreComboManager?.OnBulletMissedAll())
        .AddTo(this);

        Field.Initialize(this);
        Player.Initialize(this);
        ScoreComboManager = new ScoreComboManager(this);
    }
    private void OnDestroy()
    {
        ScoreComboManager?.FinishCountDown();
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
    private void OnBulletHitTarget(ITarget iTarget)
    {
        var applyScore = iTarget.Score * iTarget.HitCombo * GameConstant.BulletComboBonusScoreTimes;
        PlayerScore.Apply(applyScore);

        var scoreInfo = new ScoreInfo()
        {
            Diff = applyScore,
            AfterScore = PlayerScore.CurrentScore,
        };

        MatchEventDispatcher.Instance.ScoreUpdateSubject.OnNext(scoreInfo);
        ScoreComboManager.AddComobo(iTarget.HitCombo);
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
    public void OnUpdateScoreCombo(int CurrentCombo)
    {
        MatchEventDispatcher.Instance.OnScoreComboUpdateSubject.OnNext(CurrentCombo);
    }
    #endregion
}
