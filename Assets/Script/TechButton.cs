using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class TechButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void TechSelect()
    {
        if (GameManager.Inst.tiles.ReligionDic[GameManager.Inst.tiles.playerRel].TechPoint <= 0)
            return;
        int i = transform.parent.GetSiblingIndex();
        int j = transform.GetSiblingIndex();

        GameManager.Inst.tiles.GetTech(GameManager.Inst.tiles.ReligionDic[GameManager.Inst.tiles.playerRel], new(i, j));
        GetComponent<Image>().color = Color.black;
        GetComponent<Button>().interactable = false;

        GameManager.Inst.tiles.ReligionDic[GameManager.Inst.tiles.playerRel].TechPoint--;
        GameObject TechUI = GameManager.Inst.tiles.TechUI;
        TechUI.transform.Find("TechPoint").GetComponent<TextMeshProUGUI>().text = $"테크 포인트 : {GameManager.Inst.tiles.ReligionDic[GameManager.Inst.tiles.playerRel].TechPoint}";
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        string[,] techDesc = { { "원칙:안정의 최소값이 10이 됩니다.", "냉철:점령한 타일의 수에 따라 논리가 1 상승합니다.", "철저:상대가 벌인 논쟁에 한하여 논리가 25% 상승합니다.", "강화:점령타일로부터 획득하는 코인이 낮은 확률로 25% 상승합니다."
                                ,"계몽:10개 이상의 붙어있는 타일에서 획득하는 햇살 50% 상승합니다.", "정확:기동력이 1 상승합니다."},
                                { "존중:타종교가 점령중인 타일에서 이질이 10% 감소합니다.", "이해:점령타일에 대한 영향력이 나보다 높은 종교를 상대로 논리가 20% 상승합니다.", "개혁:타종교가 점령하던 타일 점령시 인근 5타일의 획득 햇살이 1턴간 10% 상승합니다.", "부강:평균 획득 코인이 해당 타일보다 높을시 해당 타일에서의 이상 7% 상승합니다."
                                ,"영향:타일에 호재 발생시 인근 점령 타일에 호재 발생확률이 5% 상승합니다.", "유행:점령지 인근 타일의 전도 및 기부로 획득할 수 있는 영향력이 20% 증가합니다."},
                                { "베품:매턴 점령한 타일로부터 획득하는 코인이 30% 감소하지만 이상이 30상승합니다.", "의지:점령한 타일에 악재 발생시 획득하는 햇살이 80% 확률로 줄어듭니다.", "호감:점령한 타일에서 타 종교의 전도 및 기부로 획득할 수 있는 영향력이 10% 감소합니다.", "격려:타일에 악재 발생시 안정이 1 상승합니다.",
                                "구호:악재가 발생한 미점령 타일에 기부로 얻을 수 있는 영향력이 60% 증가합니다." ,"명성:점령인근 타일의 개수만큼 점령 확률이 2% 상승합니다."},
                                { "민감:점령한 타일에서 타 종교의 이질이 15% 상승합니다.", "명석:논쟁을 시도할 시 논리가 15% 상승합니다.", "예방:악재발생 확률이 5% 감소합니다.", "반발:타종교가 점령중인 타일에서 악재발생시 해당 종교에 대한 타일의 안정이 3감소합니다.",
                                "배교:종교를 잃거나 바뀐 타일에 대해 타 종교의 이질이 영구적으로 5 상승합니다." ,"단절:타일 점령시 다음턴까지 타 종교에 대한 이질이 25 상승합니다."},
                                { "혁신:자동 점령의 조건이 5% 감소합니다.", "변화:점령한 타일의 악재 발생 후 다음 턴의 호재 발생확률이 20% 증가합니다.", "급격:점령확률이 20%미만인 타일 점령시 인근 타 종교의 타일의 안정이 10 하락합니다.", "우연:성유물 조사를 통해 발견시 해당 턴에 한해 인수 금액이 30% 하락합니다.",
                                "선회:선택된 타일의 영향력이 가장 낮은 경우 해당 타일에 대한 이상이 10 상승합니다." ,"파문:타일 점령시 해당 턴에 한해 인근 3타일의 자동 점령 조건이 5% 감소합니다."},
                                { "고고:성지의 안정이 80이상일 경우 호재가 발생합니다.", "안심:점령한 타일의 호재 발생시 안정이 1상승합니다.", "굳건:타 종교의 성지 점령시도가 실패할 경우 성지 인근 5타일 내의 비어있는 무작위 타일을 점령합니다.", "여유:가장 많은 타일 보유시 이상이 15상승합니다.",
                                "우대:점령한 타일에 대한 전도 혹은 기부 혹은 논쟁의 효과가 30% 증가합니다." ,"미려:성유물 획득시 10턴간 이상이 20% 상승합니다."}};

        int i = transform.parent.GetSiblingIndex();
        int j = transform.GetSiblingIndex();
        GameManager.Inst.ShowTooltip($"{techDesc[i, j]}");
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        GameManager.Inst.ShowTooltip("");
    }
}
