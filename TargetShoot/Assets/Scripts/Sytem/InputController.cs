using R3;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoSingletoneBase<InputController>
{
    private readonly Subject<Unit> _onInputFireSubject = new Subject<Unit>();
    public Observable<Unit> OnInputFireObservable() => _onInputFireSubject;

    private readonly Subject<int> _onInputMoveHorizentalSubject = new Subject<int>();
    public Observable<int> OnInputMoveHorizentalObservable() => _onInputMoveHorizentalSubject;
    private void Awake()
    {
        Initialize(this);
    }
    public void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            _onInputFireSubject.OnNext(Unit.Default);
        }

        if (Keyboard.current.leftArrowKey.isPressed)
        {
            _onInputMoveHorizentalSubject.OnNext(-1);
        }
        else if (Keyboard.current.rightArrowKey.isPressed)
        {
            _onInputMoveHorizentalSubject.OnNext(1);
        }
        else
        {
            _onInputMoveHorizentalSubject.OnNext(0);
        }
    }
}
