using UnityEngine;
using GameDefinition;
public interface ITarget
{
    public FieldTargetType TargetType { get; }
}

public abstract class TargetBase : MonoBehaviour, ITarget
{
    public abstract FieldTargetType TargetType { get; }
    public int Score;
    public readonly Vector3 DefaultScale = new Vector3(1, 1, 0.1f);
    private Vector3 _startPos;
    private Vector3 _endPos;
    private float _duration;
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
    }
    public void Update()
    {
        _duration += Time.deltaTime * GameConstant.TargetMoveBaseSpeed;

        if (_duration > 1)
        {
            _duration = 0;
        }

        transform.position = Vector3.Lerp(_startPos, _endPos, _duration);
    }
}
