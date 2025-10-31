using System;
using R3;
using UnityEngine;
using UnityEngine.UI;

public class UIInputPanel : MonoBehaviour
{
    [SerializeField] private UIButtonPressHandler MoveLeftHandler;
    [SerializeField] private UIButtonPressHandler MoveRightHandler;

    [SerializeField] private Button FireButton;
    
    public Observable<bool> IsPressingMoveLeftObservable() => MoveLeftHandler.IsPressingObservable();
    public Observable<bool> IsPressingMoveRightObservable() => MoveRightHandler.IsPressingObservable();

    public Observable<Unit> OnClickFireObservable() => FireButton.OnClickAsObservable();

    private void Awake()
    {
        InputController.Instance.RegisterGameInputUI(this);
    }
}
