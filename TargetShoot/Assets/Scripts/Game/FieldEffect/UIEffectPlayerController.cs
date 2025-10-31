using R3;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class UIEffectPlayerController : MonoBehaviour
{
    [SerializeField] UINumberView NumberViewPrefab;
    [SerializeField] Camera UICamera;
    [SerializeField] Canvas RootCanvas;
    private void Awake()
    {
        MatchEventDispatcher.Instance.ScoreUpdateObservable()
        .Subscribe(PlayTargetHitScoreEffect)
        .AddTo(this);
    }

    private void PlayTargetHitScoreEffect(ScoreInfo scoreInfo)
    {
        if(scoreInfo.TargetHitPos == Vector3.zero)
        {
            return;
        }

        var newFx = Instantiate(NumberViewPrefab.gameObject, transform).GetComponent<UINumberView>();

        Vector3 viewportPos = Camera.main.WorldToViewportPoint(scoreInfo.TargetHitPos);
        Vector3 screenPos = UICamera.ViewportToScreenPoint(viewportPos);

        Vector2 uiPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            RootCanvas.transform as RectTransform,
            screenPos,
            UICamera,
            out uiPos
        );

        newFx.transform.localPosition = uiPos;
        newFx.SetNumber(scoreInfo.Diff);
    }
}
