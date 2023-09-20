using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OldMap : Relic
{
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        GameManager.Inst.ShowTooltip("���ƿ��� 5��° �Ͽ� ���� ��� ���� �����ϴ�.");
    }
    public override void RunAbility()
    {
        base.RunAbility();
        foreach (Tile tile in GameManager.Inst.tiles.tiles)
        {
            tile.SetFog(owner, false);
        }
    }
    public void HideMap()
    {
        foreach (Tile tile in GameManager.Inst.tiles.tiles)
        {
            tile.SetFog(owner, true);
        }
    }
}
