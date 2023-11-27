using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TileMenuBtn : MonoBehaviour,IPointerEnterHandler, IPointerExitHandler
{
    public Tiles tiles;
    public Button btn;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (transform.GetSiblingIndex() != transform.parent.childCount - 1)
        {
            tiles.ShowToolTip(transform.GetSiblingIndex() + 1);
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (transform.GetSiblingIndex() != transform.parent.childCount - 1)
        {
            tiles.ShowToolTip(0);
        }
    }
}
