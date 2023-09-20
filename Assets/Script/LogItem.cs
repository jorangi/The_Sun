using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LogItem : MonoBehaviour
{
    public RectTransform rect;
    public CanvasGroup canvasGroup;
    public Button closeBtn;
    private float timer = 2.0f;
    public Coroutine co;
    private void Update()
    {
        if(timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            Close();
        }
    }
    public void LogText(string text)
    {
        GetComponent<TextMeshProUGUI>().text = text;
        transform.Find("Log").GetComponent<TextMeshProUGUI>().text = text;
    }
    public void Close()
    {
        if (co != null)
            return;
        co = StartCoroutine(CloseLog());
    }
    public IEnumerator CloseLog()
    {
        closeBtn.interactable = false;

        while(canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
}
