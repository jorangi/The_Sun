using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MiracleDice : Relic
{
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        GameManager.Inst.ShowTooltip("행동력 주사위에서 높은 수치의 눈이 나올 확률이 상승합니다.");
    }
}
