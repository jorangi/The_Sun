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
    public int Dysentery { get; set; }
    public int Influence { get; set; }
    public int AutoOcc { get; set; }
    public bool ServeidRel { get; set; }
    public bool unfogging;
    public bool Unfogging
    {
        get => unfogging;
        set
        {
            unfogging = value;
            fogSprite.enabled = (religionType == ReligionType.TheSun) && value;
        }
    }
    public ReligionDataInTile(ReligionType religionType, GameObject obj)
    {
        this.religionType = religionType;
        fogSprite = obj.GetComponent<SpriteRenderer>();
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
        goldProductText = obj.GetComponent<TextMeshPro>();
        sunlightProductText = obj.GetComponent<TextMeshPro>();
    }
    public void Product()
    {
        totalProductedGold += goldProduct;
        totalProductedSunlight += sunlightProduct;
    }
}
public class TileData
{
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
        Production = new(obj);
    }
}
public class Tiles : MonoBehaviour
{
    #region field
    //컴포넌트 관련
    public GameObject TurnObject, tileMenu, miracleMenu, miracleTarget, miracleTargetPrefab, ForegoneFateUI, DetailUI, TechUI, TechButton;
    public GameObject charityMenu, argumentMenu, argumentReligionBtn, ReligionsTile, ReligionsCounters, playerRelics, otherRelics;
    public GameObject[] relicArray;
    public Button evanBtn, charityBtn, arguBtn, serRelBtn, buyRelBtn, occBtn;
    public TextMeshProUGUI coinText, sunlightText;

    //게임 데이터 관련
    public Sprite[] symbols;

    //카메라 관련
    public const int CAMMAX = 5;
    public int CAMMIN = 100;

    //타일맵 관련
    public TileData[,] tileDatas;
    private Vector2 mousePos;
    private bool moveable = false;
    public int width, height;
    public float AvgTotalMadeCoin;
    public GameObject TilePrefab;
    public Tile[,] tiles;
    public Tile setTile;
    public Color goodTile, badTile, enableTile, disableTile, blockedTile;

    //게임 플레이 관련
    private Religion player;
    private Religion bestRel;
    private bool EternalTorchGet = false;
    public Religion miracleTargetRel = null;
    public List<bool> tileSelectable = new();
    private bool autoPlay = false;
    public int baseMaintance;
    private Coroutine aiPlay;
    private WaitForSeconds actDelay = new(0.5f);
    public float baseDysentery;
    public int MaxInfluence = 20;
    public int MaxRelic;
    public int MaxReligion;
    private ReligionType turnRel;
    public Religion TurnRel
    {
        get => turnRel;
        set
        {
            if(value != ReligionType.none)
            {
                TurnObject.GetComponent<Image>().sprite = symbols[System.Array.IndexOf(System.Enum.GetNames(typeof(ReligionType)), value.ToString()) - 1];
            }
            turnRel = value;
        }
    }
    public Dictionary<Miracle, ReligionType> usingMiracle = new();
    public Dictionary<Miracle, int> MiracleTurn = new();
    public Dictionary<ReligionType, Religion> ReligionDic = new();
    public Dictionary<HolyRelic, Tile> holyRelic = new();
    public Dictionary<HolyRelic, GameObject> RelicDic = new();
    public List<HolyRelic> holyRelics = new();
    public Dictionary<ReligionType, int[]> RecoredDir = new();
    public List<Religion> religions = new();
    public int[] MiraclePrice;
    private int turn;
    public int Turn
    {
        get => turn;
        set => turn = value;
    }
    public int MaxTurn = 300;
    private int actableCount;
    public int ActableCount
    {
        get => actableCount;
        set => actableCount = value;
    }
    public int BlockedFate = 0;
    public int FixedFate = 0;
    #endregion

    private void Awake()
    {
        Init();
    }
    private void Update()
    {
        //레이캐스트
        if (tileSelectable.Count == 0 && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 1, 1 << 6);

            if (hit.collider != null)
            {
                int x = (int)hit.transform.position.x;
                int y = (int)hit.transform.position.y;
                TileData t = tileDatas[(int)(height * 0.5f - y), (int)(width * 0.5f + x)];
                ShowTileMenu(t);
            }
        }
        //카메라 조정
        Transform tr = Camera.main.transform;
        float size = Camera.main.orthographicSize;
        if (Input.mouseScrollDelta.y != 0)
        {
            if(Input.mouseScrollDelta.y > 0)
            {
                //Debug.Log("스크롤 올림");
            }
            else
            {
                //Debug.Log("스크롤 내림");
            }
            Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - Input.mouseScrollDelta.y, CAMMAX, CAMMIN);
            size = Camera.main.orthographicSize;
            tr.position += mouse - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            tr.position = new(
                Mathf.Clamp(tr.position.x, -(CAMMIN - size) * (12.5f / 7), (CAMMIN - size) * (12.5f / 7)),
                Mathf.Clamp(tr.position.y, -(CAMMIN - size), CAMMIN - size), -10);
        }
        if(Input.GetMouseButtonDown(1))
        {
            moveable = true;
            mousePos = Input.mousePosition;
        }
        if(Input.GetMouseButtonUp(1))
        {
            moveable = false;
        }
        if (moveable)
        {
            Vector2 vector2 = Camera.main.ScreenToViewportPoint(mousePos - (Vector2)Input.mousePosition);
            tr.Translate((CAMMIN - Camera.main.orthographicSize) * 2 * Time.deltaTime * vector2);
            tr.position = new(
                Mathf.Clamp(tr.position.x, -(CAMMIN - size) * (12.5f / 7), (CAMMIN - size) * (12.5f / 7)),
                Mathf.Clamp(tr.position.y, -(CAMMIN - size), CAMMIN - size), -CAMMIN);
        }
    }
    private void Init()
    {
        MiraclePrice = new int[] { 190, 210, 130, 250, 160, 90, 200, 155, 320, 225, 350, 195, 330 };
        for (int i = 1; i < System.Enum.GetValues(typeof(HolyRelic)).Length; i++)
        {
            RelicDic.Add((HolyRelic)i, relicArray[i - 1]);
        }
        for (int i = 1; i < System.Enum.GetValues(typeof(Miracle)).Length; i++)
        {
            usingMiracle.Add((Miracle)i, ReligionType.none);
            MiracleTurn.Add((Miracle)i, 0);
        }
        TurnObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{1}";
        baseDysentery = 5f;
        width = 9;
        height = 9;
        MaxRelic = 3;
        MaxReligion = 3;

        tileDatas = new TileData[height, width];
        CAMMIN = 18 + (Mathf.Max(width, height) - 30)/2 * 1; 

        Vector2 boxSize = new(width, height);

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                GameObject obj = Instantiate(TilePrefab, transform);
                tileDatas[i, j] = new TileData(new Vector2Int(j, i), obj);
                obj.transform.position = new(-boxSize.x / 2 + j, boxSize.y / 2 - i);
                obj.name = $"{j}|{i}";
            }
        }
        player = ReligionDataSetup(ReligionType.TheSun);
        religions.Add(player);

        //테크 전부 오픈
        //for (int i = 0; i < 5; i++)
        //{
        //    for (int j = 0; j < 5; j++)
        //    {
        //        player.techList.Techs[i, j].isInvested = true;
        //    }
        //}

        ////태양교 셋업
        //playerRel = Religions.TheSun;
        //ReligionDataSetup(Religions.TheSun);
        //TurnObject.GetComponent<Image>().sprite = religions[0].symbol;

        ////for(int i =0; i < 5; i++)
        ////{
        ////    for(int j =0; j < 5; j++)
        ////    {
        ////        ReligionDic[playerRel].Tech[i, j] = true;
        ////    }
        ////}

        ////무작위 종교 셋업
        //for (int i = 0; i<MaxReligion-1; i++)
        //{
        //    Religions rel;
        //    do
        //    {
        //        rel = (Religions)Random.Range(0, System.Enum.GetNames(typeof(Religions)).Length);
        //    } while (religionsOrder.Contains(rel) || rel == Religions.none);
        //    ReligionDataSetup(rel);
        //}

        ////무작위 성유물 셋업
        //HolyRelic relic = HolyRelic.none;
        //for (int i = 0; i < MaxRelic; i++)
        //{
        //    do
        //    {
        //        relic = (HolyRelic)Random.Range(0, System.Enum.GetNames(typeof(HolyRelic)).Length);
        //    } while (holyRelic.ContainsKey(relic) || relic == HolyRelic.none);

        //    Tile tile = null;
        //    do
        //    {
        //        tile = tiles[Random.Range(0, height), Random.Range(0, width)];
        //    } while (tile.relic != HolyRelic.none);
        //    holyRelic.Add(relic, tile);
        //    tile.relic = relic;
        //}
        //Turn = 1;
        //TurnRel = playerRel;
        //MaxTurn = 500;
        //AvgTotalMadeCoin = 0f;
        //ActableCount = RandomActable(ReligionDic[playerRel]);

        //foreach(Religions religions in religionsOrder)
        //{
        //    ReligionDic[religions].TorchRange = 3;
        //}
        //foreach(Tile t in tiles)
        //{
        //    t.NextCondition();
        //}

        ////for(int i =0; i< 10; i++)
        ////{
        ////    AddTile(ReligionDic[playerRel], tiles[0, i]);
        ////}
        ////for(int i =0; i<10; i++)
        ////{
        ////    Tile tempTile = null;
        ////    do
        ////    {
        ////        tempTile = tiles[Random.Range(0, height), Random.Range(0, width)];
        ////    } while (tempTile.mainReligion.religion != Religions.none);
        ////    AddTile(ReligionDic[playerRel], tempTile);
        ////}

        ////ReligionDic[playerRel].miracle.Add(Miracle.SnakeVenom);
        ////ReligionDic[playerRel].miracle.Add(Miracle.CompulsoryExcution);
        ////ReligionDic[playerRel].miracle.Add(Miracle.ForegoneFate);
        ////ReligionDic[playerRel].miracle.Add(Miracle.ConcealedFuture);
        ////ReligionDic[playerRel].miracle.Add(Miracle.PropertyGrowth);
        ////ReligionDic[playerRel].miracle.Add(Miracle.Charisma);
        ////ReligionDic[playerRel].miracle.Add(Miracle.BuildWall);
        //ReligionDic[playerRel].miracle.Add(Miracle.Prophecy);
        ////ReligionDic[playerRel].miracle.Add(Miracle.Judgement);
        ////ReligionDic[playerRel].miracle.Add(Miracle.ForesightDream);
        ////ReligionDic[playerRel].miracle.Add(Miracle.FrozenHeart);
        ////ReligionDic[playerRel].Sunlight = 10000;


        ////GetRelic(ReligionDic[playerRel], HolyRelic.GoldenEye);
        ////GetRelic(ReligionDic[playerRel], HolyRelic.SaintStaff);
        ////GetRelic(ReligionDic[playerRel], HolyRelic.MiracleDice);
        ////GetRelic(ReligionDic[playerRel], HolyRelic.GuardCharm);
        ////GetRelic(ReligionDic[playerRel], HolyRelic.WrittenStone);
        ////GetRelic(ReligionDic[playerRel], HolyRelic.NoWitherFlower);
        ////GetRelic(ReligionDic[playerRel], HolyRelic.SaintCrown);
        ////GetRelic(ReligionDic[playerRel], HolyRelic.OldMap);
        ////GetRelic(ReligionDic[playerRel], HolyRelic.LightBoots);
        ////GetRelic(ReligionDic[playerRel], HolyRelic.EternalTorch);
        ////GetRelic(ReligionDic[playerRel], HolyRelic.BotaniBracelet);
        ////GetRelic(ReligionDic[playerRel], HolyRelic.GodsMirror);
    }
    private Religion ReligionDataSetup(ReligionType religionType)
    {
        TileData tile;
        do
        {
            tile = tileDatas[Random.Range(0, height), Random.Range(0, width)];
        } while (tile.SettedRel != ReligionType.none);

        Religion religion = null;
        switch (religionType)
        {
            case ReligionType.TheSun:
                religion = new TheSun(ReligionType.TheSun, tile.POS);
                religion.GetMiracle(Miracle.RisingSun);
                break;
        }
        tile.ReligionsDataInTile.Add(religionType, new(religionType, tile.gameObject) { Unfogging = true, Influence = MaxInfluence });
        religion.HasTiles.Add(tile);
        AddTile(religion, tile);
        tile.ReligionsDataInTile[religion.religionType].Influence = MaxInfluence;
        ReligionDic.Add(religionType, religion);

        GameObject obj = Instantiate(ReligionsTile, ReligionsCounters.transform);
        obj.name = religion.religionType.ToString();
        obj.transform.GetChild(0).GetComponent<Image>().sprite = religion.symbol;
        obj.GetComponentInChildren<TextMeshProUGUI>().text = $"{1}";

        return religion;
    }
    public void AddTile(Religion religion, TileData tileData)
    {
        tileData.SacredPlace = religion.HasTiles.Count == 0;
        tileData.SettedRel = religion.religionType;
        religion.HasTiles.Add(tileData);
    }
    private void ShowTileMenu(TileData data)
    {
        if (!player.CheckContainShowedTile(data)) return;
        tileMenu.SetActive(true);
        tileSelectable.Add(true);
    }
    public void ShowTooltip(int i)
    {
        switch(i)
        {
            default:
                GameManager.Inst.ShowTooltip("");
                break;
            //case 1:
            //    if (setTile.influence[playerRel] < 20)
            //    {
            //        GameManager.Inst.ShowTooltip($"전도를 하여 <color=#ffff00ff>{0}</color>만큼의 영향력을 높힙니다.");
            //    }
            //    else
            //    {
            //        GameManager.Inst.ShowTooltip($"영향력이 최대입니다. 전도를 통해 안정만을 획득할 수 있습니다.");
            //    }
            //    break;
            //case 2:
            //    if (setTile.influence[playerRel] < 20)
            //    {
            //        GameManager.Inst.ShowTooltip("기부를 하여 영향력을 높힙니다.");
            //    }
            //    else
            //    {
            //        GameManager.Inst.ShowTooltip($"영향력이 최대입니다. 전도를 통해 안정만을 획득할 수 있습니다.");
            //    }
            //    break;
            //case 3:
            //    GameManager.Inst.ShowTooltip("논쟁을 하여 상대방의 영향력을 낮추고 자신의 영향력을 높입니다.");
            //    break;
            //case 4:
            //    if (!setTile.serveidRel[ReligionDic[playerRel]])
            //    {
            //        GameManager.Inst.ShowTooltip("해당 타일에서 성유물을 찾아냅니다.");
            //    }
            //    else
            //    {
            //        GameManager.Inst.ShowTooltip("해당 타일에서 성유물을 찾아냅니다.<br><color=#ff00ffff>해당 타일에서 성유물 조사를 이미 마쳤습니다.</color>");
            //    }
            //    break;
            //case 5:
            //    if (setTile.serveidRel[ReligionDic[playerRel]] && setTile.relic != HolyRelic.none)
            //    {
            //        int RelicPrice = 0;

            //        if (RelicPrice > ReligionDic[playerRel].Coin)
            //        {
            //            GameManager.Inst.ShowTooltip($"코인이 부족하여 성유물을 인수할 수 없습니다.<br>성유물의 가격이{RelicPrice}코인입니다.<br><color=#ff00ffff>{RelicPrice - ReligionDic[playerRel].Coin}만큼의 코인이 부족합니다.</color>");
            //        }
            //        else
            //        {
            //            GameManager.Inst.ShowTooltip($"<color=#ffff00ff>{RelicPrice}</color>코인을 지불하여 발견된 성유물을 인수합니다.");
            //        }
            //    }
            //    else if(!setTile.serveidRel[ReligionDic[playerRel]])
            //    {
            //        GameManager.Inst.ShowTooltip("<color=#ff00ffff>해당 타일에 성유물이 존재하는지 알 수 없습니다.</color>");
            //    }
            //    else
            //    {
            //        GameManager.Inst.ShowTooltip("<color=#ff00ffff>해당 타일에 성유물이 존재하지 않습니다.</color>");
            //    }
            //    break;
            //case 6:
            //    if (setTile.mainReligion != religions[0])
            //    {
            //        if(!setTile.influence.ContainsKey(playerRel))
            //        {
            //            GameManager.Inst.ShowTooltip("<color=#ff00ffff>해당 타일에 영향력이 없습니다.</color>");
            //        }
            //        else
            //        {
            //            if (setTile.Sacred.gameObject.activeSelf)
            //            {
            //                GameManager.Inst.ShowTooltip($"해당 타일은 성지입니다.<br><color=#ff00ffff>{0}%</color> 확률로 점령을 시도합니다.");
            //            }
            //            else
            //            {
            //                GameManager.Inst.ShowTooltip($"<color=#ff00ffff>{0}%</color> 확률로 점령을 시도합니다.");
            //            }
            //        }
            //    }
            //    else
            //    {
            //        GameManager.Inst.ShowTooltip("<color=#ff00ffff>해당 타일은 이미 점령되었습니다.</color>");
            //    }
            //    break;
        }
    }
    public void HideTileMenu()
    {
        tileMenu.SetActive(false);
        GameManager.Inst.tooltip.gameObject.SetActive(false);
        tileSelectable.Remove(true);
    }
    public int RandomActable(Religion rel)
    {
        int i = 0;
        re:
        if(rel.relics.Contains(HolyRelic.GuardCharm))
        {
            if(rel.relics.Contains(HolyRelic.MiracleDice))
            {
                float rand = Random.Range(0f, 1f);
                if (rand <= 0.15f)
                {
                    i = 2;
                }
                else if (rand <= 0.35f)
                {
                    i = 3;
                }
                else if (rand <= 0.55f)
                {
                    i = 4;
                }
                else if (rand <= 0.8f)
                {
                    i = 5;
                }
                else
                {
                    i = 6;
                }
            }   
            else
            {
                i = Random.Range(2, 7);
            }
        }
        else
        {
            if (rel.relics.Contains(HolyRelic.MiracleDice))
            {
                float rand = Random.Range(0f, 1f);
                if (rand <= 0.1f)
                {
                    i = 1;
                }
                else if (rand <= 0.2f)
                {
                    i = 2;
                }
                else if (rand <= 0.35f)
                {
                    i = 3;
                }
                else if ( rand <= 0.5f)
                {
                    i = 4;
                }
                else if (rand <= 0.65f)
                {
                    i = 4;
                }
                else if ( rand <= 0.8f)
                {
                    i = 5;
                }
                else
                {
                    i = 6;
                }
            }
            else
            {
                i = Random.Range(1, 7);
            }
        }
        //if(player == usingMiracle[Miracle.ForegoneFate] && BlockedFate == i)
        //{
        //    goto re;
        //}
        //if(player == usingMiracle[Miracle.ForegoneFate])
        //{
        //    BlockedFate = 0;
        //}
        return i;
    }
    public void ShowTileInfoTab()
    {
        GameManager.Inst.infoTab.SetUp(setTile);
    }
    public void GetRelic(Religion rel, HolyRelic relic)
    {
        //rel.relics.Add(relic);
        //holyRelics.Remove(relic);

        //Relic R = null;
        //if (rel.religionType == playerRel)
        //{
        //    GameObject obj = Instantiate(RelicDic[relic], playerRelics.transform);
        //    obj.name = relic.ToString();
        //    R = obj.GetComponent<Relic>();
        //    R.owner = rel;
        //    obj.transform.localScale = Vector3.one;
        //    PositionFix(obj.transform);
        //}
        //else
        //{
        //    GameObject obj = Instantiate(RelicDic[relic], otherRelics.transform);
        //    obj.name = relic.ToString();
        //    R = obj.GetComponent<Relic>();
        //    R.owner = rel;
        //    obj.transform.localScale = Vector3.one;
        //    PositionFix(obj.transform);
        //}

        //if (relic == HolyRelic.NoWitherFlower)
        //{
        //    R.RunAbility();
        //}
        //else if(relic == HolyRelic.EternalTorch)
        //{
        //    if(rel == ReligionDic[playerRel])
        //    {
        //        EternalTorchGet = true;
        //    }
        //    else
        //    {
        //        List<Tile> hideTile = new();
        //        foreach(Tile t in tiles)
        //        {
        //            if(!rel.TempTorchedTile.Contains(t))
        //            {
        //                hideTile.Add(t);
        //            }
        //        }
        //        if(hideTile.Count > 0)
        //        {
        //            (R as EternalTorch).tile = hideTile[Random.Range(0, hideTile.Count)];
        //        }
        //    }
        //}
        //else if(relic == HolyRelic.LightBoots)
        //{
        //    FindObjectOfType<LightBoots>().RunAbility();
        //}
        //else if(relic == HolyRelic.GodsMirror)
        //{
        //    FindObjectOfType<GodsMirror>().RunAbility();
        //}
        //else if(relic == HolyRelic.SaintCrown)
        //{
        //    FindObjectOfType<SaintCrown>().RunAbility();
        //}
        //else if(relic == HolyRelic.BotaniBracelet)
        //{
        //    FindObjectOfType<BotaniBracelet>().RunAbility();
        //}
        ////if (rel.Tech[5, 5])
        ////{
        ////    rel.TechValue[5, 5] = Mathf.RoundToInt(rel.Ideal * 0.2f) * 100 + 10;
        ////}

        //rel.TechPoint++;
    }
    public void PositionFix(Transform tr)
    {
        tr.GetComponent<RectTransform>().anchoredPosition3D = new(tr.position.x, tr.position.y, 0);
    }
    public void NextOrder(Religion rel)
    {
        //AutoOccupy(ReligionDic[rel]);
        //ReturnTurn(TurnRel);
        //if (aiPlay != null)
        //{
        //    StopCoroutine(aiPlay);
        //    aiPlay = null;
        //}

        //if (rel == religionsOrder[^1])
        //{
        //    TurnEnd();
        //    TurnRel = player;
        //}
        //else
        //{
        //    for(int i = 0; i<religionsOrder.Count; i++)
        //    {
        //        if(rel == religionsOrder[i])
        //        {
        //            TurnRel = religionsOrder[i + 1];
        //            break;
        //        }
        //    }
        //}

        //ActableCount = RandomActable(ReligionDic[TurnRel]);
        //if(FixedFate > 0 && usingMiracle[Miracle.ForegoneFate] == TurnRel)
        //{
        //    ActableCount = FixedFate;
        //    FixedFate = 0;
        //    BlockedFate = 0;
        //}

        //if (ReligionDic[TurnRel].isSnakeVenom)
        //{
        //    GameManager.Inst.ShowLogBox($"{TurnRel} 종교는 뱀독으로 인해 턴을 넘깁니다.");
        //    NextOrder(TurnRel);
        //    ReligionDic[TurnRel].isSnakeVenom = false;
        //    return;
        //}

        //if (TurnRel != player || autoPlay)
        //{
        //}
    }
    public void ShowMiracles()
    {
        if(miracleMenu.activeSelf)
        {
            HideMiracles();
            return;
        }
        tileSelectable.Add(true);

        foreach(Transform miracle in miracleMenu.transform.Find("MiracleList"))
        {
            miracle.gameObject.SetActive(false);
            if (player.miracles.Contains((Miracle)System.Enum.Parse(typeof(Miracle), miracle.name)))
            {
                miracle.gameObject.SetActive(true);
            }
        }
        miracleMenu.SetActive(true);
    }
    public void HideMiracles()
    {
        miracleMenu.SetActive(false);
        tileSelectable.Remove(true);
    }
    public void MiracleToolTip(Miracle miracle)
    {
        switch (miracle)
        {
            case Miracle.RisingSun:
                GameManager.Inst.ShowTooltip("1턴간 점령한 무작위 타일 10개에서 호재를 불러일으킵니다.");
                break;
            case Miracle.SnakeVenom:
                GameManager.Inst.ShowTooltip("지정한 종교는 1턴간 아무런 행동을 취할 수 없습니다.");
                break;
            case Miracle.CompulsoryExcution:
                GameManager.Inst.ShowTooltip("1턴간 논쟁 승리시 해당 타일을 강제로 빼앗습니다.");
                break;
            case Miracle.ForegoneFate:
                GameManager.Inst.ShowTooltip("다음 턴에 발생할 주사위를 미리 확인하고 고정하거나 배제할 수 있습니다.");
                break;
            case Miracle.ConcealedFuture:
                GameManager.Inst.ShowTooltip("1턴간 지정한 타일에 악재를 불러일으킵니다.");
                break;
            case Miracle.PropertyGrowth:
                GameManager.Inst.ShowTooltip("평균 획득 코인의 5배를 즉시 획득합니다.");
                break;
            case Miracle.Charisma:
                GameManager.Inst.ShowTooltip("점령확률이 80% 이상인 타일 5개를 점령합니다.");
                break;
            case Miracle.BuildWall:
                GameManager.Inst.ShowTooltip("1턴간 타 종교의 점령 확률이 20% 미만인 타일에 한해 전도 및 기부 수치를 0으로 변경합니다.");
                break;
            case Miracle.Prophecy:
                GameManager.Inst.ShowTooltip("무작위 성유물의 위치를 확인합니다.");
                break;
            case Miracle.Judgement:
                GameManager.Inst.ShowTooltip("3턴간 빼앗긴 타일이 코인을 생산하지 못합니다. 단 재점령시 해당 효과는 취소됩니다.");
                break;
            case Miracle.ForesightDream:
                GameManager.Inst.ShowTooltip("해당 턴의 행동 횟수가 2배가 됩니다.");
                break;
            case Miracle.FrozenHeart:
                GameManager.Inst.ShowTooltip("1턴간 보유한 모든 타일을 타 종교의 점령이 실패하였을 경우 해당 타일은 2턴간 점령이 불가해집니다.");
                break;
        }
    }
    public void ShowMiracleTarget()
    {
        tileSelectable.Add(true);
        foreach (Transform transform in miracleTarget.transform)
        {
            Destroy(transform.gameObject);
        }
        foreach (Religion rel in religions)
        {
            Debug.Log(rel.religionType);
            if (rel != player)
            {
                GameObject obj = Instantiate(miracleTargetPrefab, miracleTarget.transform);

                obj.name = rel.religionType.ToString();
                obj.GetComponent<Button>().onClick.AddListener(() => { miracleTargetRel = rel; miracleTarget.SetActive(false); tileSelectable.Remove(true); });
                obj.transform.Find("Symbol").GetComponent<Image>().sprite = rel.symbol;
                obj.transform.Find("RelName").GetComponent<TextMeshProUGUI>().text = rel.religionType.ToString();
            }
        }
    }
    public void TurnEnd()
    {
        if (bestRel != null)
        {
            bestRel.Ideal -= 15;
            bestRel = null;
        }

        Turn++;
    }
    public void AutoPlaySwitch()
    {
        autoPlay = !autoPlay;
    }
    public void PlayerTurnEnd()
    {
        NextOrder(TurnRel);
    }
    public void ReturnTurn(Religion rel)
    {
        //foreach(Tile tile in tiles)
        //{
        //    if(tile.servRel.Contains(ReligionDic[rel]))
        //    {
        //        foreach(Religion r in religions)
        //        {
        //            if (r.religionType == rel)
        //                continue;
        //            tile.servRel.Remove(r);
        //            tile.dysentery[r.religionType] -= 25;
        //        }
        //    }
        //}
        //foreach(KeyValuePair<Miracle, int> _turn in MiracleTurn)
        //{
        //    if (usingMiracle[_turn.Key] == rel)
        //    {
        //        MiracleTurn[_turn.Key]--;
        //        return;
        //    }
        //}
        //if (ReligionDic[rel].TechValue[5, 5] > 0)
        //{
        //    ReligionDic[rel].Ideal -= ReligionDic[rel].TechValue[5, 5] / 100;
        //    int turn = ReligionDic[rel].TechValue[5, 5] % 100 - 1;
        //    if(turn > 0)
        //    {
        //        ReligionDic[rel].TechValue[5, 5] = ReligionDic[rel].Ideal * 100 + turn;
        //    }
        //}
    }
    public void AutoOccupy(Religion rel)
    {
    }
    public void GetTech(Religion religion, Vector2Int tech)
    {
        for(int j = 0; j < 6; j++)
        {
            if(tech.y == j)
            {
                int k = j % 2 == 0 ? j + 1 : j - 1;

                for (int i = 0; i < 6; i++)
                {
                    if (religion.techList.Techs[k, i].isInvested)
                        religion.Dysentery++;
                }
            }
        }
        for (int i = 0; i < 6; i++)
        {
            if (religion.techList.Techs[tech.y, i].isInvested)
                religion.Ideal++;
        }

        if(tech == new Vector2Int(0, 5))
        {
            religion.TorchRange++;
        }
        else if(tech == new Vector2Int(2, 0))
        {
            religion.Ideal += 30;
        }
        else if(tech == new Vector2Int(4, 0))
        {
            religion.AutoOccupy -= 5;
        }
        religion.techList.Techs[tech.y, tech.x].isInvested = true;
    }
    public void ShowDetail()
    {
        DetailUI.SetActive(!DetailUI.activeSelf);
        if(DetailUI.activeSelf)
        {
            DetailUI.transform.Find("Ideal").GetComponent<TextMeshProUGUI>().text = $"이상 : {ReligionDic[playerRel].Ideal}";
            DetailUI.transform.Find("Dysentery").GetComponent<TextMeshProUGUI>().text = $"이질 : {ReligionDic[playerRel].Dysentery}";
            DetailUI.transform.Find("Logic").GetComponent<TextMeshProUGUI>().text = $"논리 : {ReligionDic[playerRel].Logic}";
        }
    }
    public void ShowTech()
    {
        if(TurnRel != playerRel)
        {
            GameManager.Inst.ShowLogBox("현재 플레이어의 턴이 아닙니다.");
            return;
        }
        if (TechUI.activeSelf)
            tileSelectable.Remove(true);
        else
            tileSelectable.Add(true);
        TechUI.SetActive(!TechUI.activeSelf);
        TechUI.transform.Find("TechPoint").GetComponent<TextMeshProUGUI>().text = $"테크 포인트 : {ReligionDic[playerRel].TechPoint}";
    }
    public void ProductAtTiles()
    {
        foreach (TileData tileData in tileDatas)
            tileData.Production.Product();
    }
}