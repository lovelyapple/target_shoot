using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using GameDefinition;
using UnityEngine;
using R3;

public class FieldChainController : MonoBehaviour
{
    public Transform StartPoint;
    public Transform EndPoint;

    [Header("View Only")]
    [SerializeField] private List<TargetBase> RunningTargets = new();
    [SerializeField] private float PointDistance = 0;
    [SerializeField] private int MaxTargetCounts = 0;
    [SerializeField] private Vector3 Direction;
    private void Awake()
    {
        Direction = (EndPoint.position - StartPoint.position).normalized;
        PointDistance = Vector3.Distance(StartPoint.position, EndPoint.position);
        MaxTargetCounts = (int)(PointDistance / (1 + GameConstant.TargetPlateDistance));
    }
    void Start()
    {
        FillChainDefault();
    }
    public void FillChainDefault()
    {
        var startPosition = StartPoint.position;

        for (int i = 0; i < MaxTargetCounts; i++)
        {
            var distance = i * (1 + GameConstant.TargetPlateDistance) * Direction + Direction * 0.5f;
            var pos = startPosition + distance;
            var target = GenerateTargetBase();
            target.transform.position = pos;
            target.transform.SetParent(this.transform);
            target.Init(startPosition, endPos: EndPoint.position);
            RunningTargets.Add(target);

            target.OnBulletHitObservable()
            .Subscribe(target => OnBulletHit(target))
            .AddTo(this);
        }
    }
    public bool CanInsertTarget()
    {
        return RunningTargets.Count < MaxTargetCounts;
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
    private void OnBulletHit(TargetBase targetBase)
    {
        var target = RunningTargets.FirstOrDefault(x => x == targetBase);

        if (target == null)
        {
            Debug.LogError($"can not find target");
            return;
        }

        RunningTargets.Remove(target);
        Destroy(target.gameObject);
    }
}
