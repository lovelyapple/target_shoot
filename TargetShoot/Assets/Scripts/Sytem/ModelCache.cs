using System.Collections.Generic;
using UnityEngine;

public class ModelCache
{
    private static ModelCache _intance;
    private static ModelCache _intanceInternal
    {
        get
        {
            if (_intance == null)
            {
                _intance = new ModelCache();
            }

            return _intance;
        }
    }
    private MatchModel _matchModelInstance;
    public static MatchModel MatchModel => _intanceInternal._matchModelInstance;
    public ModelCache()
    {
        if (_intance != null)
        {
            return;
        }

        _intance = this;

        _matchModelInstance = new MatchModel();
    }
}
