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
    private IMatch _match;
    private void Awake()
    {
        _chains.Add(Chain1);
        _chains.Add(Chain2);
        _chains.Add(Chain3);
    }
    public void Initialize(IMatch match)
    {
        _match = match;
    }
    public void OnUpdate()
    {
        if (_targetReviveDuation > 0)
        {
            if (_match.HasTargetStack)
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
            TryToFillChainWithOne();
            _targetReviveDuation += GameConstant.TargetReviveInterval;
        }
    }
    public void TryToFillChainWithOne()
    {
        if (!_match.HasTargetStack)
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
        _match.OnRespawnOneTarget();
    }
}
