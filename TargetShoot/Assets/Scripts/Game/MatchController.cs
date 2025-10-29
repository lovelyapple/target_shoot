using UnityEngine;

public class MatchController : MonoBehaviour
{
    [SerializeField] FieldController FieldController;
    public ScoreComboManager ScoreComboManager { get; private set; }
    private void Awake()
    {
        ScoreComboManager = new ScoreComboManager();
    }

    public void Update()
    {

    }
}
