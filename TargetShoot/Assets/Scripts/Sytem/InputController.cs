using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoSingletoneBase<InputController>
{
    private readonly Subject<Unit> _onInputFireSubject = new Subject<Unit>();
    public Observable<Unit> OnInputFireObservable() => _onInputFireSubject;

    private readonly Subject<int> _onInputMoveHorizentalSubject = new Subject<int>();
    public Observable<int> OnInputMoveHorizentalObservable() => _onInputMoveHorizentalSubject;

    private bool _isUIPressingLeft = false;
    private bool _isUIPressingRight = false;
    private bool _isTapFireThisFrame = false;
    private void Awake()
    {
        Initialize(this);
    }

    public void RegisterGameInputUI(UIInputPanel inputPanel)
    {
        inputPanel.IsPressingMoveLeftObservable()
            .Subscribe(x => _isUIPressingLeft = x)
            .AddTo(inputPanel);
        
        inputPanel.IsPressingMoveRightObservable()
            .Subscribe(x => _isUIPressingRight = x)
            .AddTo(inputPanel);

        inputPanel.OnClickFireObservable()
            .Subscribe(_ => _isTapFireThisFrame = true)
            .AddTo(inputPanel);
    }
    public void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame || _isTapFireThisFrame)
        {
            _onInputFireSubject.OnNext(Unit.Default);
        }

        if (_isTapFireThisFrame)
        {
            _isTapFireThisFrame = false;
        }

        if (Keyboard.current.leftArrowKey.isPressed || _isUIPressingLeft)
        {
            _onInputMoveHorizentalSubject.OnNext(-1);
        }
        else if (Keyboard.current.rightArrowKey.isPressed || _isUIPressingRight)
        {
            _onInputMoveHorizentalSubject.OnNext(1);
        }
        else
        {
            _onInputMoveHorizentalSubject.OnNext(0);
        }
    }
}
