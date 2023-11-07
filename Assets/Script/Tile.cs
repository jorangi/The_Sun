using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tile:MonoBehaviour
{
    //컴포넌트 관련 데이터
    public Vector2Int Pos;
    public SpriteRenderer outerSquare, innerSquare, Symbol, HolyRelicSymbol, Sacred, HideBox;
    public TextMeshPro coinText, sunlightText;

    //테크 관련 데이터
    public bool enlighten;
    public Tile parentTile;
    public List<Religion> servRel = new();
    public Dictionary<Religion, int> autoOcc = new();

    //종교 관련 데이터
    public List<ReligionType> History = new();
    public Religion mainReligion;
    public Dictionary<ReligionType, int> influence = new();
    public Dictionary<ReligionType, int> stability = new();
    public Dictionary<ReligionType, int> dysentery = new();
    public List<ReligionType> met = new();
    public int RisingTurn = 0;
    public ReligionType Frozen = ReligionType.none;
    public int FrozenTurn = 0;

    //생산 관련 데이터
    public TileCondition condition;
    public TileCondition nextCondition;
    public List<float[]> CoinBonus = new();
    public List<float[]> SunlightBonus = new();
    public int TotalMadeCoin;
    public int MakeCoin;
    public int ProCoin;
    public int MakeSunlight;
    public int ProSunlight;
    public bool Judgement;
    public int goodPer, badPer;


    //기부 관련 데이터
    public int TotalDonation;

    //성유물 관련 데이터
    public HolyRelic relic;
    public Dictionary<Religion, bool> serveidRel = new();
    public float findTechBonus = 1.0f;

    //안개 관련
    public Dictionary<ReligionType, List<bool>> fogBool = new();

    private void Awake()
    {
        MakeCoin = 1;
        MakeSunlight = 1;

        switch(Random.Range(0, 10))
        {
            case 0:
                SetCondition(TileCondition.Good);
                break;
            case 1:
                SetCondition(TileCondition.Bad);
                break;
            default:
                SetCondition(TileCondition.none);
                break;
        }
        switch(Random.Range(0, 10))
        {
            case 0:
                innerSquare.color = GameManager.Inst.tiles.disableTile;
                break;
            case 1:
                innerSquare.color = GameManager.Inst.tiles.blockedTile;
                break;
            default:
                innerSquare.color = GameManager.Inst.tiles.enableTile;
                break;
        }
        Refresh();
    }
    //public void Product()
    //{
    //    goodPer = 10;
    //    badPer = 10;

    //    float badPenalty = 1.0f;

    //    if(condition == TileCondition.Bad)
    //        mainReligion.Coin -= Mathf.RoundToInt(GameManager.Inst.tiles.baseMaintance * badPenalty);
    //    else
    //        mainReligion.Coin -= Mathf.RoundToInt(GameManager.Inst.tiles.baseMaintance);

    //    mainReligion.Coin += ProCoin;
    //    mainReligion.Sunlight += ProSunlight;

    //    TotalMadeCoin += Mathf.RoundToInt(ProCoin);

    //    if(mainReligion.religionType != ReligionType.none)
    //        for (int i = Mathf.Max(0, Pos.y - 1); i <= Mathf.Min(GameManager.Inst.tiles.height - 1, Pos.y + 1); i++)
    //    {
    //        for (int j = Mathf.Max(0, Pos.x - 1); j <= Mathf.Min(GameManager.Inst.tiles.width - 1, Pos.x + 1); j++)
    //        {
    //            if (Mathf.Abs(Pos.y - i) + Mathf.Abs(Pos.x - j) == 1 && Random.Range(0f, 1f) <= 0.4f)
    //            {
    //                GameManager.Inst.tiles.tiles[i, j].AddInfluence(mainReligion, 1);
    //            }
    //        }
    //    }


    //    foreach (ReligionType r in GameManager.Inst.tiles.religionsOrder)
    //    {
    //        autoOcc[GameManager.Inst.tiles.ReligionDic[r]] = 0;

    //        if (!stability.ContainsKey(r))
    //            continue;

    //        Religion rel = GameManager.Inst.tiles.ReligionDic[r];

    //        float relDyBonus = 1.0f;

    //        if (rel.Tech[1, 0] && mainReligion.religionType != ReligionType.none && mainReligion.religionType != rel.religionType)
    //        {
    //            relDyBonus -= 0.1f;
    //        }

    //        if (mainReligion.Tech[3, 0] && rel != mainReligion)
    //        {
    //            relDyBonus += 0.15f;
    //        }

    //        if (stability[r] > 0 && !Sacred.gameObject.activeSelf)
    //        {
    //            AddStability(rel, -1);
    //            if (condition == TileCondition.Bad)
    //                AddStability(rel, -3);

    //            if (Random.Range(0, 101) <= 100 - stability[r])
    //            {
    //                int distanceDys = Mathf.RoundToInt(Vector2Int.Distance(rel.SacredPlace.Pos, Pos) / Mathf.Max(GameManager.Inst.tiles.width, GameManager.Inst.tiles.height) * GameManager.Inst.tiles.baseDysentery);

    //                AddInfluence(rel, -Mathf.RoundToInt((rel.Dysentery + distanceDys + dysentery[rel.religionType]) * relDyBonus));
    //            }
    //        }
    //        else if (stability[r] > 0 && Sacred.gameObject.activeSelf)
    //        {
    //            float rand = Random.Range(0f, 1f);
    //            if (rand <= 0.4f)
    //            {
    //                AddStability(rel, -1);
    //                if (condition == TileCondition.Bad)
    //                    AddStability(rel, -1);
    //            }

    //            if (Random.Range(0, 101) <= 100 - stability[r])
    //            {
    //                int distanceDys = Mathf.RoundToInt(Vector2Int.Distance(rel.SacredPlace.Pos, Pos) / Mathf.Max(GameManager.Inst.tiles.width, GameManager.Inst.tiles.height) * GameManager.Inst.tiles.baseDysentery);

    //                AddInfluence(rel, -Mathf.RoundToInt((rel.Dysentery + distanceDys + dysentery[rel.religionType]) * relDyBonus));
    //            }
    //        }
    //    }

    //    for(int i = CoinBonus.Count - 1; i > 0; i--)
    //    {
    //        float[] t = CoinBonus[i];
    //        t[1]--;
    //        if (Mathf.RoundToInt(t[1]) == 0)
    //        {
    //            CoinBonus.RemoveAt(i);
    //        }
    //    }
    //    for(int i = SunlightBonus.Count - 1; i > 0; i--)
    //    {
    //        float[] t = SunlightBonus[i];
    //        t[1]--;
    //        if (Mathf.RoundToInt(t[1]) == 0)
    //        {
    //            SunlightBonus.RemoveAt(i);
    //        }
    //    }

    //    if (mainReligion.Tech[2, 0])
    //    {
    //        CoinBonus.Add(new float[] { 0.7f, 1.0f });
    //    }

    //    if (mainReligion.Tech[3,2])
    //    {
    //        badPer -= 5;
    //    }

    //    if (condition == TileCondition.Bad && mainReligion.Tech[4, 1])
    //    {
    //        goodPer += 20;
    //    }

    //    findTechBonus = 1.0f;

    //    if (mainReligion.Tech[5, 0] && mainReligion.SacredPlace == this && stability[mainReligion.religionType] >= 80)
    //    {
    //        nextCondition = TileCondition.Good;
    //    }

    //    History.Add(mainReligion.religionType);
    //}
    public void Refresh()
    {
        int[] prod = CalcProduct();
        ProCoin = prod[0];
        ProSunlight = prod[1];
        coinText.text = $"{ProCoin}";
        sunlightText.text = $"{ProSunlight}";
        switch (condition)
        {
            default:
            case TileCondition.none:
                break;
            case TileCondition.Good:
                outerSquare.color = GameManager.Inst.tiles.goodTile;
                if (mainReligion.Tech[1, 4])
                {
                    for (int i = Mathf.Max(0, Pos.y - 1); i <= Mathf.Min(GameManager.Inst.tiles.height - 1, Pos.y + 1); i++)
                    {
                        for (int j = Mathf.Max(0, Pos.x - 1); j <= Mathf.Min(GameManager.Inst.tiles.width - 1, Pos.x + 1); j++)
                        {
                            if (Mathf.Abs(Pos.y - i) + Mathf.Abs(Pos.x - j) == 1 && GameManager.Inst.tiles.tiles[i, j].mainReligion == mainReligion)
                            {
                                GameManager.Inst.tiles.tiles[i, j].goodPer += 5;
                            }
                        }
                    }
                }
                if (mainReligion.Tech[5, 1])
                {
                    AddStability(mainReligion, 1);
                }
                break;
            case TileCondition.Bad:
                outerSquare.color = GameManager.Inst.tiles.badTile;
                if (mainReligion.Tech[2,3])
                {
                    AddStability(mainReligion, 1);
                }
                foreach(Religion religion in GameManager.Inst.tiles.religions)
                {
                    if(religion != mainReligion && religion.Tech[3, 3])
                    {
                        AddStability(mainReligion, -3);
                    }
                }
                break;
        }
    }
    public int[] CalcProduct()
    {
        switch (condition)
        {
            case TileCondition.Good:
                MakeCoin = 2;
                MakeSunlight = 2;
                break;
            case TileCondition.Bad:
                MakeCoin = 1;
                MakeSunlight = 0;
                if (mainReligion.Tech[2,1] && Random.Range(0f, 1f) > 0.8f)
                {
                    MakeSunlight = 1;
                }
                break;
            case TileCondition.none:
                MakeCoin = 1;
                MakeSunlight = 1;
                break;
        }

        if (mainReligion.relics.Contains(HolyRelic.GoldenEye))
        {
            CoinBonus.Add(new float[] { 1.3f, 1.0f });
        }
        if (mainReligion.relics.Contains(HolyRelic.HolyGrail) && condition == TileCondition.Good)
        {
            CoinBonus.Add(new float[] { 1.3f, 1.0f });
        }
        if (mainReligion.relics.Contains(HolyRelic.SaintStaff) && Random.Range(0f, 1f) <= 0.3f)
        {
            mainReligion.Sunlight++;
        }
        if (mainReligion.religionType != ReligionType.none)
        {
            if (mainReligion.Tech[0, 3] && Random.Range(0f, 1f) <= 0.2f)
            {
                CoinBonus.Add(new float[] { 1.25f, 1.0f });
            }
        }
        if (enlighten)
        {
            SunlightBonus.Add(new float[] { 1.5f, 1.0f });
        }
        if (Judgement)
        {
            CoinBonus.Add(new float[] { 0.0f, 1.0f });
        }

        float coinBonus = 1.0f;
        float sunlightBonus = 1.0f;
        foreach (float[] bonus in CoinBonus)
        {
            coinBonus *= bonus[0];
        }
        foreach (float[] bonus in SunlightBonus)
        {
            sunlightBonus *= bonus[0];
        }
        int[] result = new int[2] { Mathf.RoundToInt(MakeCoin * coinBonus), Mathf.RoundToInt(MakeSunlight * sunlightBonus) };
        return new int[2] { result[0], result[1] };
    }
    public void NextCondition()
    {
        int good = 10 + goodPer - Mathf.RoundToInt(badPer/2f);
        int bad = 10 + badPer - Mathf.RoundToInt(goodPer / 2f);

        int rand = Random.Range(0, 101);
        if(rand <= good)
        {
            nextCondition = TileCondition.Good;
        }
        else if(rand <= bad)
        {
            nextCondition = TileCondition.Bad;
        }
        else
        {
            nextCondition = TileCondition.none;
        }

        if (RisingTurn > 0)
            nextCondition = TileCondition.Good;
    }
    public void SetCondition(TileCondition condition)
    {
        this.condition = condition;
        Refresh();
    }
    public void AddInfluence(Religion rel, int val)
    {
        val = Mathf.Min(val, GameManager.Inst.tiles.MaxInfluence);
        influence[rel.religionType] += val;
        if (!met.Contains(rel.religionType))
        {
            AddStability(rel, 100);
            met.Add(rel.religionType);
        }
        else
        {
            AddStability(rel, Random.Range(20, 51));
        }
        influence[rel.religionType] = Mathf.Clamp(influence[rel.religionType], 0, GameManager.Inst.tiles.MaxInfluence);
    }
    public void AddStability(Religion rel, int val)
    {
        if (!stability.ContainsKey(rel.religionType))
        {
            stability.Add(rel.religionType, val);
        }
        else
        {
            stability[rel.religionType] += val;
        }
        if (rel.Tech[0, 0])
            stability[rel.religionType] = Mathf.Clamp(stability[rel.religionType], 10, 100);
        else
            stability[rel.religionType] = Mathf.Clamp(stability[rel.religionType], 0, 100);
    }
    //public void SetReligion(Religion religion)
    //{
    //    foreach (Religion rels in GameManager.Inst.tiles.religions)
    //    {
    //        if (rels.Tech[3, 4])
    //        {
    //            foreach (Religion r in GameManager.Inst.tiles.religions)
    //            {
    //                if (r == rels)
    //                    continue;
    //                dysentery[r.religionType] += 5;
    //            }
    //        }
    //    }

    //    if (mainReligion.religionType != ReligionType.none)
    //    {
    //        if (mainReligion.religionType == GameManager.Inst.tiles.usingMiracle?[Miracle.Judgement])
    //        {
    //            Judgement = true;
    //        }
    //        if (religion.religionType == GameManager.Inst.tiles.usingMiracle?[Miracle.Judgement])
    //        {
    //            Judgement = false;
    //        }
    //    }

    //    if(mainReligion != null)
    //    {
    //        if(Sacred.gameObject.activeSelf)
    //        {
    //            for(int i = 0; i<History.Count; i++)
    //            {
    //                if (History[i] != History[0])
    //                {
    //                    if (i == GameManager.Inst.tiles.Turn)
    //                    {
    //                        religion.TechPoint++;
    //                    }
    //                    break;
    //                }
    //            }
    //        }
    //        mainReligion?.GetTiles.Remove(this);
    //        if (mainReligion.religionType != ReligionType.none)
    //        {
    //            if(mainReligion.GetTiles.Count == 0)
    //            {
    //                GameManager.Inst.tiles.religions.Remove(mainReligion);
    //                GameManager.Inst.tiles.religionsOrder.Remove(mainReligion.religionType);
    //                foreach (Tile tile in GameManager.Inst.tiles.tiles)
    //                {
    //                    if (tile.mainReligion == mainReligion)
    //                    {
    //                        tile.SetReligion(null);
    //                    }
    //                    tile.influence.Remove(mainReligion.religionType);
    //                    if (tile.stability.ContainsKey(mainReligion.religionType))
    //                    {
    //                        tile.stability.Remove(mainReligion.religionType);
    //                    }
    //                }
    //            }
    //            if (religion.Tech[1,2])
    //            {
    //                for (int i = Mathf.Max(0, Pos.y - 5); i <= Mathf.Min(GameManager.Inst.tiles.height - 1, Pos.y + 5); i++)
    //                {
    //                    for (int j = Mathf.Max(0, Pos.x - 5); j <= Mathf.Min(GameManager.Inst.tiles.width - 1, Pos.x + 5); j++)
    //                    {
    //                        if (Mathf.Abs(Pos.y - i) + Mathf.Abs(Pos.x - j) <= 5)
    //                        {
    //                            SunlightBonus.Add(new float[2] { 1.1f, 1.0f});
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }
    //    if(religion == null)
    //    {
    //        mainReligion = null;
    //        Symbol.sprite = null;
    //        SetFog(religion);
    //        Sacred.gameObject.SetActive(false);
    //        return;
    //    }
    //    mainReligion = religion;
    //    Symbol.sprite = religion.symbol;
    //    mainReligion.GetTiles.Add(this);
    //    SetFog(religion);

    //    if (mainReligion.SacredPlace == this)
    //    {
    //        Sacred.gameObject.SetActive(true);
    //    }

    //    if (religion.Tech[3, 5])
    //    {
    //        servRel.Add(religion);
    //        foreach(Religion rels in GameManager.Inst.tiles.religions)
    //        {
    //            if (rels == religion)
    //                continue;
    //            dysentery[rels.religionType] += 25;
    //        }
    //    }
    //}
    public void SetFog(Religion rel)
    {
        for (int i = Mathf.Max(0, Pos.y - rel.TorchRange); i <= Mathf.Min(GameManager.Inst.tiles.height - 1, Pos.y + rel.TorchRange); i++)
        {
            for (int j = Mathf.Max(0, Pos.x - rel.TorchRange); j <= Mathf.Min(GameManager.Inst.tiles.width - 1, Pos.x + rel.TorchRange); j++)
            {
                if (Mathf.Abs(Pos.y - i) + Mathf.Abs(Pos.x - j) <= rel.TorchRange)
                {
                    GameManager.Inst.tiles.tiles[i, j].SetFog(rel, mainReligion.religionType != rel.religionType);
                }
            }
        }
    }
    public void SetFog(Religion rel, bool fogOn)
    {
        if(!fogBool.ContainsKey(rel.religionType))
        {
            fogBool.Add(rel.religionType, new());
        }
        if (!fogOn)
        {
            fogBool[rel.religionType].Add(fogOn);
            if(rel.religionType == GameManager.Inst.tiles.playerRel)
            {
                HideBox.gameObject.SetActive(false);
            }
            if(!rel.TempTorchedTile.Contains(this))
                rel.TempTorchedTile.Add(this);
        }
        else
        {
            if (fogBool[rel.religionType].Count > 0)
            {
                fogBool[rel.religionType].RemoveAt(0);
            }
            if (fogBool[GameManager.Inst.tiles.playerRel].Count == 0)
            {
                HideBox.gameObject.SetActive(true);
                rel.TempTorchedTile.Remove(this);
            }
        }
    }
}
