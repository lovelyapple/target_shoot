using UnityEngine;
using UnityEngine.UI;

public class UISingleNumberView : MonoBehaviour
{
    [SerializeField] Image BackGroundImage;
    [SerializeField] Image NumberImage;
    public void Setup(Sprite backGround, Sprite number)
    {
        BackGroundImage.sprite = backGround;
        NumberImage.sprite = number;
    }
}
