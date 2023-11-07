using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class TechButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void TechSelect()
    {
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        GameManager.Inst.ShowTooltip("");
    }
}
