using TMPro;
using UnityEngine;
using R3;
public class UIGameTimeLeftView : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI TextTimeLeft;
    private void Awake()
    {
        MatchEventDispatcher.Instance.OnUpdateTimeLeftObservable()
        .Subscribe(UpdateTimeLeft)
        .AddTo(this);
    }
    private void UpdateTimeLeft(int timeLeft)
    {
        TextTimeLeft.text = timeLeft.ToString("00");
    }
}
