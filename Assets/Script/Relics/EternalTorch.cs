using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EternalTorch : Relic
{
    public Tile tile;
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        GameManager.Inst.ShowTooltip("È¹µæ½Ã ÁöÁ¤ÇÑ ±¸¿ªÀ» Ç×»ó ¹àÈü´Ï´Ù.");
    }
    public override void RunAbility()
    {
        base.RunAbility();

        Vector2Int Pos = tile.Pos;

        for (int i = Mathf.Max(0, Pos.y - owner.TorchRange); i <= Mathf.Min(GameManager.Inst.tiles.height - 1, Pos.y + owner.TorchRange); i++)
        {
            for (int j = Mathf.Max(0, Pos.x - owner.TorchRange); j <= Mathf.Min(GameManager.Inst.tiles.width - 1, Pos.x + owner.TorchRange); j++)
            {
                if (Mathf.Abs(Pos.y - i) + Mathf.Abs(Pos.x - j) <= owner.TorchRange)
                {
                    GameManager.Inst.tiles.tiles[i, j].SetFog(owner, false);
                }
            }
        }
    }
}
