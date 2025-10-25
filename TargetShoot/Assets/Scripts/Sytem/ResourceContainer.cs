using UnityEngine;

public class ResourceContainer : MonoSingletoneBase<ResourceContainer>
{
    [SerializeField] TargetBase TempNormalScoreTargetPrefab;
    [SerializeField] TargetBase TempHighScoreåTargetPrefab;
    [SerializeField] TargetBase TempDownTargetPrefab;
}