using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
    public RectTransform rect;
    public TMPro.TextMeshProUGUI textPlace;
    private string text = string.Empty;
    public string Text
    {
        get => text;
        set
        {
            textPlace.text = string.Empty;
            text = value;
            gameObject.SetActive(value != string.Empty);
            textPlace.text = value;
        }
    }
    private void Awake()
    {
        Text = string.Empty;
    }
    private void Update()
    {
        int width = Screen.width;
        int height = Screen.height;
        Vector2 p = Input.mousePosition;
        rect.sizeDelta = textPlace.rectTransform.sizeDelta + new Vector2(20, 20);
        if (p.x + rect.sizeDelta.x / 2 > width)
        {
            rect.anchoredPosition = new Vector2(p.x - width / 2 - rect.sizeDelta.x / 2, p.y - height / 2 - 60);
        }
        else if(p.x - rect.sizeDelta.x / 2 < 0)
        {
            rect.anchoredPosition = new Vector2(p.x - width / 2 + rect.sizeDelta.x / 2, p.y - height / 2 - 60);
        }
        else
        {
            rect.anchoredPosition = new Vector2(p.x - width / 2, p.y - height / 2 - 60);
        }
    }
}
