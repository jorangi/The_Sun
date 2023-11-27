using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActsOnTile
{
    public UIManager ui;
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
        }
    }
    public void Evangelize(Religion rel)
    {
        FirstContact(rel);
        ReligionDataInTile d = data.ReligionsDataInTile[rel.religionType];
        d.Stability = d.Influence == 0 && d.Stability == 0 ? 100 : d.Stability+1;
        d.Influence += rel.Ideal - rel.Dysentery;
        //ui.ShowTileInfo(data);
    }
    public void Charity(Religion rel, int coin)
    {
        FirstContact(rel);
        ReligionDataInTile d = data.ReligionsDataInTile[rel.religionType];
        int inf = Mathf.RoundToInt((float)coin / data.Production.GoldProduct * coin / data.Production.totalProductedGold * (data.Production.Status == false ? 2 : 1));
        d.Stability = d.Influence == 0 && d.Stability == 0 ? 100 : d.Stability + 1;
        d.Influence += inf;
    }
}