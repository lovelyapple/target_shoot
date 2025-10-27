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
    public void Awake()
    {
        _chains.Add(Chain1);
        _chains.Add(Chain2);
        _chains.Add(Chain3);
    }
    public void Update()
    {
        if (_targetReviveDuation > 0)
        {
            _targetReviveDuation -= Time.deltaTime;
        }
        else
        {
            TryToFillChainWithOne();
            _targetReviveDuation += GameConstant.TargetReviveInterval;
        }
    }
    public void TryToFillChainWithOne()
    {
        var chains = _chains.Where(x => x.CanInsertTarget());

        if (!chains.Any())
        {
            return;
        }

        
    }
}
