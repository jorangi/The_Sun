using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ReligionDataInTile
{
    private ReligionType religionType;
    private SpriteRenderer fogSprite;
    private int dysentery;
    public int Dysentery
    {
        get => dysentery;
        set => dysentery = value;
    }
    private int influence;
    public int Influence
    {
        get => influence;
        set
        {
            value = Mathf.Clamp(value, 0, 100);
            influence = value;
        }
    }
    private int stability;
    public int Stability
    {
        get => stability;
        set
        {
            value = Mathf.Clamp(value, 0, 100);
            stability = value;
        }
    }
    private int autoOcc;
    public int AutoOcc
    {
        get => autoOcc;
        set => autoOcc = value;
    }
    private bool serveidRel;
    public bool ServeidRel
    {
        get => serveidRel;
        set => serveidRel = value;
    }
    private bool unfogging;
    public bool Unfogging
    {
        get => unfogging;
        set
        {
            unfogging = value;
            fogSprite.enabled = !value;
        }
    }
    public ReligionDataInTile(ReligionType religionType, GameObject obj)
    {
        this.religionType = religionType;
        fogSprite = obj.transform.Find("Fog").GetComponent<SpriteRenderer>();
    }
}
public class TileProduction
{
    private SpriteRenderer statusSprite;
    private TextMeshPro goldProductText, sunlightProductText;
    private int goldProduct;
    public int GoldProduct
    {
        get => goldProduct;
        set
        {
            goldProduct = value;
            goldProductText.text = $"{value}";
        }
    }
    private int sunlightProduct;
    public int SunlightProduct
    {
        get => sunlightProduct;
        set
        {
            sunlightProduct = value;
            sunlightProductText.text = $"{value}";
        }
    }
    public int totalProductedGold, totalProductedSunlight;

    private bool? status;//null : normal, true : good, false : bad
    public bool? Status
    {
        get => status;
        set
        {
            status = value;
            switch (value)
            {
                case true:
                    statusSprite.color = Color.green; break;
                case false:
                    statusSprite.color = Color.red; break;
                default:
                    statusSprite.color = Color.white; break;

            }
        }
    }
    public TileProduction(GameObject obj)
    {
        goldProductText = obj.transform.Find("Coin").GetComponent<TextMeshPro>();
        sunlightProductText = obj.transform.Find("Sunlight").GetComponent<TextMeshPro>();
        statusSprite = obj.GetComponent<SpriteRenderer>();
    }
    public void Product()
    {
        totalProductedGold += goldProduct;
        totalProductedSunlight += sunlightProduct;
    }
}
public class OccupyTile
{
    private SpriteRenderer renderer;
    public OccupyTile(SpriteRenderer TileRenderer)
    {
        renderer = TileRenderer;
    }
    public void DisplayOccupiable(TileData data)
    {
        if (data.SettedRel == Tiles.player.religionType)
        {
            renderer.color = Color.white;
            return;
        }
        int den = 0;
        int num = 0;
        int max = 0;
        foreach(KeyValuePair<ReligionType, ReligionDataInTile> d in data.ReligionsDataInTile)
        {
            max = Mathf.Max(max, d.Value.Influence);
            if(d.Key == Tiles.player.religionType)
            {
                num = d.Value.Influence;
            }
            else
            {
                den += d.Value.Influence;
            }
        }
        den += max;

        float r = den == 0 ? 1 : num / den;
        renderer.color = GameManager.Inst.OccLv.Evaluate(r);
        //renderer.color = GameManager.Inst.OccLv.Evaluate(Random.Range(0.0f, 1.0f));
    }
}
public class TileData
{
    public OccupyTile occpy;
    public TileData[] nearTile = new TileData[4];//EWSN
    public TileProduction Production;
    public GameObject gameObject;
    private SpriteRenderer symbolSprite, sacredSymbol, ExistableRelic;
    private ReligionType settedRel;
    public ReligionType SettedRel
    {
        get => settedRel;
        set => settedRel = value;
    }
    private Vector2Int pos;
    public Vector2Int POS
    {
        get => pos;
        set => pos = value;
    }
    private bool sacredPlace;
    public bool SacredPlace
    {
        get => sacredPlace;
        set
        {
            sacredPlace = value;
            sacredSymbol.gameObject.SetActive(value);
        }
    }
    public Dictionary<ReligionType, ReligionDataInTile> ReligionsDataInTile = new();
    public TileData(Vector2Int pos, GameObject obj)
    {
        POS = pos;
        gameObject = obj;
        symbolSprite = obj.transform.Find("Symbol").GetComponent<SpriteRenderer>();
        sacredSymbol = obj.transform.Find("Sacred").GetComponent<SpriteRenderer>();
        occpy = new OccupyTile(obj.transform.Find("InnerSquare").GetComponent<SpriteRenderer>());
        ExistableRelic = obj.transform.Find("ExistableRelic").GetComponent<SpriteRenderer>();
        Production = new(obj);
        ProductionChange();
    }
    public void Subjugate(ReligionType religionType, Sprite symbol)
    {
        SettedRel = religionType;
        symbolSprite.sprite = symbol;
    }
    public void ProductionChange()
    {
        int r = Random.Range(0, 101);
        if (r >= 50)
        {
            Production.GoldProduct = 1;
            Production.SunlightProduct = 1;
        }
        else
        {
            Production.GoldProduct = 0;
            Production.SunlightProduct = 0;
        }
    }
}