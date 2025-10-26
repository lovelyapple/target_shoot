using System;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class UIPlayerScoreView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ScoreText;
    [SerializeField] private RectTransform Root;
    [SerializeField] private UIPlayerScoreDiffView DiffAddPrefab;
    [SerializeField] private UIPlayerScoreDiffView DiffDownPrefab;
    public void ApplyScore(int after, int diff)
    {
        ScoreText.text = after.ToString("0000");
    
        if(diff > 0)
        {
            PlayEffect(DiffAddPrefab, Math.Abs(diff));
        }
        else
        {
            PlayEffect(DiffDownPrefab, Math.Abs(diff));
        }
    }
    private void PlayEffect(UIPlayerScoreDiffView prefabView, int displayScore)
    {
        var fx = Instantiate(prefabView);
        var tran = fx.transform as RectTransform;
        tran.SetParent(this.Root);
        tran.anchoredPosition = Vector2.zero;
        tran.localScale = Vector3.one;
        var view = fx.GetComponent<UIPlayerScoreDiffView>();
        view.SetScore(displayScore);
        fx.gameObject.SetActive(true);
    }
}
