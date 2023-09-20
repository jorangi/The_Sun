using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NoWitherFlower : Relic
{
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        GameManager.Inst.ShowTooltip("모든 타일의 점령확률이 5% 상승합니다.");
    }
}
