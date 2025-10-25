using UnityEngine;

public class ResourceContainer : MonoSingletoneBase<ResourceContainer>
{
    [SerializeField] TargetBase TempNormalScoreTargetPrefab;
    [SerializeField] TargetBase TempHighScoreTargetPrefab;
    [SerializeField] TargetBase TempDownTargetPrefab;
    private void Awake()
    {
        Initialize(this);
    }
    public TargetBase CreatePrefabNormalTarget()
    {
        return GameObject.Instantiate(TempNormalScoreTargetPrefab.gameObject).GetComponent<TargetBase>();
    }
    public TargetBase CreateHighScoreTarget()
    {
        return GameObject.Instantiate(TempHighScoreTargetPrefab.gameObject).GetComponent<TargetBase>();
    }
    public TargetBase CreateDownTargettarget()
    {
        return GameObject.Instantiate(TempDownTargetPrefab.gameObject).GetComponent<TargetBase>();
    }
}