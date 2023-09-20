using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TMPro.TMP_FontAsset mainFont;
    public GameObject LogBox;
    public GameObject Logs;
    public TileInfoTab infoTab;
    public Tooltip tooltip;
    public Tiles tiles;
    public DataManager dataManager;
    public float gameSpeed;
    private static GameManager instance;
    public static GameManager Inst
    {
        get
        {
            if(!instance)
            {
                instance = FindObjectOfType<GameManager>();
            }
            return instance;
        }
    }
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        Application.targetFrameRate = 120;
        gameSpeed = 1f;
    }
    public void ShowTooltip(string text)
    {
        tooltip.gameObject.SetActive(true);
        tooltip.rect.sizeDelta = Vector2.zero;
        tooltip._Text = text;
    }
    public void ShowLogBox(string text)
    {
        GameObject obj = Instantiate(LogBox, Logs.transform);
        obj.GetComponent<LogItem>().LogText(text);
    }
}
