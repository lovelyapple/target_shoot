using TMPro;
using UnityEngine;

public class UIPlayerScoreDiffView : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI ScoreText;
    public void SetScore(int score)
    {
        ScoreText.text = score.ToString("00");
    }
}
