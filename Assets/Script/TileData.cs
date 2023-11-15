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
        set => influence = value;
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
            fogSprite.enabled = religionType != ReligionType.TheSun || !value;
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
    private int totalProductedGold, totalProductedSunlight;

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
    }
    public void Product()
    {
        totalProductedGold += goldProduct;
        totalProductedSunlight += sunlightProduct;
    }
}
public class TileData
{
    public TileData[] nearTile = new TileData[4];//EWSN
    public TileProduction Production;
    public GameObject gameObject;
    private SpriteRenderer symbolSprite, sacredSymbol;
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
        Production = new(obj);
    }
    public void Subjugate(ReligionType religionType, Sprite symbol)
    {
        SettedRel = religionType;
        symbolSprite.sprite = symbol;
    }
}