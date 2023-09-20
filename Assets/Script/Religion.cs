using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Religion
{
    private int coin;
    public int Coin
    {
        get => coin;
        set
        {
            coin = value;
            if(religion == GameManager.Inst.tiles.playerRel)
            {
                GameManager.Inst.tiles.coinText.text = value.ToString();
            }
        }
    }
    private int sunlight;
    public int Sunlight
    {
        get => sunlight;
        set
        {
            sunlight = value;
            if (religion == GameManager.Inst.tiles.playerRel)
            {
                GameManager.Inst.tiles.sunlightText.text = value.ToString();
            }
        }
    }
    public int totalCoin, avgCoin, totalSunlight, avgSunlight;
    public bool[,] Tech = new bool[6, 6];
    private int[,] techValue = new int[6, 6];
    public int[,] TechValue
    {
        get => techValue;
        set
        {
            List<Vector2Int> arrdiff = new();
            for(int i = 0; i < 6; i++)
            {
                for(int j = 0; j < 6; j++)
                {
                    if(value[i, j] != techValue[i, j])
                    {
                        arrdiff.Add(new(i, j));
                    }
                }
            }
            //foreach(Vector2Int arr in arrdiff)
            //{
            //    if (arr.x == 0 && arr.y == 1)
            //    {
            //        ideal -= techValue[arr.x, arr.y];
            //        ideal += value[arr.x, arr.y];
            //    }
            //}
            techValue = value;
        }
    }
    public Vector2Int RecentTech = new(-1, -1);
    public int TechPoint;
    public int evangelizeValue;
    private int logic;
    public int Logic
    {
        get => logic;
        set
        {
            logic = value;
            if(religion == GameManager.Inst.tiles.playerRel)
            {
                GameManager.Inst.tiles.DetailUI.transform.Find("Logic").GetComponent<TextMeshProUGUI>().text = $"논리 : {value}";
            }
        }
    }
    private int ideal;
    public int Ideal
    {
        get => ideal;
        set
        {
            ideal = value;
            if (religion == GameManager.Inst.tiles.playerRel)
            {
                GameManager.Inst.tiles.DetailUI.transform.Find("Ideal").GetComponent<TextMeshProUGUI>().text = $"이상 : {value}";
            }
        }
    }
    private int dysentery;
    public int Dysentery
    {
        get => dysentery;
        set
        {
            dysentery = value;
            if (religion == GameManager.Inst.tiles.playerRel)
            {
                GameManager.Inst.tiles.DetailUI.transform.Find("Dysentery").GetComponent<TextMeshProUGUI>().text = $"이질 : {value}";
            }
        }
    }
    public int AutoOccupy;
    public Tile SacredPlace;
    public List<HolyRelic> relics = new();
    public List<Miracle> miracle = new();
    public Religions religion;
    public List<Tile> GetTiles = new();
    public Color religionColor;
    public Sprite symbol;
    private int torchRange;
    public int TorchRange
    {
        get => torchRange;
        set
        {
            torchRange = value;
            foreach (Tile tile in GetTiles)
            {
                tile.SetFog(this);
            }
        }
    }
    public bool isSnakeVenom;
    public List<Tile> knownRelicTile = new();
    public List<Tile> TorchedTile = new();
}
