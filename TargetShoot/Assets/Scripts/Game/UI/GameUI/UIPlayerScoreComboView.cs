using TMPro;
using UnityEngine;
using R3;
using GameDefinition;
public class UIPlayerScoreComboView : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI ComboText;
    [SerializeField] TextMeshProUGUI ComboTimesText;
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
            ComboTimesText.gameObject.SetActive(false);
            return;
        }

        ComboText.gameObject.SetActive(true);
        ComboTimesText.gameObject.SetActive(true);
        ComboText.text = combo.ToString("00");
        ComboTimesText.text = GameConstant.GetComboTimes(combo).ToString("0.00");
    }
}
