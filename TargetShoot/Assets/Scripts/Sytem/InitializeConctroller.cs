using UnityEngine;

public class InitializeConctroller : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SceneTransit.RequestGotoScene(GameDefinition.SceneType.Title);
    }
}
