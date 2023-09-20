using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BotaniBracelet : Relic
{
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        GameManager.Inst.ShowTooltip("자동 점령의 조건이 20% 감소합니다.");
    }
    public override void RunAbility()
    {
        base.RunAbility();
        owner.AutoOccupy -= 20;
    }
}
