using GameDefinition;
using UnityEngine;
using R3;

public interface ITargetSpawner
{
    public Vector3 StartPos { get; }
    public Vector3 EndPos { get; }
    public TargetBase Target { get; }
}
public class TargetSpawnerBase : MonoBehaviour, ITargetSpawner
{
    private Vector3 _startPos;
    private Vector3 _endPos;
    private TargetBase _target;
    public Vector3 StartPos => _startPos;
    public Vector3 EndPos => EndPos;
    public TargetBase Target => _target;
    public bool IsEmpty => Target == null;
    private float _duration;
    private bool _isRequestRespawn;
    public void Init(Vector3 startPos, Vector3 endPos)
    {
        _startPos = startPos;
        _endPos = endPos;
        var range = Vector3.Distance(_startPos, _endPos);
        var currentRange = Vector3.Distance(transform.position, _endPos);

        if (currentRange > range)
        {
            currentRange = range;
        }

        _duration = (float)currentRange / range;

        SpawnTarget();
    }
    public void Update()
    {
        _duration += Time.deltaTime * GameConstant.TargetMoveBaseSpeed;

        if (_duration > 1)
        {
            _duration = 0;

            if (_isRequestRespawn)
            {
                SpawnTarget();
            }
        }

        transform.position = Vector3.Lerp(_startPos, _endPos, _duration);
    }
    private void SpawnTarget()
    {
        _target = GenerateTargetBase();
        _target.transform.SetParent(this.transform);
        _target.transform.localPosition = Vector3.zero;
        _isRequestRespawn = false;

        _target.OnBulletHitObservable()
        .Subscribe(target => OnBulletHit(target))
        .AddTo(this);
    }
    private void OnBulletHit(TargetBase targetBase)
    {
        MatchEventDispatcher.Instance.OnDispatchBulletHitSubject.OnNext(targetBase);
        Destroy(targetBase.gameObject);
        _target = null;
    }
    public void RequestRespawn()
    {
        _isRequestRespawn = true;
    }
    private TargetBase GenerateTargetBase()
    {
        var res = UnityEngine.Random.Range(0, GameConstant.MaxProbability);

        if (res < GameConstant.HighScoreTargetProbability)
        {
            return ResourceContainer.Instance.CreateHighScoreTarget();
        }
        else if (res < GameConstant.DownTargetProbability + GameConstant.HighScoreTargetProbability)
        {
            return ResourceContainer.Instance.CreateDownTargettarget();
        }
        else
        {
            return ResourceContainer.Instance.CreatePrefabNormalTarget();
        }
    }
}
