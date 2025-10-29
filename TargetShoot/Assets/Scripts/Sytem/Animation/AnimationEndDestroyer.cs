using UnityEngine;

public class AnimationEndDestroyer : MonoBehaviour
{
    public void OnAnimaPlayEnd()
    {
        Destroy(gameObject);
    }
}
