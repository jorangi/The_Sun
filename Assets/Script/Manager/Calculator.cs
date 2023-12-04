using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calculator
{
    public (int, int) Evangelize(Religion rel, TileData tile)
    {
        ReligionDataInTile d = tile.ReligionsDataInTile[rel.religionType];
        return (Mathf.Min(GameManager.Inst.MaxInfluence, rel.Ideal - rel.Dysentery), d.Influence == 0 && d.Stability == 0 ? 100 : d.Stability + 1);
    }
    public (int, int) Charity(Religion rel, TileData tile, int coin)
    {
        ReligionDataInTile d = tile.ReligionsDataInTile[rel.religionType];
        int inf = Mathf.RoundToInt((float)coin / Mathf.Max(1, tile.Production.GoldProduct) * coin / Mathf.Max(1, tile.Production.CharitiedGold) * (tile.Production.Status == false ? 2 : 1));
        return (Mathf.Min(inf, GameManager.Inst.MaxInfluence), d.Influence == 0 && d.Stability == 0 ? 100 : d.Stability + 1);
    }
    public int Occupy(Religion rel, TileData tile)
    {
        if (tile.SettedRel == rel.religionType) return -1;
        int occ = 0;
        int max = 0, sum = 0;
        foreach(KeyValuePair<ReligionType, ReligionDataInTile> p in tile.ReligionsDataInTile)
        {
            max = Mathf.Max(p.Value.Influence, max);
            if (p.Key != rel.religionType)
            {
                sum += p.Value.Influence;
            }
        }
        if(tile.SacredPlace)
        {
            try { occ = 100 * (tile.ReligionsDataInTile[Tiles.player.religionType].Influence - tile.ReligionsDataInTile[tile.SettedRel].Influence) / max; }
            catch {
            }
        }
        else
        {
            try { occ = 100 * tile.ReligionsDataInTile[Tiles.player.religionType].Influence / (sum + max); }
            catch
            {
            }
        }
        return occ;
    }
}