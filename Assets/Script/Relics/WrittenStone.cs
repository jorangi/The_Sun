using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WrittenStone : Relic
{
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        GameManager.Inst.ShowTooltip("������ ��η� �����ϴ� ������� 1 �����մϴ�.");
    }
}
