using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BotaniBracelet : Relic
{
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        GameManager.Inst.ShowTooltip("�ڵ� ������ ������ 20% �����մϴ�.");
    }
    public override void RunAbility()
    {
        base.RunAbility();
        owner.AutoOccupy -= 20;
    }
}
