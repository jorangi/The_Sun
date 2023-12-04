using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyRelicButton : TileMenuBtn
{
    private void OnEnable()
    {
        btn.interactable = tiles.SettedTile.relic.CheckSurveidByRel(Tiles.player) && tiles.SettedTile.relic.relic != HolyRelic.none;
    }
}
