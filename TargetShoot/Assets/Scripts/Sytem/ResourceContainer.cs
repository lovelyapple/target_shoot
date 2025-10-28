using UnityEngine;

public class ResourceContainer : MonoSingletoneBase<ResourceContainer>
{
    [SerializeField] TargetBase TempNormalScoreTargetPrefab;
    [SerializeField] TargetBase TempHighScoreTargetPrefab;
    [SerializeField] TargetBase TempDownTargetPrefab;
    [SerializeField] TargetSpawnerBase TempSpawnerPrefab;
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
    public TargetSpawnerBase CreateTempSpawnerPrefab()
    {
        return GameObject.Instantiate(TempSpawnerPrefab.gameObject).GetComponent<TargetSpawnerBase>();
    }
}