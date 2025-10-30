using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UINumberView : MonoBehaviour
{
    [SerializeField] List<Sprite> BackGroundImages;
    [SerializeField] List<Sprite> NumberImages;
    [SerializeField] List<UISingleNumberView> Numbers;
    [SerializeField] int Number;
    private const float VerticalBias = 0.2f;
    [ContextMenu("Exe")]
    public void Set()
    {
        SetNumber(Number);
    }
    [SerializeField] private float _moveSpeed = 10f;
    private Vector3 _direction;
    public void SetNumber(int number)
    {
        string numStr = number.ToString("D4"); // 4桁固定（例：0045）

        for (int i = 0; i < Numbers.Count; i++)
        {
            int index = numStr[i] - '0';

            if (i < numStr.Length)
            {
                var backGroundImage = BackGroundImages[index];
                var numberImage = NumberImages[index];
                Numbers[i].Setup(backGroundImage, numberImage);
            }
        }

        // 0〜360度のランダム方向を作る
        float angle = Random.Range(0f, 360f);
        _direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

        // 上下の動きを弱める（verticalBiasをかける）
        _direction.y *= VerticalBias;

        // ベクトルを正規化して速度を一定にする
        _direction.Normalize();
    }
    public void Update()
    {
        // RectTransformを動かす
        RectTransform rect = GetComponent<RectTransform>();
        rect.anchoredPosition += (Vector2)(_direction * _moveSpeed * Time.deltaTime);
    }
    public void OnAnimPlayFinished()
    {
        Destroy(this.gameObject);
    }
}
