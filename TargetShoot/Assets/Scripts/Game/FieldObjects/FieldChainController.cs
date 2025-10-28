using System.Collections.Generic;
using System.Linq;
using GameDefinition;
using UnityEngine;

public class FieldChainController : MonoBehaviour
{
    public Transform StartPoint;
    public Transform EndPoint;

    [Header("View Only")]
    [SerializeField] private List<TargetSpawnerBase> RunningSpanwers = new();
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
            var spawner = ResourceContainer.Instance.CreateTempSpawnerPrefab();
            spawner.transform.position = pos;
            spawner.transform.SetParent(this.transform);
            spawner.Init(startPosition, endPos: EndPoint.position);
            RunningSpanwers.Add(spawner);
        }
    }
    public bool CanInsertTarget()
    {
        return RunningSpanwers.Any(x => x.IsEmpty) && ModelCache.Match.TargetStackInfo.CurrentPoint > 0;
    }
    public bool RevieveOne()
    {
        foreach (var targetSpawner in RunningSpanwers)
        {
            if (targetSpawner.IsEmpty)
            {
                targetSpawner.RequestRespawn();
                return true;
            }
        }

        return false;
    }
}
