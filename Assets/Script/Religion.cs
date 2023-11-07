using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TheSun : Religion
{
    public TheSun(ReligionType relT, Vector2Int startPlace):base(relT, startPlace)
    {
        this.religionType = relT;
        this.sacredPlace = startPlace;
        miracles.Add(Miracle.RisingSun);
    }
    public override void UseMiracle(Miracle miralce)
    {

    }
}
public abstract class Tech
{
    public Tech(string name, string desc)
    {
        this.name = name;
        this.desc = desc;
    }
    public abstract void RunAbility();
    private string _name;
    public string name { get => _name; set => _name = value; }
    private string _desc;
    public string desc { get => _desc; set => _desc = value; }
    public bool isInvested;
    public bool IsInvested { get => isInvested; set => isInvested = value; }
}
public class TechList
{
    public class Tech0_0:Tech
    {
        public Tech0_0(string name ="테크0_0", string desc = "해당 테크는 0_0번입니다."):base(name, desc)
        {
            RunAbility();
        }
        public override void RunAbility() { }
    }
    public Tech[,] Techs = new Tech[6, 6];
    public TechList()
    {
    }
}
public class ReligionAssets
{
    private UIManager ui;
    private int coin = 10;
    public int Coin
    {
        get => coin;
        set
        {
            coin = value;
            ui.coinUI.ChangeUI(value);
        }
    }
    private int sunlight = 0;
    public int Sunlight
    {
        get => sunlight;
        set
        {
            sunlight = value;
            ui.sunlightUI.ChangeUI(value);
        }
    }
    public int totalCoin, totalSunlight;
    public ReligionAssets(UIManager ui)
    {
        this.ui = ui;
    }
}
public abstract class Religion
{
    [SerializeField]
    private UIManager ui;
    protected Vector2Int sacredPlace;
    public List<Vector2Int> TorchedTile;
    public List<TileData> HasTiles = new();

    public ReligionAssets assets;
    public TechList techList;
    private int techPoint;
    public int TechPoint
    {
        get => techPoint;
        set => techPoint = value;
    }
    public int evangelizeValue;
    private int logic = 5;
    public int Logic
    {
        get => logic;
        set => logic = value;
    }
    private int ideal = 5;
    public int Ideal
    {
        get => ideal;
        set => ideal = value;
    }
    private int dysentery = 0;
    public int Dysentery
    {
        get => dysentery;
        set => dysentery = value;
    }
    private int autoOccupy = 95;
    public int AutoOccupy
    {
        get => autoOccupy;
        set => autoOccupy = value;
    }

    public List<HolyRelic> relics = new();
    public List<Miracle> miracles = new();
    public ReligionType religionType;
    //public List<Tile> GetTiles = new();
    public Color religionColor;
    public Sprite symbol;
    private int torchRange;
    public int TorchRange
    {
        get => torchRange;
        set
        {
            torchRange = value;
            foreach (TileData tileData in HasTiles)
            {
                tileData.ReligionsDataInTile[religionType].Unfogging = true;
            }
        }
    }
    public bool isSnakeVenom;
    //public List<Tile> knownRelicTile = new();
    //public List<Tile> TempTorchedTile = new();

    public bool CheckContainShowedTile(TileData tileData) => TorchedTile.Contains(tileData.POS);
    public Religion(ReligionType relT, Vector2Int startPlace)
    {
        this.religionType = relT;
        this.sacredPlace = startPlace;
    }
    public abstract void UseMiracle(Miracle miralce);
    public void GetMiracle(Miracle miracle)
    {
        miracles.Add(miracle);
    }
}
