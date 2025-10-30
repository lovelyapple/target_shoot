using UnityEngine;
using GameDefinition;
using R3;
public interface ITarget
{
    public FieldTargetType TargetType { get; }
    public int Score { get; }
    public int CatchStackCount { get; }
    public int HitCombo { get; }
    public bool IsFrameTarget { get; }
    public Vector3 HitWorldPosition{ get; set; }
}

public abstract class TargetBase : MonoBehaviour, ITarget
{
    [Tooltip("Setting")]
    [SerializeField] int ScoreSetting;
    [SerializeField] int CatchStackCountSetting;
    [SerializeField] GameObject FrameObj;
    public abstract FieldTargetType TargetType { get; }
    public int Score => ScoreSetting;
    public int CatchStackCount => CatchStackCountSetting;
    public Vector3 HitWorldPosition { get; set; }
    private bool _isTarget = false;
    public bool IsFrameTarget => _isTarget;
    public int HitCombo { get; private set; } = 0;
    public readonly Vector3 DefaultScale = new Vector3(1, 1, 0.1f);
    private Subject<ITarget> _onBulletHit = new Subject<ITarget>();
    public Observable<ITarget> OnBulletHitObservable() => _onBulletHit;
    private Rigidbody _rigidbody;
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.isKinematic = true;
    }
    public void SetAsFrameTarget()
    {
        _isTarget = true;
        FrameObj.SetActive(true);
    }
    private void Update()
    {
        if (transform.position.y < GameConstant.TargetDispearLimitHeight)
        {
            Destroy(gameObject);
        }
    }
    public void OnHit(int currentCombo, Vector3 hitWorldPosition)
    {
        HitCombo = currentCombo;
        HitWorldPosition = hitWorldPosition;

        _onBulletHit.OnNext(this);
        _onBulletHit.OnCompleted();

        _rigidbody.isKinematic = false;
        gameObject.layer = LayerMask.NameToLayer("Default");
    }
    public void OnTriggerEnter(Collider col)
    {
        if (col.transform.tag == "Backet")
        {
            MatchEventDispatcher.Instance.OnDispatchCatchTargetSubject.OnNext(this);
        }
    }
}
