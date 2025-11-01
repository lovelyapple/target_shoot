using System.Collections.Generic;
using UnityEngine;

public class UIBulletView : MonoBehaviour
{
    [SerializeField] private List<GameObject> BulletObjects;

    public void UpdateBullet(int bulletCount)
    {
        var cnt = BulletObjects.Count;

        for(int i = 0; i < cnt; i++)
        {
            BulletObjects[i].SetActive(i < bulletCount);
        }
    } 
}
