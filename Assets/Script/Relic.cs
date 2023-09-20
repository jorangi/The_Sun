using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Relic : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Religion owner;
    public Image relicSymbol;
    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        relicSymbol.color = new Color(0.9f, 0.9f, 0.9f);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        relicSymbol.color = Color.white;
        GameManager.Inst.ShowTooltip("");
    }
    public virtual void RunAbility()
    {

    }
}
