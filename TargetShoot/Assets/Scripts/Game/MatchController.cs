using UnityEngine;

public class MatchController : MonoBehaviour
{
    [SerializeField] FieldController FieldController;
    public ScoreComboManager ScoreComboManager { getg; private set; }
    private void Awake()
    {
        ScoreComboManager = new ScoreComboManager();
    }

    public void Update()
    {

    }
}
