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
        else Text.text = string.Format("0,000", arg);
    }
}
#endregion
public class UIManager : MonoBehaviour
{
    public Religion player;

    #region UIElements
    [SerializeField]
    private TextMeshProUGUI coinText, sunlightText, turnText, turnActText;
    [SerializeField]
    private Image turnedReligion;
    #endregion
    #region UIObject
    public UI coinUI, sunlightUI, turnUI, turnActUI, turnedReligionUI, logicUI, idealUI;
    #endregion
    public void OnEnable()
    {
        coinUI = new UI(coinText);
        sunlightUI = new UI(sunlightText);
        turnUI = new UI(coinText);
        turnActUI = new UI(coinText);
        turnedReligionUI = new UI(null, turnedReligion);
    }
}