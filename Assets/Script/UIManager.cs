using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#region UIElementsBase
public class UI
{
    protected TextMeshProUGUI Text;
    protected Image Image;
    public UI(TextMeshProUGUI text = null, Image image = null)
    {
        Text = text;
        Image = image;
    }
    public void ChangeUI<T>(T arg)
    {
        if(arg is Sprite) Image.sprite = arg as Sprite;
        else Text.text = arg.ToString();
    }
}
public class ValueUI : UI
{
    public ValueUI(TextMeshProUGUI text = null, Image image = null):base(text, image){}
    public void ChangeUI(string arg)
    {
        Text.text = string.Format("0,000", arg);
    }
}
#endregion
public class TileInfoUI
{
    private GameObject tileInfoUI;
    private UI PositionUI;
    private UI Context;
    public TileInfoUI(GameObject tileInfoUI)
    {
        this.tileInfoUI = tileInfoUI;
        PositionUI = new(tileInfoUI.transform.Find("position").GetComponent<TextMeshProUGUI>());
        Context = new(tileInfoUI.transform.Find("context").GetComponent<TextMeshProUGUI>());
    }
    public void SetPosition(Vector2Int pos)
    {
        PositionUI.ChangeUI($"({pos.x}, {pos.y})");
    }
    public void SetContext(TileData tileData)
    {
        string inf = "";
        string sta = "";
        foreach(KeyValuePair<ReligionType, ReligionDataInTile> d in tileData.ReligionsDataInTile)
        {
            inf += $"({d.Key} : {d.Value.Influence}) ";
            sta += $"({d.Key} : {d.Value.Stability}) ";
        }
        Context.ChangeUI($"[영향력]<br>{inf}<br><br>[안정]<br>{sta}");
    }
}
public class UIManager : MonoBehaviour
{
    public Religion player;

    #region UIElements
    [SerializeField]
    private TextMeshProUGUI coinText, sunlightText, turnText, turnActText;
    [SerializeField]
    private Image turnedReligion;
    [SerializeField]
    private GameObject tileInfoUI;
    #endregion
    #region UIObject
    public ValueUI coinUI, sunlightUI, turnUI, turnActUI, logicUI, idealUI;
    public UI turnedReligionUI;
    public TileInfoUI TileInfo;
    #endregion
    public void OnEnable()
    {
        coinUI = new ValueUI(coinText);
        sunlightUI = new ValueUI(sunlightText);
        turnUI = new ValueUI(coinText);
        turnActUI = new ValueUI(coinText);
        turnedReligionUI = new ValueUI(null, turnedReligion);
        TileInfo = new TileInfoUI(tileInfoUI);
    }
    public void ShowTileInfo(TileData tileData)
    {
        TileInfo.SetPosition(tileData.POS);
        TileInfo.SetContext(tileData);
        tileInfoUI.SetActive(true);
    }
    public void Evangelize()
    {

    }
    public void ShowCharityMenu()
    {

    }
}