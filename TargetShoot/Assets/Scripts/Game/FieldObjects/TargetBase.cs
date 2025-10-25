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
}
