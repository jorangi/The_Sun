using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TileMenuBtn : MonoBehaviour,IPointerEnterHandler, IPointerExitHandler
{
    public Button btn;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (transform.GetSiblingIndex() != transform.parent.childCount - 1)
        {
            GameManager.Inst.tiles.ShowTooltip(transform.GetSiblingIndex() + 1);
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (transform.GetSiblingIndex() != transform.parent.childCount - 1)
        {
            GameManager.Inst.tiles.ShowTooltip(0);
        }
    }
}
