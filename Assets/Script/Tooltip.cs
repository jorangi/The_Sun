using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
    public RectTransform rect;
    public TMPro.TextMeshProUGUI text;
    private string _text;
    public string _Text
    {
        get => _text;
        set
        {
            text.text = string.Empty;
            _text = value;
            if (value == string.Empty)
            {
                gameObject.SetActive(false);
            }
            text.text = value;
        }
    }
    private void Update()
    {
        rect.sizeDelta = text.rectTransform.sizeDelta + new Vector2(20, 20);
        if (Input.mousePosition.x + rect.sizeDelta.x / 2 > Screen.width)
        {
            rect.anchoredPosition = new Vector2(Input.mousePosition.x - Screen.width / 2 - rect.sizeDelta.x / 2, Input.mousePosition.y - Screen.height / 2 - 60);
        }
        else if(Input.mousePosition.x - rect.sizeDelta.x / 2 < 0)
        {
            rect.anchoredPosition = new Vector2(Input.mousePosition.x - Screen.width / 2 + rect.sizeDelta.x / 2, Input.mousePosition.y - Screen.height / 2 - 60);
        }
        else
        {
            rect.anchoredPosition = new Vector2(Input.mousePosition.x - Screen.width / 2, Input.mousePosition.y - Screen.height / 2 - 60);
        }
    }
}
