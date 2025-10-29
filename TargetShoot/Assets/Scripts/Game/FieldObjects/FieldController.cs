using System.Collections.Generic;
using System.Linq;
using GameDefinition;
using UnityEngine;

public class FieldController : MonoBehaviour
{
    [SerializeField] FieldChainController Chain1;
    [SerializeField] FieldChainController Chain2;
    [SerializeField] FieldChainController Chain3;
    private float _targetReviveDuation;
    private List<FieldChainController> _chains = new List<FieldChainController>();
    private void Awake()
    {
        _chains.Add(Chain1);
        _chains.Add(Chain2);
        _chains.Add(Chain3);
    }
    public void OnUpdate(MatchController match)
    {
        if (_targetReviveDuation > 0)
        {
            if (match.HasTargetStack)
            {
                _targetReviveDuation -= Time.deltaTime;
            }
            else if (_targetReviveDuation != GameConstant.TargetReviveInterval)
            {
                _targetReviveDuation = GameConstant.TargetReviveInterval;
            }
        }
        else
        {
            TryToFillChainWithOne(match);
            _targetReviveDuation += GameConstant.TargetReviveInterval;
        }
    }
    public void TryToFillChainWithOne(MatchController match)
    {
        if (!match.HasTargetStack)
        {
            return;
        }

        var chains = _chains.Where(x => x.CanInsertTarget()).ToArray();

        if (!chains.Any())
        {
            return;
        }

        var index = Random.Range(0, chains.Length);
        chains[index].RevieveOne();
        match.OnRespawnOneTarget();
    }
}
