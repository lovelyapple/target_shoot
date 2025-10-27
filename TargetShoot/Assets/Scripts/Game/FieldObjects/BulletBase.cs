using System.Linq;
using GameDefinition;
using R3;
using UnityEngine;

public class BulletBase : MonoBehaviour
{
    private static int _layerMask = -1;
    private float _lifeTime = 3f;
    private void Awake()
    {
        if (_layerMask == -1)
        {
            _layerMask = 1 << LayerMask.NameToLayer("Target");
        }
    }
    public void Update()
    {
        var move = Time.deltaTime * GameConstant.BulletSpeed;

        var hits = Physics.RaycastAll(
            transform.position,
            Vector3.forward,
            move,
            _layerMask
        );

        if (hits.Any())
        {
            foreach (var hit in hits)
            {
                var target = hit.transform.gameObject.GetComponent<TargetBase>();

                if (target != null)
                {
                    target.OnHit();
                }
            }

            // Destroy(this.gameObject);
            return;
        }

        _lifeTime -= Time.deltaTime;

        if (_lifeTime <= 0)
        {
            Destroy(this.gameObject);
        }

        transform.position += Vector3.forward * move;
    }
}
