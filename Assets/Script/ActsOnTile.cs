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
        data.SetReligionInfluence(rel, Tiles.calc.Evangelize(rel, data));
        ui.ShowTileMenu();
    }
    public void Charity(Religion rel, int coin)
    {
        FirstContact(rel);
        data.SetReligionInfluence(rel, Tiles.calc.Charity(rel, data, coin));
        rel.assets.Coin -= coin;
        ui.ShowCharityMenu();
    }
    public void Occupy(Religion rel)
    {
        int occ = Tiles.calc.Occupy(rel, data);
        if(Random.Range(0, 101) <= occ)
        {
            rel.AddTile(data);
        }
        else
        {
            Debug.Log("점령실패");
        }
    }
}