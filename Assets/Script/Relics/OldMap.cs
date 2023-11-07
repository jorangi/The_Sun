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
        foreach (TileData tileDatas in GameManager.Inst.tiles.tileDatas)
        {
            //tile.SetFog(owner, false);
        }
    }
    public void HideMap()
    {
        foreach (TileData tileDatas in GameManager.Inst.tiles.tileDatas)
        {
            //tile.SetFog(owner, true);
        }
    }
}
