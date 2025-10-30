using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINumberView : MonoBehaviour
{
    [SerializeField] List<Sprite> BackGroundImages;
    [SerializeField] List<Sprite> NumberImages;
    [SerializeField] List<UISingleNumberView> Numbers;
    [SerializeField] int Number;
    [ContextMenu("Exe")]
    public void Set()
    {
        SetNumber(Number);
    }
    public void SetNumber(int number)
    {
        string numStr = number.ToString("D4"); // 4桁固定（例：0045）

        for (int i = 0; i < Numbers.Count; i++)
        {
            int index = numStr[i] - '0';

            if (i < numStr.Length)
            {
                Numbers[i].gameObject.SetActive(true);
                var backGroundImage = BackGroundImages[index];
                var numberImage = NumberImages[index];
                Numbers[i].Setup(backGroundImage, numberImage);
            }
            else
                Numbers[i].gameObject.SetActive(false);
        }
    }
}
