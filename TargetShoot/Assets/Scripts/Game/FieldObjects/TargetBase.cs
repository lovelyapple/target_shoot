using UnityEngine;
using GameDefinition;
using R3;
public interface ITarget
{
    public FieldTargetType TargetType { get; }
    public int Score { get; }
    public int CatchStackCount { get; }
    public int HitCombo { get; }
}

public abstract class TargetBase : MonoBehaviour, ITarget
{
    [Tooltip("Setting")]
    [SerializeField] int ScoreSetting;
    [SerializeField] int CatchStackCountSetting;
    public abstract FieldTargetType TargetType { get; }
    public int Score => ScoreSetting;
    public int CatchStackCount => CatchStackCountSetting;
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
    private void Update()
    {
        if (transform.position.y < GameConstant.TargetDispearLimitHeight)
        {
            Destroy(gameObject);
        }
    }
    public void OnHit(int currentCombo)
    {
        HitCombo = currentCombo;

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
