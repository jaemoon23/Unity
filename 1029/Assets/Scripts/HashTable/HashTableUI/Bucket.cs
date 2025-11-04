using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Bucket : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image image;

    public void UpdateUI(string value)
    {
        // 버켓에 리스트가 생기면 이미지 색상 녹색, 텍스트 값 변경, 리스트 크기만큼 텍스트 늘리기, 이미지 텍스트 크기만큼 늘리기

        if (!string.IsNullOrEmpty(value))
        {
            image.color = Color.green;
        }
        else
        {
            image.color = Color.white;
        }

        text.text = value;
    }

    public string GetCurrentText()
    {
        return text.text;
    }
}
