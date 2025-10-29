using System.Linq;
using GameDefinition;
using R3;
using UnityEngine;
using UnityEngine.UIElements;

public class BulletBase : MonoBehaviour
{
    private static int _layerMask = -1;
    private float _lifeTime = 3f;
    private Vector3 _moveDirection;
    public BulletComboInfo ComboInfo { get; private set; }
    private void Awake()
    {
        if (_layerMask == -1)
        {
            _layerMask = 1 << LayerMask.NameToLayer("Target");
        }
    }
    public void Setup(Vector3 moveDireciont)
    {
        _moveDirection = moveDireciont;
        ComboInfo = new BulletComboInfo();
    }
    public void Update()
    {
        var move = Time.deltaTime * GameConstant.BulletSpeed;

        var hits = Physics.RaycastAll(
            transform.position,
            _moveDirection,
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
                    ComboInfo.AddCombo();
                    target.OnHit(ComboInfo.CurrentCombo);
                }
            }

            return;
        }

        _lifeTime -= Time.deltaTime;

        if (_lifeTime <= 0)
        {
            if (ComboInfo.CurrentCombo == 0)
            {
                MatchEventDispatcher.Instance.OnBulletMissedAllSubject.OnNext(Unit.Default);
            }

            Destroy(this.gameObject);
        }

        transform.position += _moveDirection * move;
    }
}
