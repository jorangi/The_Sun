using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GodsMirror : Relic
{
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        GameManager.Inst.ShowTooltip("�⵿���� 2 ����մϴ�.");
    }
    public override void RunAbility()
    {
        base.RunAbility();
        owner.TorchRange += 2;
    }
}
