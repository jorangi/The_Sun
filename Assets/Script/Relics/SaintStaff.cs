using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SaintStaff : Relic
{
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        GameManager.Inst.ShowTooltip("������ Ÿ�Ͽ��� �� �ϸ��� 30%�� Ȯ���� �޻��� ȹ���մϴ�.");
    }
    public override void RunAbility()
    {
        base.RunAbility();
        for(int i = 0; i<owner.HasTiles.Count; i++)
        {
            if(Random.Range(0f, 1f) <= 0.3f)
            {
                owner.assets.Sunlight += 1;
            }
        }
    }
}
