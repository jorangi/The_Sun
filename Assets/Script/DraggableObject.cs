using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableObject : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    RectTransform rect;
    Vector2 startObjectPos, beginMousePos;
    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        rect.anchorMin = new(0, 1); rect.anchorMax = new(0, 1); rect.pivot = new(0, 1);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        startObjectPos = rect.anchoredPosition;
        beginMousePos = eventData.position;
    }
    public void OnDrag(PointerEventData eventData)
    {
        rect.anchoredPosition = startObjectPos + (eventData.position - beginMousePos);
    }
}
