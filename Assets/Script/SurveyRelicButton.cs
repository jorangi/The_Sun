using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurveyRelicButton : TileMenuBtn
{
    private void OnEnable()
    {
        btn.interactable = !tiles.SettedTile.relic.CheckSurveidByRel(Tiles.player);
    }
}
