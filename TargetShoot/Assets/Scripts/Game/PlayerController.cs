using GameDefinition;
using UnityEngine;
using R3;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Transform FirePointTransform;
    [SerializeField] Transform TargetPointTransform;
    [SerializeField] GameObject BulletPrefab;
    private Vector3 _moveDirection = Vector3.zero;
    private Vector3 _bulletMoveDierction = Vector3.zero;
    private void Awake()
    {
        InputController.Instance.OnInputMoveHorizentalObservable()
        .Subscribe(x => OnInput(x))
        .AddTo(this);

        InputController.Instance.OnInputFireObservable()
        .Subscribe(_ => OnInputFire())
        .AddTo(this);

        _bulletMoveDierction = (TargetPointTransform.position - FirePointTransform.position).normalized;
    }
    private void Update()
    {
        if (_moveDirection == Vector3.zero)
        {
            return;
        }

        var nextPosition = transform.position + _moveDirection * Time.deltaTime * GameConstant.MoveSpeed;

        nextPosition.x = Mathf.Clamp(nextPosition.x, -GameConstant.MoveLimit, GameConstant.MoveLimit);

        transform.position = nextPosition;
    }
    private void OnInput(int dir)
    {
        if (dir == 0)
        {
            _moveDirection = Vector3.zero;
            return;
        }

        if (dir == 1)
        {
            _moveDirection = Vector3.right;
        }
        else
        {
            _moveDirection = Vector3.left;
        }
    }
    private void OnInputFire()
    {
        var obj = Instantiate(BulletPrefab);
        obj.gameObject.SetActive(true);
        obj.transform.position = FirePointTransform.position;
        var bullet = obj.GetComponent<BulletBase>();
        bullet.Setup(_bulletMoveDierction);
    }
}
