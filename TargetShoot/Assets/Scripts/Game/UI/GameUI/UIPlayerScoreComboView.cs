using TMPro;
using UnityEngine;
using R3;
public class UIPlayerScoreComboView : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI ComboText;
    private void Awake()
    {
        MatchEventDispatcher.Instance.OnScoreComboUpdateObservable()
        .Subscribe(UpdateComboText)
        .AddTo(this);
    }
    private void UpdateComboText(int combo)
    {
        if (combo == 0)
        {
            ComboText.gameObject.SetActive(false);
            return;
        }

        ComboText.gameObject.SetActive(true);
        ComboText.text = combo.ToString("00");
    }
}
