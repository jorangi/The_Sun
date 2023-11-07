using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SaintCrown : Relic
{
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        GameManager.Inst.ShowTooltip("ȹ��� ������ ������ 1�� Ÿ���� ������ �����մϴ�.");
    }
    public override void RunAbility()
    {
        base.RunAbility();
        Tiles tiles = GameManager.Inst.tiles;
        Tile t;
        bool a = false;
        foreach(Tile tile in tiles.tiles)
        {
            if(tile.mainReligion.religionType == ReligionType.none)
            {
                a = true;
                break;
            }
        }
        if(a)
        {
            do
            {
                t = tiles.tiles[Random.Range(0, tiles.height), Random.Range(0, tiles.width)];
            } while (t.mainReligion.religionType != ReligionType.none);

            GameManager.Inst.tiles.AddTile(owner, t);
        }
    }
}
