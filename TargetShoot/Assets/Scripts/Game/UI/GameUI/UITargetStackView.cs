using TMPro;
using UnityEngine;
using R3;
public class UITargetStackView : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI TextStackCount;
    private void Awake()
    {
        MatchEventDispatcher.Instance.StackUpdateObservable()
        .Subscribe(UpdateStack)
        .AddTo(this);
    }
    private void UpdateStack(TargetStackInfo info)
    {
        TextStackCount.text = info.CurrentPoint.ToString("00");
    }
}
