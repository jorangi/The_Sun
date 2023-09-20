using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TileInfoTab : MonoBehaviour
{
    public TextMeshProUGUI Coord, Influence, Stability, Percentage;

    public void SetUp(Tile tile)
    {
        float max = 0;
        Coord.text = $"{tile.Pos.x}, {tile.Pos.y}";
        Influence.text = $"[영향력]<br>";
        Stability.text = $"[안정]<br>";
        foreach (KeyValuePair<Religions, int> val in tile.influence)
        {
            Influence.text += $"({val.Key}, {val.Value}) ";
            if (val.Key != GameManager.Inst.tiles.playerRel)
                max += val.Value;
        }
        foreach (KeyValuePair<Religions, int> val in tile.stability)
        {
            Stability.text += $"({val.Key}, {val.Value}) ";
        }
        if(tile.influence.ContainsKey(GameManager.Inst.tiles.playerRel))
        {
            float p = 0;
            if(tile.Sacred.gameObject.activeSelf)
            {
                p =(float)tile.influence[GameManager.Inst.tiles.playerRel] - tile.influence[tile.mainReligion.religion] / GameManager.Inst.tiles.MaxInfluence * 100;
            }
            else
            {
                p = (float)tile.influence[GameManager.Inst.tiles.playerRel] / (GameManager.Inst.tiles.MaxInfluence + max) * 100;
            }
            Percentage.text = $"점령 확률 : {p}%";
        }
        else
        {
            Percentage.text = $"점령 확률 : {0}%";
        }
        gameObject.SetActive(true);
    }
}
