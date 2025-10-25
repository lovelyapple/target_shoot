using R3;
using UnityEngine;

public class InputController : MonoSingletoneBase<InputController>
{
    private readonly Subject<Unit> _onInputFireSubject = new Subject<Unit>();
    public Observable<Unit> OnInputFireObservable() => _onInputFireSubject;

    private readonly Subject<int> _onInputMoveHorizentalSubject = new Subject<int>();
    public Observable<int> OnInputMoveHorizentalObservable() => _onInputMoveHorizentalSubject;
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _onInputFireSubject.OnNext(Unit.Default);
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            _onInputMoveHorizentalSubject.OnNext(-1);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            _onInputMoveHorizentalSubject.OnNext(1);
        }
        else
        {
            _onInputMoveHorizentalSubject.OnNext(0);
        }
    }
}
