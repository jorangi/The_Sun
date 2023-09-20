using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tiles : MonoBehaviour
{
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
    private Vector2 mousePos;
    private bool moveable = false;
    public int width, height;
    public float AvgTotalMadeCoin;
    public GameObject TilePrefab;
    public Tile[,] tiles;
    public Tile setTile;
    public Color goodTile, badTile, enableTile, disableTile, blockedTile;

    //게임 플레이 관련
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
    public Religions playerRel;
    private Religions turnRel;
    public Religions TurnRel
    {
        get => turnRel;
        set
        {
            if(value != Religions.none)
            {
                TurnObject.GetComponent<Image>().sprite = symbols[System.Array.IndexOf(System.Enum.GetNames(typeof(Religions)), value.ToString()) - 1];
            }
            turnRel = value;
        }
    }
    public Dictionary<Miracle, Religions> usingMiracle = new();
    public Dictionary<Miracle, int> MiracleTurn = new();
    public Dictionary<Religions, Religion> ReligionDic = new();
    public Dictionary<HolyRelic, Tile> holyRelic = new();
    public Dictionary<HolyRelic, GameObject> RelicDic = new();
    public List<HolyRelic> holyRelics = new();
    public Dictionary<Religions, int[]> RecoredDir = new();
    public List<Religion> religions = new();
    public List<Religions> religionsOrder = new();
    public int[] MiraclePrice;
    private int turn;
    public int Turn
    {
        get => turn;
        set
        {
            if(value == Mathf.RoundToInt(MaxTurn * 0.2f) || value == Mathf.RoundToInt(MaxTurn * 0.4f) || value == Mathf.RoundToInt(MaxTurn * 0.6f) || value == Mathf.RoundToInt(MaxTurn * 0.8f))
            {
                foreach(Religion rel in religions)
                {
                    rel.TechPoint++;
                }
            }
            if(value > MaxTurn)
            {
                Time.timeScale = 0;
                Debug.Log("오와리");
            }
            TurnObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = value.ToString();
            if(ReligionDic[playerRel].relics.Contains(HolyRelic.OldMap) && turn % 5 == 0 && value % 5 != 0)
            {
                FindObjectOfType<OldMap>().HideMap();
            }
            turn = value;
            if(ReligionDic[playerRel].relics.Contains(HolyRelic.OldMap) && value%5 == 0)
            {
                FindObjectOfType<OldMap>().RunAbility();
            }
        }
    }
    public int MaxTurn = 300;
    private int actableCount;
    public int ActableCount
    {
        get => actableCount;
        set
        {
            actableCount = value;
            TurnObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"{ActableCount}";
            //if(value == 0 && TurnRel != playerRel)
        }
    }
    public int BlockedFate = 0;
    public int FixedFate = 0;

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
                setTile = hit.collider.GetComponent<Tile>();
                if (setTile.fogBool[playerRel].Count > 0 && !EternalTorchGet)
                {
                    ShowTileMenu();
                }
                else if (EternalTorchGet)
                {
                    FindObjectOfType<EternalTorch>().tile = setTile;
                    FindObjectOfType<EternalTorch>().RunAbility();
                    EternalTorchGet = false;
                }
            }
        }

        if (Input.GetMouseButtonDown(2))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 1, 1 << 6);

            if (hit.collider != null)
            {
                hit.collider.GetComponent<Tile>().SetReligion(null);
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
            usingMiracle.Add((Miracle)i, Religions.none);
            MiracleTurn.Add((Miracle)i, 0);
        }
        TurnObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{1}";
        baseDysentery = 5f;
        width = 10;
        height = 10;
        MaxRelic = 3;
        MaxReligion = 3;

        CAMMIN = 18 + (Mathf.Max(width, height) - 30)/2 * 1; 

        Vector2 boxSize = new(width, height);
        tiles = new Tile[height, width];

        for(int i = 0; i<height; i++)
        {
            for(int j = 0; j<width; j++)
            {
                GameObject obj = Instantiate(TilePrefab, transform);
                tiles[i, j] = obj.GetComponent<Tile>();
                tiles[i, j].Pos = new Vector2Int(j, i);
                obj.name = $"{j}|{i}";
                obj.transform.position = new(-boxSize.x/2 + j , boxSize.y/2 - i);
            }
        }

        //태양교 셋업
        playerRel = Religions.TheSun;
        ReligionDataSetup(Religions.TheSun);
        TurnObject.GetComponent<Image>().sprite = religions[0].symbol;

        //for(int i =0; i < 5; i++)
        //{
        //    for(int j =0; j < 5; j++)
        //    {
        //        ReligionDic[playerRel].Tech[i, j] = true;
        //    }
        //}

        //무작위 종교 셋업
        for (int i = 0; i<MaxReligion-1; i++)
        {
            Religions rel;
            do
            {
                rel = (Religions)Random.Range(0, System.Enum.GetNames(typeof(Religions)).Length);
            } while (religionsOrder.Contains(rel) || rel == Religions.none);
            ReligionDataSetup(rel);
        }

        //무작위 성유물 셋업
        HolyRelic relic = HolyRelic.none;
        for (int i = 0; i < MaxRelic; i++)
        {
            do
            {
                relic = (HolyRelic)Random.Range(0, System.Enum.GetNames(typeof(HolyRelic)).Length);
            } while (holyRelic.ContainsKey(relic) || relic == HolyRelic.none);

            Tile tile = null;
            do
            {
                tile = tiles[Random.Range(0, height), Random.Range(0, width)];
            } while (tile.relic != HolyRelic.none);
            holyRelic.Add(relic, tile);
            tile.relic = relic;
        }
        Turn = 1;
        TurnRel = playerRel;
        MaxTurn = 500;
        AvgTotalMadeCoin = 0f;
        ActableCount = RandomActable(ReligionDic[playerRel]);

        foreach(Religions religions in religionsOrder)
        {
            ReligionDic[religions].TorchRange = 3;
        }
        foreach(Tile t in tiles)
        {
            t.NextCondition();
        }

        //for(int i =0; i< 10; i++)
        //{
        //    AddTile(ReligionDic[playerRel], tiles[0, i]);
        //}
        //for(int i =0; i<10; i++)
        //{
        //    Tile tempTile = null;
        //    do
        //    {
        //        tempTile = tiles[Random.Range(0, height), Random.Range(0, width)];
        //    } while (tempTile.mainReligion.religion != Religions.none);
        //    AddTile(ReligionDic[playerRel], tempTile);
        //}

        //ReligionDic[playerRel].miracle.Add(Miracle.SnakeVenom);
        //ReligionDic[playerRel].miracle.Add(Miracle.CompulsoryExcution);
        //ReligionDic[playerRel].miracle.Add(Miracle.ForegoneFate);
        //ReligionDic[playerRel].miracle.Add(Miracle.ConcealedFuture);
        //ReligionDic[playerRel].miracle.Add(Miracle.PropertyGrowth);
        //ReligionDic[playerRel].miracle.Add(Miracle.Charisma);
        //ReligionDic[playerRel].miracle.Add(Miracle.BuildWall);
        ReligionDic[playerRel].miracle.Add(Miracle.Prophecy);
        //ReligionDic[playerRel].miracle.Add(Miracle.Judgement);
        //ReligionDic[playerRel].miracle.Add(Miracle.ForesightDream);
        //ReligionDic[playerRel].miracle.Add(Miracle.FrozenHeart);
        //ReligionDic[playerRel].Sunlight = 10000;


        //GetRelic(ReligionDic[playerRel], HolyRelic.GoldenEye);
        //GetRelic(ReligionDic[playerRel], HolyRelic.SaintStaff);
        //GetRelic(ReligionDic[playerRel], HolyRelic.MiracleDice);
        //GetRelic(ReligionDic[playerRel], HolyRelic.GuardCharm);
        //GetRelic(ReligionDic[playerRel], HolyRelic.WrittenStone);
        //GetRelic(ReligionDic[playerRel], HolyRelic.NoWitherFlower);
        //GetRelic(ReligionDic[playerRel], HolyRelic.SaintCrown);
        //GetRelic(ReligionDic[playerRel], HolyRelic.OldMap);
        //GetRelic(ReligionDic[playerRel], HolyRelic.LightBoots);
        //GetRelic(ReligionDic[playerRel], HolyRelic.EternalTorch);
        //GetRelic(ReligionDic[playerRel], HolyRelic.BotaniBracelet);
        //GetRelic(ReligionDic[playerRel], HolyRelic.GodsMirror);
    }
    private Religion ReligionDataSetup(Religions rels)
    {
        Tile tile;
        do
        {
            tile = tiles[Random.Range(0, height), Random.Range(0, width)];
        } while (tile.mainReligion.religion != Religions.none);

        Religion religion = new()
        {
            Sunlight = 0,
            AutoOccupy = 95,
            evangelizeValue = 1,
            Logic = 5,
            Ideal = 5,
            Dysentery = 0,
            religion = rels,
            symbol = symbols[(int)rels - 1],
            SacredPlace = tile
        };
        religion.Coin = 10;
        foreach (Tile t in tiles)
        {
            t.dysentery.Add(rels, 0);
            t.influence.Add(rels, 0);
            t.serveidRel.Add(religion, false);
            t.autoOcc.Add(religion, 0);
            t.SetFog(religion, t != tile);
        }

        AddTile(religion, tile);
        religionsOrder.Add(religion.religion);
        religions.Add(religion);
        tile.AddInfluence(religion, MaxInfluence);
        tile.Sacred.gameObject.SetActive(true);
        ReligionDic.Add(rels, religion);
        GameObject obj = Instantiate(ReligionsTile, ReligionsCounters.transform);
        obj.name = religion.religion.ToString();
        obj.transform.GetChild(0).GetComponent<Image>().sprite = religion.symbol;
        obj.GetComponentInChildren<TextMeshProUGUI>().text = $"{1}";

        switch (rels)
        {
            case Religions.TheSun:
                religion.miracle.Add(Miracle.RisingSun);
                break;
            case Religions.PurpleSnake:
                religion.miracle.Add(Miracle.SnakeVenom);
                break;
            case Religions.PeaceParin:
                religion.miracle.Add(Miracle.CompulsoryExcution);
                break;
            case Religions.Yusung:
                religion.miracle.Add(Miracle.ForegoneFate);
                break;
            case Religions.Nortra:
                religion.miracle.Add(Miracle.Charisma);
                break;
            case Religions.Kamomir:
                religion.miracle.Add(Miracle.BuildWall);
                break;
            case Religions.Vectarr:
                religion.miracle.Add(Miracle.Prophecy);
                break;
            case Religions.Hwagak:
                religion.miracle.Add(Miracle.PropertyGrowth);
                break;
            case Religions.BingGyeock:
                religion.miracle.Add(Miracle.FrozenHeart);
                break;
            case Religions.Ogare:
                religion.miracle.Add(Miracle.Judgement);
                break;
            case Religions.Nuvikk:
                religion.miracle.Add(Miracle.ForesightDream);
                break;
            case Religions.Concheta:
                religion.miracle.Add(Miracle.ConcealedFuture);
                break;
        }

        return religion;
    }
    public void AddTile(Religion rel, Tile tile)
    {
        if (tile.mainReligion.GetTiles.Count == 1)
        {
            foreach(Miracle miracles in tile.mainReligion.miracle)
            {
                rel.miracle.Add(miracles);
            }
        }

        rel.GetTiles.Add(tile);
        rel.GetTiles[^1].SetReligion(rel);
        int max = 0;
        foreach(KeyValuePair<Religions, int> r in tile.influence)
        {
            if(r.Key != playerRel)
            {
                max += r.Value;
            }
        }
    }
    private void ShowTileMenu()
    {
        tileMenu.SetActive(true);
        evanBtn.interactable = ActableCount > 0;
        charityBtn.interactable = religions[0].Coin > 0 && ActableCount > 0;
        int i = 0;
        foreach(KeyValuePair<Religions, int> inf in setTile.influence)
        {
            if(inf.Key != playerRel)
            {
                i++;
            }
        }
        arguBtn.interactable = i > 0 && setTile.influence[playerRel] > 0 && ActableCount > 0;
        serRelBtn.interactable = !setTile.serveidRel[ReligionDic[playerRel]] && ActableCount > 0;
        buyRelBtn.interactable = setTile.serveidRel[ReligionDic[playerRel]] && setTile.relic != HolyRelic.none && ActableCount > 0;
        occBtn.interactable = setTile.mainReligion != religions[0] && setTile.influence.ContainsKey(playerRel) && ActableCount > 0;

        tileSelectable.Add(true);
    }
    public void ShowTooltip(int i)
    {
        switch(i)
        {
            default:
                GameManager.Inst.ShowTooltip("");
                break;
            case 1:
                if (setTile.influence[playerRel] < 20)
                {
                    GameManager.Inst.ShowTooltip($"전도를 하여 <color=#ffff00ff>{EvanGelizeCalc(ReligionDic[playerRel])}</color>만큼의 영향력을 높힙니다.");
                }
                else
                {
                    GameManager.Inst.ShowTooltip($"영향력이 최대입니다. 전도를 통해 안정만을 획득할 수 있습니다.");
                }
                break;
            case 2:
                if (setTile.influence[playerRel] < 20)
                {
                    GameManager.Inst.ShowTooltip("기부를 하여 영향력을 높힙니다.");
                }
                else
                {
                    GameManager.Inst.ShowTooltip($"영향력이 최대입니다. 전도를 통해 안정만을 획득할 수 있습니다.");
                }
                break;
            case 3:
                GameManager.Inst.ShowTooltip("논쟁을 하여 상대방의 영향력을 낮추고 자신의 영향력을 높입니다.");
                break;
            case 4:
                if (!setTile.serveidRel[ReligionDic[playerRel]])
                {
                    GameManager.Inst.ShowTooltip("해당 타일에서 성유물을 찾아냅니다.");
                }
                else
                {
                    GameManager.Inst.ShowTooltip("해당 타일에서 성유물을 찾아냅니다.<br><color=#ff00ffff>해당 타일에서 성유물 조사를 이미 마쳤습니다.</color>");
                }
                break;
            case 5:
                if (setTile.serveidRel[ReligionDic[playerRel]] && setTile.relic != HolyRelic.none)
                {
                    int RelicPrice = BuyRelicCalc(ReligionDic[playerRel]);

                    if (RelicPrice > ReligionDic[playerRel].Coin)
                    {
                        GameManager.Inst.ShowTooltip($"코인이 부족하여 성유물을 인수할 수 없습니다.<br>성유물의 가격이{RelicPrice}코인입니다.<br><color=#ff00ffff>{RelicPrice - ReligionDic[playerRel].Coin}만큼의 코인이 부족합니다.</color>");
                    }
                    else
                    {
                        GameManager.Inst.ShowTooltip($"<color=#ffff00ff>{RelicPrice}</color>코인을 지불하여 발견된 성유물을 인수합니다.");
                    }
                }
                else if(!setTile.serveidRel[ReligionDic[playerRel]])
                {
                    GameManager.Inst.ShowTooltip("<color=#ff00ffff>해당 타일에 성유물이 존재하는지 알 수 없습니다.</color>");
                }
                else
                {
                    GameManager.Inst.ShowTooltip("<color=#ff00ffff>해당 타일에 성유물이 존재하지 않습니다.</color>");
                }
                break;
            case 6:
                if (setTile.mainReligion != religions[0])
                {
                    if(!setTile.influence.ContainsKey(playerRel))
                    {
                        GameManager.Inst.ShowTooltip("<color=#ff00ffff>해당 타일에 영향력이 없습니다.</color>");
                    }
                    else
                    {
                        if (setTile.Sacred.gameObject.activeSelf)
                        {
                            GameManager.Inst.ShowTooltip($"해당 타일은 성지입니다.<br><color=#ff00ffff>{OccupyCalc(ReligionDic[playerRel], setTile)}%</color> 확률로 점령을 시도합니다.");
                        }
                        else
                        {
                            GameManager.Inst.ShowTooltip($"<color=#ff00ffff>{OccupyCalc(ReligionDic[playerRel], setTile)}%</color> 확률로 점령을 시도합니다.");
                        }
                    }
                }
                else
                {
                    GameManager.Inst.ShowTooltip("<color=#ff00ffff>해당 타일은 이미 점령되었습니다.</color>");
                }
                break;
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
        if(TurnRel == usingMiracle[Miracle.ForegoneFate] && BlockedFate == i)
        {
            goto re;
        }
        if(TurnRel == usingMiracle[Miracle.ForegoneFate])
        {
            BlockedFate = 0;
        }
        return i;
    }
    public void p_Evangelize()
    {
        Evangelize();
        ActableCount--;
    }
    public void Evangelize()
    {
        Evangelize(ReligionDic[playerRel]);
    }
    public void Evangelize(Religion rel)
    {
        setTile.AddInfluence(rel, EvanGelizeCalc(rel));
        if (rel == ReligionDic[playerRel])
        {
            GameManager.Inst.ShowLogBox($"해당 타일에서 {EvanGelizeCalc(rel)}의 영향력을 획득하였습니다.");
        }
        HideTileMenu();
    }
    public int EvanGelizeCalc(Religion rel)
    {
        float relDyBonus = 1.0f;
        float idealBonus = 1.0f;

        if (rel.Tech[1, 0] && setTile.mainReligion.religion != Religions.none && setTile.mainReligion.religion != rel.religion)
        {
            relDyBonus -= 0.1f;
        }
        if (setTile.mainReligion.Tech[3,0] && rel != setTile.mainReligion)
        {
            relDyBonus += 0.15f;
        }
        int distanceDys = Mathf.RoundToInt(Vector2Int.Distance(rel.SacredPlace.Pos, setTile.Pos) / Mathf.Min(width, height) * baseDysentery);
        int writtenStone = rel.relics.Contains(HolyRelic.WrittenStone) ? 1 : 0;

        if (MiracleTurn[Miracle.BuildWall] > 0 && usingMiracle[Miracle.BuildWall] != rel.religion && OccupyCalc(rel, setTile) < 20)
        {
            return writtenStone;
        }

        if (rel.Tech[1, 3] && rel.avgCoin >= setTile.ProCoin)
        {
            idealBonus += 0.07f;
        }

        float valueBonus = 1.0f;
        if (rel.Tech[1,5])
        {
            for (int i = Mathf.Max(0, setTile.Pos.y - 1); i <= Mathf.Min(GameManager.Inst.tiles.height - 1, setTile.Pos.y + 1); i++)
            {
                for (int j = Mathf.Max(0, setTile.Pos.x - 1); j <= Mathf.Min(GameManager.Inst.tiles.width - 1, setTile.Pos.x + 1); j++)
                {
                    if (Mathf.Abs(setTile.Pos.y - i) + Mathf.Abs(setTile.Pos.x - j) == 1 && GameManager.Inst.tiles.tiles[i, j].mainReligion == setTile.mainReligion)
                    {
                        valueBonus += 0.2f;
                        goto techOneFive;
                    }
                }
            }
        }
        if (rel.Tech[5, 4] && setTile.mainReligion == rel)
        {
            valueBonus += 0.3f;
        }
        techOneFive:

        if (setTile.mainReligion.Tech[2, 2])
        {
            valueBonus -= 0.1f;
        }
        float relInf = setTile.influence[rel.religion];

        foreach(Religion r in religions)
        {
            if (r != rel && relInf < setTile.influence[r.religion])
                relInf = -1.0f;
        }
        if (relInf < 0)
        {
            relInf = 0f;
        }
        else
        {
            relInf = 10;
        }

        return Mathf.Clamp(Mathf.RoundToInt((Mathf.RoundToInt((rel.Ideal + relInf) * idealBonus) - Mathf.RoundToInt((rel.Dysentery + distanceDys + setTile.dysentery[rel.religion]) * relDyBonus)) * valueBonus) + writtenStone, 1, MaxInfluence);
    }
    public void Charity()
    {
        int val = System.Convert.ToInt32(charityMenu.GetComponentInChildren<TMP_InputField>().text);
        if(val <= ReligionDic[playerRel].Coin)
        {
            Charity(ReligionDic[playerRel], val);
            HideCharityMenu(false);
            ActableCount--;
        }
        else
        {
            GameManager.Inst.ShowLogBox("코인이 모자릅니다.");
        }
    }
    public void CharityValueEnd()
    {
        TMP_InputField t = charityMenu.GetComponentInChildren<TMP_InputField>();
        int val = System.Convert.ToInt32(t.text);
        charityMenu.transform.Find("CharityDetail").Find("GetInfluence").GetComponent<TextMeshProUGUI>().text = $"획득 영향력 : {CharityCalc(ReligionDic[playerRel], val)}";
        if (t.text != string.Empty)
        {
            t.text = $"{Mathf.Min(System.Convert.ToInt32(t.text), ReligionDic[playerRel].Coin)}";
        }
    }
    public void ShowCharityMenu()
    {
        GameManager.Inst.tooltip.gameObject.SetActive(false);
        tileMenu.SetActive(false);
        charityMenu.SetActive(true);
        tileSelectable.Add(true);
    }
    public void HideCharityMenu(bool cancle)
    {
        tileMenu.SetActive(cancle);
        charityMenu.SetActive(false);
        tileSelectable.Remove(true);
    }
    public int CharityCalc(Religion rel, int val)
    {
        int badBonus = setTile.condition == TileCondition.Bad ? 2 : 1;
        int writtenStone = rel.relics.Contains(HolyRelic.WrittenStone) ? 1 : 0;

        if (MiracleTurn[Miracle.BuildWall] > 0 && usingMiracle[Miracle.BuildWall] != rel.religion && OccupyCalc(rel, setTile) < 20)
        {
            return writtenStone;
        }

        float valueBonus = 1.0f;
        if (rel.Tech[1, 5])
        {
            for (int i = Mathf.Max(0, setTile.Pos.y - 1); i <= Mathf.Min(GameManager.Inst.tiles.height - 1, setTile.Pos.y + 1); i++)
            {
                for (int j = Mathf.Max(0, setTile.Pos.x - 1); j <= Mathf.Min(GameManager.Inst.tiles.width - 1, setTile.Pos.x + 1); j++)
                {
                    if (Mathf.Abs(setTile.Pos.y - i) + Mathf.Abs(setTile.Pos.x - j) == 1 && GameManager.Inst.tiles.tiles[i, j].mainReligion == setTile.mainReligion)
                    {
                        valueBonus += 0.2f;
                        goto techOneFive;
                    }
                }
            }
        }

        if (rel.Tech[5, 4] && setTile.mainReligion == rel)
        {
            valueBonus += 0.3f;
        }
        techOneFive:

        if (setTile.mainReligion.Tech[2,2])
        {
            valueBonus -= 0.1f;
        }
        if (rel.Tech[2,4] && setTile.mainReligion == null)
        {
            valueBonus += 0.6f;
        }

        if ((float)(val / Mathf.Max(1, setTile.TotalMadeCoin)) * (val / Mathf.Max(1, setTile.TotalDonation)) * badBonus > 0)
        {
            return Mathf.Min(MaxInfluence, Mathf.RoundToInt((float)(val / Mathf.Max(1, setTile.TotalMadeCoin)) * (val / Mathf.Max(1, setTile.TotalDonation)) * badBonus * valueBonus) + writtenStone);
        }
        else
        {
            return 1 + writtenStone;
        }
    }
    public void Charity(Religion rel, int val)
    {
        rel.Coin -= val;
        setTile.AddInfluence(rel, CharityCalc(rel, val));
        setTile.TotalDonation += val;
        if (rel == ReligionDic[playerRel])
        {
            GameManager.Inst.ShowLogBox($"해당 타일에서 {CharityCalc(rel, val)}의 영향력을 획득하였습니다.");
        }
        HideTileMenu();
    }
    public float ArgumentCalc(Religion rel, Religion target)
    {
        float x = setTile.stability.ContainsKey(rel.religion) ? setTile.stability[rel.religion] : 0;
        int myLogic = rel.Logic;
        int targetLogic = target.Logic;
        if (target.Tech[0, 2])
        {
            targetLogic = Mathf.RoundToInt(targetLogic * 1.25f);
        }
        if (rel.Tech[3,1])
        {
            myLogic = Mathf.RoundToInt(myLogic * 1.15f);
        }
        if (target.Tech[3,1])
        {
            targetLogic = Mathf.RoundToInt(myLogic * 1.15f);
        }
        if (rel.Tech[1,1] && setTile.influence[rel.religion] < setTile.influence[target.religion])
        {
            myLogic = Mathf.RoundToInt(myLogic * 1.2f);
        }
        if (target.Tech[1, 1] && setTile.influence[target.religion] < setTile.influence[rel.religion])
        {
            targetLogic = Mathf.RoundToInt(targetLogic * 1.2f);
        }
        return Mathf.Clamp(((float)(myLogic + x) - (targetLogic + x)) / Mathf.Max(1, myLogic + x), 0, 1);
    }
    public int ArgumentReward(Religion rel, Religion target, bool victory)
    {
        float relDyBonus = 1.0f;
        float tarDyBonus = 1.0f;

        if (rel.Tech[1, 0] && setTile.mainReligion.religion != Religions.none && setTile.mainReligion.religion != rel.religion)
        {
            relDyBonus -= 0.1f;
        }
        if (target.Tech[1, 0] && setTile.mainReligion.religion != Religions.none && setTile.mainReligion.religion != target.religion)
        {
            tarDyBonus -= 0.1f;
        }
        if (setTile.mainReligion.Tech[3, 0] && rel != setTile.mainReligion)
        {
            relDyBonus += 0.15f;
        }
        if (setTile.mainReligion.Tech[3, 0] && target != setTile.mainReligion)
        {
            tarDyBonus += 0.15f;
        }

        float valueBonus = 1.0f;
        if (rel.Tech[5, 4] && setTile.mainReligion == rel)
        {
            valueBonus += 0.3f;
        }

        if(victory)
        {
            int distanceDys = Mathf.RoundToInt(Vector2Int.Distance(target.SacredPlace.Pos, setTile.Pos) / Mathf.Max(width, height) * baseDysentery);
            int val = Mathf.RoundToInt(valueBonus * Mathf.Min(setTile.influence[target.religion], Mathf.RoundToInt((target.Dysentery + distanceDys + setTile.dysentery[rel.religion]) * tarDyBonus)));
            return Mathf.Max(1, Mathf.RoundToInt(val * ArgumentCalc(rel, target)));
        }
        else
        {
            int distanceDys = Mathf.RoundToInt(Vector2Int.Distance(rel.SacredPlace.Pos, setTile.Pos) / Mathf.Max(width, height) * baseDysentery);
            int val = Mathf.RoundToInt(valueBonus * Mathf.Min(setTile.influence[rel.religion], Mathf.RoundToInt((rel.Dysentery + distanceDys + setTile.dysentery[rel.religion]) * relDyBonus)));
            return Mathf.Max(1, Mathf.RoundToInt(val * ArgumentCalc(rel, target)));
        }
    }
    public void Argument(Religion rel, Religion target)
    {
        if (Random.Range(0f, 1f) <= ArgumentCalc(rel, target))
        {
            int val = ArgumentReward(rel, target, true);
            Debug.Log(val);
            setTile.AddInfluence(target, -val);
            setTile.AddInfluence(rel, val);

            GameManager.Inst.ShowLogBox($"{setTile.Pos} 타일에서 {rel.religion.ToString()} 종교가 {target.religion.ToString()} 종교와의 논쟁에서 승리하여 {val}만큼의 영향력을 흡수하였습니다.");
            if (usingMiracle[Miracle.CompulsoryExcution] == rel.religion)
            {
                AddTile(rel, setTile);
            }
        }
        else
        {
            int val = ArgumentReward(rel, target, false);
            setTile.AddInfluence(rel, -val);
            setTile.AddInfluence(target, val);
            GameManager.Inst.ShowLogBox($"{setTile.Pos} 타일에서 {target.religion.ToString()} 종교가 {rel.religion.ToString()} 종교와의 논쟁에서 승리하여 {val}만큼의 영향력을 흡수하였습니다.");
            if (usingMiracle[Miracle.CompulsoryExcution] == target.religion)
            {
                AddTile(target, setTile);
            }
        }
        HideTileMenu();
    }
    public void Argument()
    {
        foreach(Transform tr in argumentMenu.transform)
        {
            Destroy(tr.gameObject);
        }
        GameManager.Inst.tooltip.gameObject.SetActive(false);
        tileMenu.SetActive(false);
        argumentMenu.SetActive(true);
        tileSelectable.Add(true);
        foreach(Religion rel in religions)
        {
            if (setTile.influence[rel.religion] > 0)
            {
                GameObject obj = Instantiate(argumentReligionBtn, argumentMenu.transform);
                obj.transform.Find("Button").Find("Symbol").GetComponent<Image>().sprite = rel.symbol;
                if (rel.religion == playerRel)
                {
                    obj.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => HideArgumentMenu(true));
                    obj.transform.Find("Button").Find("RelName").GetComponent<TextMeshProUGUI>().text = "논쟁 취소";
                    obj.transform.Find("Value").GetComponent<TextMeshProUGUI>().text = "";
                }
                else
                {
                    obj.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => { Argument(ReligionDic[playerRel], rel); HideArgumentMenu(false); ActableCount--; });
                    obj.transform.Find("Button").Find("RelName").GetComponent<TextMeshProUGUI>().text = $"{rel.religion.ToString()}";
                    obj.transform.Find("Value").GetComponent<TextMeshProUGUI>().text = $"{Mathf.RoundToInt(ArgumentCalc(ReligionDic[playerRel], rel) * 100)}% / {ArgumentReward(ReligionDic[playerRel], rel, true)}";
                }
            }
        }
    }
    public void HideArgumentMenu(bool cancle)
    {
        tileMenu.SetActive(cancle);
        argumentMenu.SetActive(false);
        tileSelectable.Remove(true);
    }
    public void SurveyRelic()
    {
        SurveyRelic(ReligionDic[playerRel]);
        ActableCount--;
    }
    public void SurveyRelic(Religion rel)
    {
        setTile.serveidRel[rel] = true;
        if (setTile.relic != HolyRelic.none)
        {
            //렐릭 심볼과 X로 표기할 예정
            GameManager.Inst.ShowLogBox($"{rel.religion}종교가 성유물 '{setTile.relic}'을 발견하였습니다.");
            setTile.HolyRelicSymbol.gameObject.SetActive(true);
            rel.knownRelicTile.Add(setTile);
            if(rel.religion == playerRel)
            {
                setTile.transform.Find("TempRelic").gameObject.SetActive(true);
            }

            if (rel.Tech[4, 3])
            {
                setTile.findTechBonus = 0.7f;
            }
        }
        else
        {
            if (rel.religion == playerRel)
                GameManager.Inst.ShowLogBox($"해당 지역에서 성유물을 발견할 수 없었습니다.");
        }
        HideTileMenu();
    }
    public void BuyRelic()
    {
        BuyRelic(ReligionDic[playerRel]);
        ActableCount--;
    }
    public void BuyRelic(Religion rel)
    {
        rel.Coin -= BuyRelicCalc(rel);
        GetRelic(rel, setTile.relic);
        foreach (Religion rels in religions)
        {
            if(rels.knownRelicTile.Contains(setTile))
            {
                rels.knownRelicTile.Remove(setTile);
            }
        }
        GameManager.Inst.ShowLogBox($"{rel.religion}종교가 성유물 '{setTile.relic}'을 인수하였습니다.");
        setTile.relic = HolyRelic.none;
        HideTileMenu();
    }
    public int BuyRelicCalc(Religion rel)
    {
        float badBonus = setTile.condition == TileCondition.Bad ? 0.8f : 1f;
        float occBonus = 1f;
        if (setTile.mainReligion?.religion == rel.religion)
        {
            occBonus = 0.8f;
        }

        int BasePrice = 100;

        switch (setTile.relic)
        {
            case HolyRelic.GoldenEye:
                BasePrice = 800;
                break;
            case HolyRelic.SaintStaff:
                BasePrice = 750;
                break;
            case HolyRelic.MiracleDice:
                BasePrice = 820;
                break;
            case HolyRelic.GuardCharm:
                BasePrice = 900;
                break;
            case HolyRelic.WrittenStone:
                BasePrice = 680;
                break;
            case HolyRelic.NoWitherFlower:
                BasePrice = 840;
                break;
            case HolyRelic.SaintCrown:
                BasePrice = 640;
                break;
            case HolyRelic.OldMap:
                BasePrice = 880;
                break;
            case HolyRelic.LightBoots:
                BasePrice = 770;
                break;
            case HolyRelic.EternalTorch:
                BasePrice = 630;
                break;
            case HolyRelic.HolyGrail:
                BasePrice = 790;
                break;
            case HolyRelic.BotaniBracelet:
                BasePrice = 1030;
                break;
            case HolyRelic.GodsMirror:
                BasePrice = 740;
                break;
        }

        int RelicPrice = Mathf.RoundToInt(BasePrice * Mathf.Max(1, setTile.TotalMadeCoin) / Mathf.Max(1, AvgTotalMadeCoin) * Turn / MaxTurn * badBonus * occBonus * setTile.findTechBonus);
        return RelicPrice;
    }
    public void Occupy()
    {
        Occupy(ReligionDic[playerRel]);
        ActableCount--;
    }
    public void Occupy(Religion rel)
    {
        if(setTile.Frozen != Religions.none && setTile.FrozenTurn <= Turn)
        {
            return;
        }

        if (Random.Range(0, 101) <= OccupyCalc(rel, setTile))
        {
            if(rel.Tech[4, 2] && OccupyCalc(rel, setTile) < 20)
            {
                for (int i = Mathf.Max(0, setTile.Pos.y - 1); i <= Mathf.Min(GameManager.Inst.tiles.height - 1, setTile.Pos.y + 1); i++)
                {
                    for (int j = Mathf.Max(0, setTile.Pos.x - 1); j <= Mathf.Min(GameManager.Inst.tiles.width - 1, setTile.Pos.x + 1); j++)
                    {
                        if (Mathf.Abs(setTile.Pos.y - i) + Mathf.Abs(setTile.Pos.x - j) == 1)
                        {
                            foreach(Religion r in religions)
                            {
                                if(r != rel && setTile.stability.ContainsKey(r.religion))
                                    setTile.AddStability(r, -10);
                            }
                        }
                    }
                }
            }

            if (rel.Tech[4, 5])
            {
                for (int i = Mathf.Max(0, setTile.Pos.y - 3); i <= Mathf.Min(GameManager.Inst.tiles.height - 1, setTile.Pos.y + 3); i++)
                {
                    for (int j = Mathf.Max(0, setTile.Pos.x - 3); j <= Mathf.Min(GameManager.Inst.tiles.width - 1, setTile.Pos.x + 3); j++)
                    {
                        if (Mathf.Abs(setTile.Pos.y - i) + Mathf.Abs(setTile.Pos.x - j) <= 3)
                        {
                            setTile.autoOcc[rel] += 5;
                        }
                    }
                }
            }
            AddTile(rel, setTile);
            GameManager.Inst.ShowLogBox($"{rel.religion}종교가 {setTile.Pos}타일 점령에 성공하였습니다.");
            if(setTile.relic != HolyRelic.none)
            {
                float bonus = 0.0f;

                for(int x = Mathf.Max(0, setTile.Pos.x - 1); x <= Mathf.Min(width - 1, setTile.Pos.x + 1); x++)
                {
                    for (int y = Mathf.Max(0, setTile.Pos.y - 1); y <= Mathf.Min(height - 1, setTile.Pos.y + 1); y++)
                    {
                        if(Mathf.Abs(setTile.Pos.x - x) + Mathf.Abs(setTile.Pos.y - y) == 1 && tiles[y, x].mainReligion == rel)
                        {
                            bonus += 0.1f;
                        }
                    }
                }

                if(Random.Range(0f, 1f) <= bonus + 0.05f)
                {
                    GetRelic(rel, setTile.relic);
                    foreach (Religion rels in religions)
                    {
                        if (rels.knownRelicTile.Contains(setTile))
                        {
                            rels.knownRelicTile.Remove(setTile);
                        }
                    }
                    GameManager.Inst.ShowLogBox($"{rel.religion}종교가 성유물 '{setTile.relic}'을 획득하였습니다.");
                    setTile.relic = HolyRelic.none;
                }
            }
        }
        else
        {
            GameManager.Inst.ShowLogBox($"{rel.religion}종교가 {setTile.Pos}타일 점령에 실패하였습니다.");
            if (setTile.mainReligion?.religion == usingMiracle[Miracle.FrozenHeart])
            {
                setTile.Frozen = usingMiracle[Miracle.FrozenHeart];
                setTile.FrozenTurn = Turn + 2;
            }
            if(setTile == setTile.mainReligion.SacredPlace && setTile.mainReligion.Tech[5, 2])
            {
                List<Tile> tempList = new();
                for (int i = Mathf.Max(0, setTile.Pos.y - 5); i <= Mathf.Min(GameManager.Inst.tiles.height - 1, setTile.Pos.y + 5); i++)
                {
                    for (int j = Mathf.Max(0, setTile.Pos.x - 5); j <= Mathf.Min(GameManager.Inst.tiles.width - 1, setTile.Pos.x + 5); j++)
                    {
                        if (Mathf.Abs(setTile.Pos.y - i) + Mathf.Abs(setTile.Pos.x - j) <= 5 && tiles[i, j].mainReligion == null)
                        {
                            tempList.Add(tiles[i, j]);
                        }
                    }
                }
                if(tempList.Count > 0)
                {
                    AddTile(setTile.mainReligion, tempList[Random.Range(0, tempList.Count)]);
                }
            }
        }

        HideTileMenu();
        RefreshGetTiles();
    }
    public int OccupyCalc(Religion rel, Tile tile)
    {
        float valueBonus = 1.0f;
        if (rel.Tech[2, 5])
        {
            for (int i = Mathf.Max(0, setTile.Pos.y - 1); i <= Mathf.Min(GameManager.Inst.tiles.height - 1, setTile.Pos.y + 1); i++)
            {
                for (int j = Mathf.Max(0, setTile.Pos.x - 1); j <= Mathf.Min(GameManager.Inst.tiles.width - 1, setTile.Pos.x + 1); j++)
                {
                    if (Mathf.Abs(setTile.Pos.y - i) + Mathf.Abs(setTile.Pos.x - j) == 1 && GameManager.Inst.tiles.tiles[i, j].mainReligion == setTile.mainReligion)
                    {
                        valueBonus += 0.02f;
                    }
                }
            }
        }

        int max = 0;
        foreach (KeyValuePair<Religions, int> r in tile.influence)
        {
            if (r.Key != rel.religion)
            {
                max += r.Value;
            }
        }
        int noWitherFlower = rel.relics.Contains(HolyRelic.NoWitherFlower) ? 5 : 0;
        if (tile.Sacred.gameObject.activeSelf)
        {
            if(usingMiracle[Miracle.BuildWall] != Religions.none && usingMiracle[Miracle.BuildWall] != rel.religion)
            {
                int val = Mathf.RoundToInt(valueBonus * Mathf.RoundToInt(Mathf.Clamp(Mathf.Max(0, (float)tile.influence[rel.religion] - tile.influence[tile.mainReligion.religion]) / MaxInfluence, 0, 100) * 100)) + noWitherFlower;
                return val > 20 ? val : 0;
            }
            else
            {
                return Mathf.RoundToInt(Mathf.RoundToInt(valueBonus * Mathf.Clamp(Mathf.Max(0, (float)tile.influence[rel.religion] - tile.influence[tile.mainReligion.religion]) / MaxInfluence, 0, 100) * 100)) + noWitherFlower;
            }
        }
        else
        {
            if (usingMiracle[Miracle.BuildWall] != Religions.none && usingMiracle[Miracle.BuildWall] != rel.religion)
            {
                int val = Mathf.RoundToInt(Mathf.Clamp((float)tile.influence[rel.religion] / (MaxInfluence + max), 0, 100) * 100) + noWitherFlower;
                return val > 20 ? val : 0;
            }
            else
            {
                return Mathf.RoundToInt(Mathf.Clamp((float)tile.influence[rel.religion] / (MaxInfluence + max), 0, 100) * 100) + noWitherFlower;
            }
        }
    }
    public void ShowTileInfoTab()
    {
        GameManager.Inst.infoTab.SetUp(setTile);
    }
    public void RefreshTiles()
    {
        foreach(Religion rels in religions)
        {
            float totalCoin = 0.0f;
            float totalSunlight = 0.0f;

            foreach(Tile tile in rels.GetTiles)
            {
                totalCoin += tile.ProCoin;
                totalSunlight += tile.ProSunlight;
            }

            rels.totalCoin = Mathf.RoundToInt(totalCoin);
            rels.avgCoin = Mathf.RoundToInt(totalCoin / rels.GetTiles.Count);
            rels.totalSunlight = Mathf.RoundToInt(totalSunlight);
            rels.avgSunlight = Mathf.RoundToInt(totalSunlight / rels.GetTiles.Count);
        }
    }
    public void RefreshGetTiles()
    {
        for(int i = 0; i<religionsOrder.Count; i++)
        {
            Religion r = ReligionDic[religionsOrder[i]];
            r.GetTiles = r.GetTiles.Distinct().ToList();
            ReligionsCounters.transform.GetChild(i).GetComponentInChildren<TextMeshProUGUI>().text = r.GetTiles.Count.ToString();
        }
        RefreshTiles();
    }
    public void GetRelic(Religion rel, HolyRelic relic)
    {
        rel.relics.Add(relic);
        holyRelics.Remove(relic);

        Relic R = null;
        if (rel.religion == playerRel)
        {
            GameObject obj = Instantiate(RelicDic[relic], playerRelics.transform);
            obj.name = relic.ToString();
            R = obj.GetComponent<Relic>();
            R.owner = rel;
            obj.transform.localScale = Vector3.one;
            PositionFix(obj.transform);
        }
        else
        {
            GameObject obj = Instantiate(RelicDic[relic], otherRelics.transform);
            obj.name = relic.ToString();
            R = obj.GetComponent<Relic>();
            R.owner = rel;
            obj.transform.localScale = Vector3.one;
            PositionFix(obj.transform);
        }

        if (relic == HolyRelic.NoWitherFlower)
        {
            R.RunAbility();
        }
        else if(relic == HolyRelic.EternalTorch)
        {
            if(rel == ReligionDic[playerRel])
            {
                EternalTorchGet = true;
            }
            else
            {
                List<Tile> hideTile = new();
                foreach(Tile t in tiles)
                {
                    if(!rel.TorchedTile.Contains(t))
                    {
                        hideTile.Add(t);
                    }
                }
                if(hideTile.Count > 0)
                {
                    (R as EternalTorch).tile = hideTile[Random.Range(0, hideTile.Count)];
                }
            }
        }
        else if(relic == HolyRelic.LightBoots)
        {
            FindObjectOfType<LightBoots>().RunAbility();
        }
        else if(relic == HolyRelic.GodsMirror)
        {
            FindObjectOfType<GodsMirror>().RunAbility();
        }
        else if(relic == HolyRelic.SaintCrown)
        {
            FindObjectOfType<SaintCrown>().RunAbility();
        }
        else if(relic == HolyRelic.BotaniBracelet)
        {
            FindObjectOfType<BotaniBracelet>().RunAbility();
        }
        if (rel.Tech[5, 5])
        {
            rel.TechValue[5, 5] = Mathf.RoundToInt(rel.Ideal * 0.2f) * 100 + 10;
        }

        rel.TechPoint++;
    }
    public void PositionFix(Transform tr)
    {
        tr.GetComponent<RectTransform>().anchoredPosition3D = new(tr.position.x, tr.position.y, 0);
    }
    public void NextOrder(Religions rel)
    {
        AutoOccupy(ReligionDic[rel]);
        ReturnTurn(TurnRel);
        if (aiPlay != null)
        {
            StopCoroutine(aiPlay);
            aiPlay = null;
        }

        if (rel == religionsOrder[^1])
        {
            TurnEnd();
            TurnRel = playerRel;
        }
        else
        {
            for(int i = 0; i<religionsOrder.Count; i++)
            {
                if(rel == religionsOrder[i])
                {
                    TurnRel = religionsOrder[i + 1];
                    break;
                }
            }
        }

        ActableCount = RandomActable(ReligionDic[TurnRel]);
        if(FixedFate > 0 && usingMiracle[Miracle.ForegoneFate] == TurnRel)
        {
            ActableCount = FixedFate;
            FixedFate = 0;
            BlockedFate = 0;
        }

        if (ReligionDic[TurnRel].isSnakeVenom)
        {
            GameManager.Inst.ShowLogBox($"{TurnRel} 종교는 뱀독으로 인해 턴을 넘깁니다.");
            NextOrder(TurnRel);
            ReligionDic[TurnRel].isSnakeVenom = false;
            return;
        }

        if (TurnRel != playerRel || autoPlay)
        {
            aiPlay = StartCoroutine(AIAct());
        }
    }
    public IEnumerator AIAct()
    {
        Religion Rel = ReligionDic[TurnRel];
        UseMiracles(AIMiracleSel());
        if(Rel.TechPoint > 0)
        {
            Vector2Int techVec = new(-1, -1);
            if(Rel.RecentTech.x < 0)
            {
                techVec = new Vector2Int(Random.Range(0, 7), Random.Range(0, 7));
                GetTech(Rel, techVec);
            }
            else
            {
                for(int i = 0; i < Rel.Tech.GetLength(0); i++)
                {
                    if (!Rel.Tech[Rel.RecentTech.x, i] && Random.Range(0f, 1f) <= 0.8f)
                    {
                        do
                        {
                            techVec = new(Rel.RecentTech.x, Random.Range(0, 7));
                        } while (Rel.Tech[techVec.x, techVec.y]);
                        GetTech(Rel, techVec);
                        goto techSet;
                    }
                }
                List<Vector2Int> randVec = new();
                for(int i =0; i<Rel.Tech.GetLength(0); i++)
                {
                    if(i == Rel.RecentTech.x)
                    {
                        continue;
                    }
                    for(int j = 0; j<Rel.Tech.GetLength(1); j++)
                    {
                        if (!Rel.Tech[i, j])
                        {
                            randVec.Add(new(i, j));
                        }
                    }
                }
                if (randVec.Count == Rel.Tech.Length - 12 && Random.Range(0f, 1f) <= 0.8f)
                {
                    techVec = randVec[Random.Range(0, randVec.Count)];
                    GetTech(Rel, techVec);
                    goto techSet;
                }
                if(randVec.Count > 0)
                {
                    techVec = randVec[Random.Range(0, randVec.Count)];
                    goto techSet;
                }
                else if(System.Array.IndexOf(Rel.Tech, false) > -1)
                {
                    do
                    {
                        techVec = new(Random.Range(0, 6), Random.Range(0, 6));
                    } while (Rel.Tech[techVec.x, techVec.y]);
                    GetTech(Rel, techVec);
                    goto techSet;
                }
                else
                {
                    goto aiContent;
                }

            }
            techSet:
            Rel.RecentTech = techVec;
            Rel.TechPoint--;
        }

        aiContent:

        //제외 타일 리스트 생성
        List<Tile> excludeTile = new();

        //최대 루프값 적용
        int loop = 30;

        //이전 행동 타일 저장용 변수
        Tile saveTile = null;

        //행동 횟수가 다할 때까지 반복
        do
        {
            int charityCoin = Mathf.RoundToInt(Rel.Coin * Random.Range(0.03f, 0.05f));
            RefreshGetTiles();
            if (Rel.GetTiles.Count == tiles.Length)
            {
                Time.timeScale = 0.0f;
                Debug.Log("전체 점령");
            }
            loop--;
            //성유물 인수
            for (int rT = 0; rT < Rel.knownRelicTile.Count; rT++)
            {
                setTile = Rel.knownRelicTile[rT];
                if (BuyRelicCalc(Rel) <= Rel.Coin * Random.Range(0.6f, 0.8f))
                {
                    BuyRelic(Rel);
                }
            }

            //행동 방향 선택 (+-,+-)
            int xDir, yDir;
            if (RecoredDir.ContainsKey(TurnRel))
            {
                if (Random.Range(0f, 1f) <= 0.5f - RecoredDir[TurnRel][2] * 0.05f)
                {
                    xDir = RecoredDir[TurnRel][0];
                }
                else
                {
                    xDir = RecoredDir[TurnRel][0] * -1;
                }

                if (Random.Range(0f, 1f) <= 0.5f - RecoredDir[TurnRel][3] * 0.05f)
                {
                    yDir = RecoredDir[TurnRel][1];
                }
                else
                {
                    yDir = RecoredDir[TurnRel][1] * -1;
                }
            }
            else
            {
                xDir = Random.Range(0, 2) == 0 ? 1 : -1;
                yDir = Random.Range(0, 2) == 0 ? 1 : -1;
            }

            //루프 대기값이 0일 경우 비점령 무작위 타일 선택
            if (loop == 0)
            {
                List<Tile> notMine = new();
                foreach (Tile tile in Rel.TorchedTile)
                {
                    if (tile.mainReligion.religion != TurnRel)
                    {
                        notMine.Add(tile);
                    }
                }
                setTile = notMine[Random.Range(0, notMine.Count)];
                saveTile = setTile;
            }

            //저장된 타일이 없을 경우 새 타일 선택 후 행동
            if (saveTile == null)
            {
                //선택된 방향의 모든 타일 선택
                List<Tile> dirTile = new();
                foreach (Tile selDirTile in Rel.TorchedTile)
                {
                    Vector2Int PosDiff = selDirTile.Pos - Rel.SacredPlace.Pos;
                    if (((Mathf.Sign(PosDiff.x) == xDir || PosDiff.x == 0) && Mathf.Sign(PosDiff.y) == yDir) || ((Mathf.Sign(PosDiff.y) == yDir || PosDiff.y == 0) && Mathf.Sign(PosDiff.x) == xDir))
                    {
                        dirTile.Add(selDirTile);
                    }
                }

                //밝혀진 타일 개수에 반비례 확률로 맵 밝히기
                if (Random.Range(0f, 1f) <= ((float)tiles.Length - Rel.TorchedTile.Count) / tiles.Length)
                {
                    //우선적으로 맵을 밝히는 경우
                    List<Tile> moreThanTwoEmpty = new();
                    foreach (Tile farTile in dirTile)
                    {
                        //제외 타일에 존재할 경우 건너뜀
                        if (excludeTile.Contains(farTile))
                        {
                            continue;
                        }
                        int empty = 0;

                        //인접한 타일이 몇개나 안개에 가려져 있는지에 따라 해당 타일의 가치를 계산
                        for (int x = Mathf.Max(0, farTile.Pos.x - 1); x <= Mathf.Min(width - 1, farTile.Pos.x + 1); x++)
                        {
                            for (int y = Mathf.Max(0, farTile.Pos.y - 1); y <= Mathf.Min(height - 1, farTile.Pos.y + 1); y++)
                            {
                                if (tiles[y, x].fogBool[TurnRel].Count == 0 && Mathf.Abs(farTile.Pos.x - x) + Mathf.Abs(farTile.Pos.y - y) == 1)
                                {
                                    empty++;
                                }
                            }
                        }

                        //인접한 안개 타일이 2개 이상일 경우 차출
                        if (empty >= 2)
                            moreThanTwoEmpty.Add(farTile);
                    }

                    //인접한 안개 타일 2개 이상인 리스트가 존재할 경우
                    if (moreThanTwoEmpty.Count > 0)
                    {
                        //타일 리스트 셔플링
                        for (int i = 0; i < moreThanTwoEmpty.Count; i++)
                        {
                            int j = Random.Range(0, moreThanTwoEmpty.Count);
                            (moreThanTwoEmpty[i], moreThanTwoEmpty[j]) = (moreThanTwoEmpty[j], moreThanTwoEmpty[i]);
                        }

                        //점령확률 비교용 변수 생성
                        int occPer = 0;

                        //자동 점령 미만의 가장 점령확률이 높은 타일 선택
                        foreach (Tile bestTile in moreThanTwoEmpty)
                        {
                            int occper = OccupyCalc(Rel, bestTile);
                            if (occper < Rel.AutoOccupy - bestTile.autoOcc[Rel] && occPer <= occper)
                            {
                                occPer = occper;
                                setTile = bestTile;
                            }
                        }

                        //선택된 타일의 점령확률이 적당히 높다면 점령 시도
                        if (occPer >= Random.Range(50, Mathf.RoundToInt((Rel.AutoOccupy - setTile.autoOcc[Rel]) * 0.78f) + 1) && Random.Range(0f, 1f) <= (occPer - 45) * 0.04f && setTile.mainReligion != Rel && setTile.FrozenTurn == 0)
                        {
                            Occupy(Rel);
                        }
                        else
                        {
                            //선택된 타일에서의 영향력이 최대일 경우
                            if (setTile.influence[TurnRel] == MaxInfluence)
                            {
                                //해당 타일에서의 모든 종교의 영향력을 순회
                                foreach (KeyValuePair<Religions, int> inf in setTile.influence)
                                {
                                    //타 종교의 영향력이 1이상이며 난수값이 논쟁 승리 확률보다 높다면 논쟁 시도
                                    if (inf.Key != TurnRel && inf.Value > 0 && ArgumentCalc(Rel, ReligionDic[inf.Key]) >= Random.Range(0f, 1f))
                                    {
                                        Argument(Rel, ReligionDic[inf.Key]);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                //선택된 타일에서의 영향력이 최대가 아닐 경우 보유 코인 5% 기부의 효율과 전도의 효율을 비교하여 선택
                                if (CharityCalc(Rel, charityCoin) > EvanGelizeCalc(Rel))
                                {
                                    Charity(Rel, charityCoin);
                                }
                                else
                                {
                                    Evangelize(Rel);
                                }
                            }
                        }
                    }
                    else
                    {
                        //선택된 방향에서의 인접한 안개 타일이 2개 이상인 타일이 없을 경우

                        //종교의 밝혀진 모든 타일 선택
                        foreach (Tile otherdirTile in Rel.TorchedTile)
                        {
                            //선택된 방향을 제외한 다른 모든 타일 중
                            if (!dirTile.Contains(otherdirTile))
                            {
                                int empty = 0;
                                //인접한 안개 타일이 2개 이상인 경우 리스트화
                                for (int x = Mathf.Max(0, otherdirTile.Pos.x - 1); x <= Mathf.Min(width - 1, otherdirTile.Pos.x + 1); x++)
                                {
                                    for (int y = Mathf.Max(0, otherdirTile.Pos.y - 1); y <= Mathf.Min(height - 1, otherdirTile.Pos.y + 1); y++)
                                    {
                                        if (tiles[y, x].fogBool[TurnRel].Count == 0 && Mathf.Abs(otherdirTile.Pos.x - x) + Mathf.Abs(otherdirTile.Pos.y - y) == 1)
                                        {
                                            empty++;
                                        }
                                    }
                                }

                                if (empty >= 2)
                                    moreThanTwoEmpty.Add(otherdirTile);
                            }
                        }

                        //점령확률 비교용 변수 생성
                        int occPer = 0;

                        //자동 점령 미만의 가장 점령확률이 높은 타일 선택
                        foreach (Tile bestTile in moreThanTwoEmpty)
                        {
                            int occper = OccupyCalc(Rel, bestTile);
                            if (occper < Rel.AutoOccupy && occPer <= occper)
                            {
                                occPer = occper;
                                setTile = bestTile;
                            }
                        }

                        if (setTile == null)
                        {
                            foreach (Tile bestTile in Rel.TorchedTile)
                            {
                                int occper = OccupyCalc(Rel, bestTile);
                                if (occper < Rel.AutoOccupy - bestTile.autoOcc[Rel] && occPer <= occper)
                                {
                                    occPer = occper;
                                    setTile = bestTile;
                                }
                            }
                        }

                        //선택된 타일의 점령확률이 적당히 높다면 점령 시도
                        if (occPer >= Random.Range(50, Mathf.RoundToInt((Rel.AutoOccupy - setTile.autoOcc[Rel])* 0.78f) + 1) && Random.Range(0f, 1f) <= (occPer - 45) * 0.04f && setTile.mainReligion != Rel && setTile.FrozenTurn == 0)
                        {
                            Occupy(Rel);
                        }
                        else
                        {
                            //선택된 타일에서의 영향력이 최대일 경우
                            if (setTile.influence[TurnRel] == MaxInfluence)
                            {
                                //해당 타일에서의 모든 종교의 영향력을 순회
                                foreach (KeyValuePair<Religions, int> inf in setTile.influence)
                                {
                                    //타 종교의 영향력이 1이상이며 난수값이 논쟁 승리 확률보다 높다면 논쟁 시도
                                    if (inf.Key != TurnRel && inf.Value > 0 && ArgumentCalc(Rel, ReligionDic[inf.Key]) >= Random.Range(0f, 1f))
                                    {
                                        Argument(Rel, ReligionDic[inf.Key]);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                //선택된 타일에서의 영향력이 최대가 아닐 경우 보유 코인 5% 기부의 효율과 전도의 효율을 비교하여 선택
                                if (CharityCalc(Rel, charityCoin) > EvanGelizeCalc(Rel))
                                {
                                    Charity(Rel, charityCoin);
                                }
                                else
                                {
                                    Evangelize(Rel);
                                }
                            }
                        }
                    }
                }
                else
                {
                    //밝혀진 타일이 많을 경우
                    int occPer = 0;

                    //종교 타일을 합산
                    int religionTileCount = 0;
                    foreach (Religion allRel in religions)
                    {
                        religionTileCount += allRel.GetTiles.Count;
                    }

                    //행동 확률을 위한 난수 생성
                    float actRand = Random.Range(0f, 1f);
                    float undiscoverdRelic = Rel.knownRelicTile.Count < holyRelics.Count ? 0.05f : 0f;

                    //세력 억제, 세력 확장, 성유물 조사를 확률에 따라 진행
                    if (undiscoverdRelic + 0.45f - Mathf.Max(0, (Mathf.RoundToInt(tiles.Length * 0.6f) - religionTileCount) * (0.2f / tiles.Length)) >= actRand)
                    {
                        //세력 억제
                        Religion supTarget = null;
                        int tileCount = 0;

                        //가장 세력이 큰 종교 선택
                        foreach (Religion tileCountRel in religions)
                        {
                            if (tileCountRel.religion != TurnRel && tileCount < tileCountRel.GetTiles.Count)
                            {
                                tileCount = tileCountRel.GetTiles.Count;
                                supTarget = tileCountRel;
                            }
                        }

                        if (supTarget != null)
                        {
                            //지정한 세력에 대한 타일과 그 중 점령되지 않은 타일 리스트화
                            List<Tile> TargetsTiles = new();
                            List<Tile> unoccupiedTiles = new();
                            foreach (Tile includeTile in tiles)
                            {
                                if (includeTile.influence[supTarget.religion] > 0)
                                {
                                    TargetsTiles.Add(includeTile);
                                    if (includeTile.mainReligion.religion == Religions.none)
                                    {
                                        unoccupiedTiles.Add(includeTile);
                                    }
                                }
                            }

                            //타일 리스트 셔플링
                            for (int i = 0; i < TargetsTiles.Count; i++)
                            {
                                int j = Random.Range(0, TargetsTiles.Count);
                                (TargetsTiles[i], TargetsTiles[j]) = (TargetsTiles[j], TargetsTiles[i]);
                            }
                            for (int i = 0; i < unoccupiedTiles.Count; i++)
                            {
                                int j = Random.Range(0, unoccupiedTiles.Count);
                                (unoccupiedTiles[i], unoccupiedTiles[j]) = (unoccupiedTiles[j], unoccupiedTiles[i]);
                            }

                            //점령되지 않은 타일 개수에 비례하여 타일 선택
                            if (Random.Range(0f, 1f) <= (tiles.Length - religionTileCount) / tiles.Length * 2.5f)
                            {
                                //가장 점령확률이 높은 타일 선택
                                foreach (Tile bestTile in unoccupiedTiles)
                                {
                                    int occper = OccupyCalc(Rel, bestTile);
                                    if (occper < Rel.AutoOccupy - bestTile.autoOcc[Rel] && occPer < occper)
                                    {
                                        occPer = occper;
                                        setTile = bestTile;
                                    }
                                }
                            }
                            else
                            {
                                //가장 점령확률이 높은 타일 선택
                                foreach (Tile bestTile in TargetsTiles)
                                {
                                    int occper = OccupyCalc(Rel, bestTile);
                                    if (occper < Rel.AutoOccupy - bestTile.autoOcc[Rel] && occPer < occper)
                                    {
                                        occPer = occper;
                                        setTile = bestTile;
                                    }
                                }
                            }

                            if (setTile == null)
                            {
                                setTile = dirTile[Random.Range(0, dirTile.Count)];
                            }
                        }

                        //지정타일이 없다면 선택된 방향의 무작위 타일을 선택
                        if (setTile == null)
                        {
                            setTile = dirTile[Random.Range(0, dirTile.Count)];
                        }

                        //점령 확률이 적당히 높다면 점령 시도 아닐 경우 전도 혹은 기부
                        if (occPer >= Random.Range(50, Mathf.RoundToInt((Rel.AutoOccupy - setTile.autoOcc[Rel]) * 0.78f) + 1) && Random.Range(0f, 1f) <= (occPer - 45) * 0.04f && setTile.mainReligion != Rel && setTile.FrozenTurn == 0)
                        {
                            Occupy(Rel);
                        }
                        else
                        {
                            //논쟁 선택, 보유 코인 5% 기부의 효율과 전도의 효율을 비교하여 선택
                            if (setTile.influence[TurnRel] == MaxInfluence)
                            {
                                foreach (KeyValuePair<Religions, int> inf in setTile.influence)
                                {
                                    if (inf.Key != TurnRel && inf.Value > 0 && ArgumentCalc(Rel, ReligionDic[inf.Key]) >= Random.Range(0f, 1f))
                                    {
                                        Argument(Rel, ReligionDic[inf.Key]);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                if (CharityCalc(Rel, charityCoin) > EvanGelizeCalc(Rel))
                                {
                                    Charity(Rel, charityCoin);
                                }
                                else
                                {
                                    Evangelize(Rel);
                                }
                            }
                        }
                    }
                    else if (undiscoverdRelic + 0.45f + Mathf.Max(0, (Mathf.RoundToInt(tiles.Length * 0.6f) - religionTileCount) * (0.2f / tiles.Length)) >= actRand)
                    {
                        //세력 확장
                        //제외 타일 리스트 생성
                        excludeTile.Clear();

                        //타일 리스트 셔플링
                        for (int i = 0; i < dirTile.Count; i++)
                        {
                            int j = Random.Range(0, dirTile.Count);
                            (dirTile[i], dirTile[j]) = (dirTile[j], dirTile[i]);
                        }

                        //가장 점령확률이 높은 타일 선택
                        foreach (Tile bestTile in dirTile)
                        {
                            int occper = OccupyCalc(Rel, bestTile);
                            if (occper < Rel.AutoOccupy - bestTile.autoOcc[Rel] && occPer <= occper)
                            {
                                occPer = occper;
                                setTile = bestTile;
                            }
                        }

                        if (setTile == null)
                        {
                            setTile = dirTile[Random.Range(0, dirTile.Count)];
                        }

                        //점령 확률이 적당히 높다면 점령 시도 아닐 경우 전도 혹은 기부
                        if (occPer >= Random.Range(50, Mathf.RoundToInt((Rel.AutoOccupy - setTile.autoOcc[Rel]) * 0.78f) + 1) && Random.Range(0f, 1f) <= (occPer - 45) * 0.04f && setTile.mainReligion != Rel && setTile.FrozenTurn == 0)
                        {
                            Occupy(Rel);
                        }
                        else
                        {
                            //보유 코인 5% 기부의 효율과 전도의 효율을 비교하여 선택
                            if (setTile.influence[TurnRel] == MaxInfluence)
                            {
                                bool arguTry = false;
                                foreach (KeyValuePair<Religions, int> inf in setTile.influence)
                                {
                                    if (inf.Key != TurnRel && inf.Value > 0 && ArgumentCalc(Rel, ReligionDic[inf.Key]) >= Random.Range(0f, 1f))
                                    {
                                        arguTry = true;
                                        Argument(Rel, ReligionDic[inf.Key]);
                                        break;
                                    }
                                }
                                if (!arguTry)
                                {
                                    excludeTile.Add(setTile);
                                    if (occPer >= Rel.AutoOccupy - setTile.autoOcc[Rel])
                                    {
                                        setTile = null;
                                    }
                                }
                            }
                            else
                            {
                                if (CharityCalc(Rel, charityCoin) > EvanGelizeCalc(Rel))
                                {
                                    Charity(Rel, charityCoin);
                                }
                                else
                                {
                                    Evangelize(Rel);
                                }
                            }
                        }
                    }
                    else
                    {
                        //성유물 조사
                        //타일 리스트 셔플링
                        for (int i = 0; i < dirTile.Count; i++)
                        {
                            int j = Random.Range(0, dirTile.Count);
                            (dirTile[i], dirTile[j]) = (dirTile[j], dirTile[i]);
                        }

                        //선택된 타일 성유물 조사
                        foreach (Tile researchTile in dirTile)
                        {
                            if (Rel.knownRelicTile.Contains(researchTile) || researchTile.serveidRel[Rel])
                            {
                                continue;
                            }
                            setTile = researchTile;
                            SurveyRelic(Rel);
                            saveTile = null;
                            break;
                        }
                    }
                }
            }
            else
            {
                setTile = saveTile;
                int occPer = OccupyCalc(Rel, setTile);

                //점령 확률이 적당히 높다면 점령 시도 아닐 경우 전도 혹은 기부
                if (occPer >= Random.Range(50, Mathf.RoundToInt((Rel.AutoOccupy - setTile.autoOcc[Rel]) * 0.78f) + 1) && Random.Range(0f, 1f) <= (occPer - 45) * 0.04f && setTile.mainReligion != Rel && setTile.FrozenTurn == 0)
                {
                    Occupy(Rel);
                }
                else
                {
                    //논쟁 선택, 보유 코인 5% 기부의 효율과 전도의 효율을 비교하여 선택
                    if (setTile.influence[TurnRel] == MaxInfluence)
                    {
                        foreach (KeyValuePair<Religions, int> inf in setTile.influence)
                        {
                            if (inf.Key != TurnRel && inf.Value > 0 && ArgumentCalc(Rel, ReligionDic[inf.Key]) >= Random.Range(0f, 1f))
                            {
                                Argument(Rel, ReligionDic[inf.Key]);
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (CharityCalc(Rel, charityCoin) > EvanGelizeCalc(Rel))
                        {
                            Charity(Rel, charityCoin);
                        }
                        else
                        {
                            Evangelize(Rel);
                        }
                    }
                }
            }

            //해당 타일을 점령하지 못하였을 경우 saveTile에 저장
            if (setTile != null)
            {
                if (setTile.mainReligion != Rel)
                {
                    saveTile = setTile;
                }
                else
                {
                    saveTile = null;
                }
            }

            ActableCount--;
            //행동 방향 기록
            if (!RecoredDir.ContainsKey(TurnRel))
            {
                RecoredDir.Add(TurnRel, new int[4] { xDir, yDir, 1, 1 });
            }
            else
            {
                int[] DirRecord = new int[4];
                if (RecoredDir[TurnRel][0] == xDir)
                {
                    DirRecord[0] = xDir;
                    DirRecord[2]++;
                }
                else
                {
                    DirRecord[0] = xDir;
                    DirRecord[2] = 1;
                }
                if (RecoredDir[TurnRel][1] == yDir)
                {
                    DirRecord[1] = yDir;
                    DirRecord[3]++;
                }
                else
                {
                    DirRecord[1] = yDir;
                    DirRecord[3] = 1;
                }
                RecoredDir[TurnRel] = DirRecord;
            }

            yield return actDelay;
        } while (ActableCount > 1);
        
        NextOrder(TurnRel);
        setTile = null;
    }
    public Miracle AIMiracleSel()
    {
        float max = 0;
        List<float> miracleSelect = new();
        foreach(Miracle miracle in ReligionDic[TurnRel].miracle)
        {
            switch(miracle)
            {
                case Miracle.RisingSun:
                    max += 0.15f;
                    break;
                case Miracle.SnakeVenom:
                    max += 0.2f;
                    break;
                case Miracle.CompulsoryExcution:
                    max += 0.18f;
                    break;
                case Miracle.ForegoneFate:
                    max += 0.34f;
                    break;
                case Miracle.PropertyGrowth:
                    max += 0.17f;
                    break;
                case Miracle.BuildWall:
                    max += 0.27f;
                    break;
                case Miracle.Prophecy:
                    max += 0.5f;
                    break;
                case Miracle.Judgement:
                    max += 0.21f;
                    break;
                case Miracle.ForesightDream:
                    max += 1f;
                    break;
                case Miracle.FrozenHeart:
                    max += 0.22f;
                    break;
            }
            miracleSelect.Add(max);
        }
        do
        {
            float Rand = Random.Range(0f, max + 0.3f);
            miracleSelect.Add(0.3f);
            int sel = 0;
            for (int i = 0; i < miracleSelect.Count; i++)
            {
                if (Rand <= miracleSelect[i])
                {
                    sel = i;
                }
            }
            if (sel == miracleSelect.Count - 1)
            {
                return Miracle.none;
            }
            else
            {
                int i = 0;
                foreach(string miracleName in System.Enum.GetNames(typeof(Miracle)))
                {
                    if(miracleName == System.Enum.GetName(typeof(Miracle), ReligionDic[TurnRel].miracle[sel]))
                    {
                        if (MiraclePrice[i-1] <= ReligionDic[TurnRel].Sunlight)
                        {
                            ReligionDic[TurnRel].Sunlight -= MiraclePrice[i - 1];
                            return ReligionDic[TurnRel].miracle[sel];
                        }
                        break;
                    }
                    i++;
                }
            }
        } while (true);
    }
    public void UseMiracles(Miracle miracle)
    {
        if (miracle != Miracle.none)
            GameManager.Inst.ShowLogBox($"{TurnRel.ToString()}종교가 {miracle.ToString()} 기적을 사용");

        switch (miracle)
        {
            case Miracle.RisingSun:
                RisingSun();
                break;
            case Miracle.SnakeVenom:
                StartCoroutine(SnakeVenom());
                break;
            case Miracle.CompulsoryExcution:
                CompulsoryExcution();
                break;
            case Miracle.ForegoneFate:
                StartCoroutine(ForegoneFate());
                break;
            case Miracle.ConcealedFuture:
                StartCoroutine(ConcealedFuture());
                break;
            case Miracle.PropertyGrowth:
                PropertyGrowth();
                break;
            case Miracle.Charisma:
                Charisma();
                break;
            case Miracle.BuildWall:                
                BuildWall();
                break;
            case Miracle.Prophecy:
                Prophecy();
                break;
            case Miracle.Judgement:
                Judgement();
                break;
            case Miracle.ForesightDream:
                ForesightDream();
                break;
            case Miracle.FrozenHeart:
                FrozenHeart();
                break;
        }

        if(usingMiracle.ContainsKey(miracle))
        {
            usingMiracle[miracle] = TurnRel;
        }
        else
        {
            usingMiracle.Add(miracle, TurnRel);
        }

        if(TurnRel == playerRel)
        {
            miracleMenu.SetActive(false);
            tileSelectable.Remove(true);
        }
    }
    public void UseMiracle(GameObject obj)
    {
        UseMiracles((Miracle)System.Enum.Parse(typeof(Miracle), obj.name));
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
            if (ReligionDic[playerRel].miracle.Contains((Miracle)System.Enum.Parse(typeof(Miracle), miracle.name)))
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
            Debug.Log(rel.religion);
            if (rel.religion != playerRel)
            {
                GameObject obj = Instantiate(miracleTargetPrefab, miracleTarget.transform);

                obj.name = rel.religion.ToString();
                obj.GetComponent<Button>().onClick.AddListener(() => { miracleTargetRel = rel; miracleTarget.SetActive(false); tileSelectable.Remove(true); });
                obj.transform.Find("Symbol").GetComponent<Image>().sprite = rel.symbol;
                obj.transform.Find("RelName").GetComponent<TextMeshProUGUI>().text = rel.religion.ToString();
            }
        }
    }
    public void RisingSun()
    {
        List<Tile> rise = new();
        foreach(Tile risingTile in ReligionDic[TurnRel].GetTiles)
        {
            if(risingTile.condition != TileCondition.Good)
            {
                rise.Add(risingTile);
            }
        }
        for(int i = 0; i<rise.Count; i++)
        {
            int j = Random.Range(0, rise.Count);
            (rise[i], rise[j]) = (rise[j], rise[i]);
        }
        for(int i =0; i<Mathf.Min(10, rise.Count); i++)
        {
            rise[i].SetCondition(TileCondition.Good);
            rise[i].RisingTurn = 1;
        }
        MiracleTurn[Miracle.RisingSun] = 1;
    }
    public IEnumerator SnakeVenom()
    {
        if (TurnRel == playerRel)
        {
            miracleTarget.SetActive(true);
            ShowMiracleTarget();

            while(miracleTarget.activeSelf)
            {
                yield return null;
            }
            miracleTargetRel.isSnakeVenom = true;
            miracleTargetRel = null;
        }
        else
        {
            int SnakeVenom_max = 0;
            List<int> SnakeVenom_count = new();
            List<Religions> SnakeVenom_tmpRel = new();
            foreach (Religions religions in religionsOrder)
            {
                if (religions != TurnRel)
                {
                    SnakeVenom_count.Add(SnakeVenom_max + ReligionDic[religions].GetTiles.Count);
                    SnakeVenom_tmpRel.Add(religions);
                    SnakeVenom_max += ReligionDic[religions].GetTiles.Count;
                }
            }
            int SnakeVenom_rand = Random.Range(0, SnakeVenom_max + 1);
            int SnakeVenom_j = 0;
            foreach (int snakevenom in SnakeVenom_count)
            {
                if (SnakeVenom_rand <= snakevenom)
                {
                    ReligionDic[SnakeVenom_tmpRel[SnakeVenom_j]].isSnakeVenom = true;
                    break;
                }
                SnakeVenom_j++;
            }
        }
        MiracleTurn[Miracle.SnakeVenom] = 1;
    }
    public void CompulsoryExcution()
    {
        MiracleTurn[Miracle.CompulsoryExcution] = 1;
    }
    public IEnumerator ForegoneFate()
    {
        if(TurnRel == playerRel)
        {
            ForegoneFateUI.SetActive(true);
            tileSelectable.Add(true);
            ForegoneFateUI.transform.Find("Value").GetComponent<TextMeshProUGUI>().text = $"{RandomActable(ReligionDic[playerRel])}";

            while(ForegoneFateUI.activeSelf)
            {
                yield return null;
            }
            tileSelectable.Remove(true);
        }
        else
        {
            float foregone = Random.Range(0f, 1f);
            switch (RandomActable(ReligionDic[TurnRel]))
            {
                case 1:
                    BlockedFate = 1;
                    break;
                case 2:
                    if (foregone <= 0.2f)
                    {
                        BlockedFate = 2;
                    }
                    break;
                case 3:
                    if (foregone <= 0.3f)
                    {
                        BlockedFate = 3;
                    }
                    else if (foregone > 0.6f)
                    {
                        FixedFate = 3;
                    }
                    break;
                case 4:
                    if (foregone <= 0.1f)
                    {
                        BlockedFate = 4;
                    }
                    else if (foregone > 0.8f)
                    {
                        FixedFate = 4;
                    }
                    break;
                case 5:
                    if (foregone <= 0.02f)
                    {
                        BlockedFate = 5;
                    }
                    else if (foregone > 0.9f)
                    {
                        FixedFate = 5;
                    }
                    break;
                case 6:
                    FixedFate = 6;
                    break;
            }
        }

        MiracleTurn[Miracle.ForegoneFate] = 1;
    }
    public void FixFate()
    {
        ForegoneFateUI.SetActive(false);
        FixedFate = (System.Convert.ToInt32(ForegoneFateUI.transform.Find("Value").GetComponent<TextMeshProUGUI>().text));
    }
    public void ExcFate()
    {
        ForegoneFateUI.SetActive(false);
        BlockedFate = (System.Convert.ToInt32(ForegoneFateUI.transform.Find("Value").GetComponent<TextMeshProUGUI>().text));
    }
    public IEnumerator ConcealedFuture()
    {
        Tile tile = null;
        if(TurnRel == playerRel)
        {
            while(tile == null)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 1, 1 << 6);

                    if (hit.collider != null)
                    {
                        tile = hit.collider.GetComponent<Tile>();
                    }
                }
                yield return null;

            }
            tile.nextCondition = TileCondition.Bad;
        }
        else
        {
            MiracleTurn[Miracle.ConcealedFuture] = 1;
            int ConcealedFuture_max = 0;
            List<int> ConcealedFuture_count = new();
            List<Religions> ConcealedFuture_tmpRel = new();
            foreach (Religions religions in religionsOrder)
            {
                if (religions != TurnRel)
                {
                    ConcealedFuture_count.Add(ConcealedFuture_max + ReligionDic[religions].GetTiles.Count);
                    ConcealedFuture_tmpRel.Add(religions);
                    ConcealedFuture_max += ReligionDic[religions].GetTiles.Count;
                }
            }
            int ConcealedFuture_rand = Random.Range(0, ConcealedFuture_max + 1);
            int ConcealedFuture_j = 0;
            foreach (int concealedfuture in ConcealedFuture_count)
            {
                if (ConcealedFuture_rand <= concealedfuture)
                {
                    Tile ConcealedTargetTile = ReligionDic[ConcealedFuture_tmpRel[ConcealedFuture_j]].GetTiles[Random.Range(0, ReligionDic[ConcealedFuture_tmpRel[ConcealedFuture_j]].GetTiles.Count)];
                    tile = ConcealedTargetTile;
                    break;
                }
                ConcealedFuture_j++;
            }
        }

        tile.nextCondition = TileCondition.Bad;
    }
    public void PropertyGrowth()
    {
        float avg = 0;
        foreach(Tile tile in ReligionDic[TurnRel].GetTiles)
        {
            avg += tile.MakeCoin;
        }
        avg /= ReligionDic[TurnRel].GetTiles.Count;
        ReligionDic[TurnRel].Coin += Mathf.RoundToInt(avg * 5);
    }
    public void Charisma()
    {
        List<Tile> t = new();
        List<Tile> t2 = new();
        foreach(Tile tile in tiles)
        {
            if (OccupyCalc(ReligionDic[playerRel], tile) >= 80)
            {
                t.Add(tile);
            }
        }
        for(int i = 0; i<Mathf.Min(10, t.Count); i++)
        {
            Tile tmp = null;
            do
            {
                tmp = t[Random.Range(0, t.Count)];
            } while (t2.Contains(tmp));
            t2.Add(tmp);
            AddTile(ReligionDic[TurnRel], tmp);
        }
    }
    public void BuildWall()
    {
        MiracleTurn[Miracle.BuildWall] = 1;
    }
    public void Prophecy()
    {
        if (ReligionDic[TurnRel].knownRelicTile.Count == MaxRelic)
        {
            ReligionDic[TurnRel].Sunlight += MiraclePrice[8];
            return;
        }
        int c = 0;
        foreach(Religions religion in religionsOrder)
        {
            c += ReligionDic[religion].relics.Count;
        }
        if (c == MaxRelic)
        {
            GameManager.Inst.ShowLogBox("이미 모든 성유물이 인수되었습니다.");
            return;
        }
        else
        {
            foreach(Tile tile in tiles)
            {
                if (tile.relic != HolyRelic.none && !ReligionDic[TurnRel].knownRelicTile.Contains(tile))
                {
                    setTile = tile;
                    SurveyRelic(ReligionDic[TurnRel]);
                    setTile.SetFog(ReligionDic[TurnRel], false);
                    setTile = null;
                    return;
                }
            }
        }
    }
    public void Judgement()
    {
        MiracleTurn[Miracle.Judgement] = 3;
    }
    public void ForesightDream()
    {
        ActableCount *=2;
    }
    public void FrozenHeart()
    {
        MiracleTurn[Miracle.FrozenHeart] = 1;
    }
    public void TurnEnd()
    {
        if (bestRel != null)
        {
            bestRel.Ideal -= 15;
            bestRel = null;
        }

        foreach (Tile tile in tiles)
        {
            tile.parentTile = null;
            tile.enlighten = false;
        }

        foreach(Religion religion in religions)
        {
            if (religion.Tech[0, 1])
            {
                religion.Logic += religion.GetTiles.Count;
                religion.Logic -= religion.TechValue[0, 1];
                religion.TechValue[0, 1] = religion.GetTiles.Count;
            }
            if (religion.Tech[0, 4])
            {
                Tile selTile = null;
                List<Tile> closeList = new();

                foreach (Tile relTile in religion.GetTiles)
                {
                    selTile = relTile;
                    if (closeList.Contains(relTile))
                        continue;
                    closeList.Add(relTile);
                    int groupSize = 1;
                    while (selTile != null)
                    {
                        for (int x = Mathf.Max(0, selTile.Pos.x - 1); x <= Mathf.Min(selTile.Pos.x + 1, GameManager.Inst.tiles.width - 1); x++)
                        {
                            for (int y = Mathf.Max(0, selTile.Pos.y - 1); y <= Mathf.Min(selTile.Pos.y + 1, GameManager.Inst.tiles.height - 1); y++)
                            {
                                if (Mathf.Abs(selTile.Pos.y - y) + Mathf.Abs(selTile.Pos.x - x) == 1 && tiles[y, x].mainReligion == religion && !closeList.Contains(tiles[y, x]))
                                {
                                    tiles[y, x].parentTile = selTile;
                                    selTile = tiles[y, x];
                                    closeList.Add(tiles[y, x]);
                                    groupSize++;
                                    goto breakPoint;
                                }
                            }
                        }
                        if(selTile.parentTile != null)
                        {
                            selTile = selTile.parentTile;
                        }
                        else
                        {
                            selTile = null;
                        }
                    breakPoint:
                        continue;
                    }
                    if(groupSize >= 10)
                    {
                        foreach(Tile eTile in closeList)
                        {
                            eTile.enlighten = true;
                        }
                    }
                }
            }
            if (religion.Tech[5, 3] && bestRel == null)
            {
                int i = religion.GetTiles.Count;
                foreach (Religion r in religions)
                {
                    if (r == religion)
                        continue;
                    i = Mathf.Max(i, r.GetTiles.Count);
                }
                if(i == religion.GetTiles.Count)
                {
                    bestRel = religion;
                    religion.Ideal += 15;
                }
            }
        }

        foreach (Tile tile in tiles)
        {
            tile.Product();
            tile.SetCondition(tile.nextCondition);
            tile.NextCondition();
            tile.RisingTurn--;
            tile.FrozenTurn--;
        }
        RefreshTiles();
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
    public void ReturnTurn(Religions rel)
    {
        foreach(Tile tile in tiles)
        {
            if(tile.servRel.Contains(ReligionDic[rel]))
            {
                foreach(Religion r in religions)
                {
                    if (r.religion == rel)
                        continue;
                    tile.servRel.Remove(r);
                    tile.dysentery[r.religion] -= 25;
                }
            }
        }
        foreach(KeyValuePair<Miracle, int> _turn in MiracleTurn)
        {
            if (usingMiracle[_turn.Key] == rel)
            {
                MiracleTurn[_turn.Key]--;
                return;
            }
        }
        if (ReligionDic[rel].TechValue[5, 5] > 0)
        {
            ReligionDic[rel].Ideal -= ReligionDic[rel].TechValue[5, 5] / 100;
            int turn = ReligionDic[rel].TechValue[5, 5] % 100 - 1;
            if(turn > 0)
            {
                ReligionDic[rel].TechValue[5, 5] = ReligionDic[rel].Ideal * 100 + turn;
            }
        }
    }
    public void AutoOccupy(Religion rel)
    {
        foreach(Tile tile in tiles)
        {
            if(OccupyCalc(rel, tile) > rel.AutoOccupy - tile.autoOcc[rel])
            {
                int i = OccupyCalc(rel, tile);
                foreach(Religions religion in tile.met)
                {
                    if(religion != playerRel && OccupyCalc(ReligionDic[religion], tile) > i)
                    {
                        goto next;
                    }
                }
                AddTile(rel, tile);
            }
        next:
            continue;
        }
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
                    if (religion.Tech[k, i])
                        religion.Dysentery++;
                }
            }
        }
        for (int i = 0; i < 6; i++)
        {
            if (religion.Tech[tech.y, i])
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
        religion.Tech[tech.y, tech.x] = true;
        religion.RecentTech = new(tech.x, tech.y);
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
}