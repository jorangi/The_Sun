using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WrittenStone : Relic
{
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        GameManager.Inst.ShowTooltip("전도와 기부로 증가하는 영향력이 1 증가합니다.");
    }
}
