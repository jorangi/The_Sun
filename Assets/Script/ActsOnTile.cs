using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActsOnTile
{
    private TileData data;
    public TileData Data
    {
        get => data;
        set => data = value;
    }
    public void FirstContact(Religion rel)
    {
        if (!data.ReligionsDataInTile.ContainsKey(rel.religionType))
        {
            data.ReligionsDataInTile.Add(rel.religionType, new ReligionDataInTile(rel.religionType, data.gameObject));
            data.ReligionsDataInTile[rel.religionType].Stability = 100;
        }
    }
    public void Evangelize(Religion rel)
    {
        FirstContact(rel);
        data.ReligionsDataInTile[rel.religionType].Influence += rel.Ideal - rel.Dysentery;
        data.ReligionsDataInTile[rel.religionType].Stability++;
    }
    public void Charity(Religion rel, int coin)
    {
        FirstContact(rel);
        int inf = Mathf.RoundToInt((float)coin / data.Production.GoldProduct * coin / data.Production.totalProductedGold * (data.Production.Status == false ? 2 : 1));
        data.ReligionsDataInTile[rel.religionType].Influence += inf;
        data.ReligionsDataInTile[rel.religionType].Stability++;
    }
}