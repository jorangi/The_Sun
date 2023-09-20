using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OldMap : Relic
{
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        GameManager.Inst.ShowTooltip("돌아오는 5번째 턴에 한해 모든 맵을 밝힙니다.");
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
