using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MiracleDice : Relic
{
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        GameManager.Inst.ShowTooltip("�ൿ�� �ֻ������� ���� ��ġ�� ���� ���� Ȯ���� ����մϴ�.");
    }
}
