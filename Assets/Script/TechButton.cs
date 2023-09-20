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
        TechUI.transform.Find("TechPoint").GetComponent<TextMeshProUGUI>().text = $"��ũ ����Ʈ : {GameManager.Inst.tiles.ReligionDic[GameManager.Inst.tiles.playerRel].TechPoint}";
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        string[,] techDesc = { { "��Ģ:������ �ּҰ��� 10�� �˴ϴ�.", "��ö:������ Ÿ���� ���� ���� ���� 1 ����մϴ�.", "ö��:��밡 ���� ���￡ ���Ͽ� ���� 25% ����մϴ�.", "��ȭ:����Ÿ�Ϸκ��� ȹ���ϴ� ������ ���� Ȯ���� 25% ����մϴ�."
                                ,"���:10�� �̻��� �پ��ִ� Ÿ�Ͽ��� ȹ���ϴ� �޻� 50% ����մϴ�.", "��Ȯ:�⵿���� 1 ����մϴ�."},
                                { "����:Ÿ������ �������� Ÿ�Ͽ��� ������ 10% �����մϴ�.", "����:����Ÿ�Ͽ� ���� ������� ������ ���� ������ ���� ���� 20% ����մϴ�.", "����:Ÿ������ �����ϴ� Ÿ�� ���ɽ� �α� 5Ÿ���� ȹ�� �޻��� 1�ϰ� 10% ����մϴ�.", "�ΰ�:��� ȹ�� ������ �ش� Ÿ�Ϻ��� ������ �ش� Ÿ�Ͽ����� �̻� 7% ����մϴ�."
                                ,"����:Ÿ�Ͽ� ȣ�� �߻��� �α� ���� Ÿ�Ͽ� ȣ�� �߻�Ȯ���� 5% ����մϴ�.", "����:������ �α� Ÿ���� ���� �� ��η� ȹ���� �� �ִ� ������� 20% �����մϴ�."},
                                { "��ǰ:���� ������ Ÿ�Ϸκ��� ȹ���ϴ� ������ 30% ���������� �̻��� 30����մϴ�.", "����:������ Ÿ�Ͽ� ���� �߻��� ȹ���ϴ� �޻��� 80% Ȯ���� �پ��ϴ�.", "ȣ��:������ Ÿ�Ͽ��� Ÿ ������ ���� �� ��η� ȹ���� �� �ִ� ������� 10% �����մϴ�.", "�ݷ�:Ÿ�Ͽ� ���� �߻��� ������ 1 ����մϴ�.",
                                "��ȣ:���簡 �߻��� ������ Ÿ�Ͽ� ��η� ���� �� �ִ� ������� 60% �����մϴ�." ,"��:�����α� Ÿ���� ������ŭ ���� Ȯ���� 2% ����մϴ�."},
                                { "�ΰ�:������ Ÿ�Ͽ��� Ÿ ������ ������ 15% ����մϴ�.", "��:������ �õ��� �� ���� 15% ����մϴ�.", "����:����߻� Ȯ���� 5% �����մϴ�.", "�ݹ�:Ÿ������ �������� Ÿ�Ͽ��� ����߻��� �ش� ������ ���� Ÿ���� ������ 3�����մϴ�.",
                                "�豳:������ �Ұų� �ٲ� Ÿ�Ͽ� ���� Ÿ ������ ������ ���������� 5 ����մϴ�." ,"����:Ÿ�� ���ɽ� �����ϱ��� Ÿ ������ ���� ������ 25 ����մϴ�."},
                                { "����:�ڵ� ������ ������ 5% �����մϴ�.", "��ȭ:������ Ÿ���� ���� �߻� �� ���� ���� ȣ�� �߻�Ȯ���� 20% �����մϴ�.", "�ް�:����Ȯ���� 20%�̸��� Ÿ�� ���ɽ� �α� Ÿ ������ Ÿ���� ������ 10 �϶��մϴ�.", "�쿬:������ ���縦 ���� �߽߰� �ش� �Ͽ� ���� �μ� �ݾ��� 30% �϶��մϴ�.",
                                "��ȸ:���õ� Ÿ���� ������� ���� ���� ��� �ش� Ÿ�Ͽ� ���� �̻��� 10 ����մϴ�." ,"�Ĺ�:Ÿ�� ���ɽ� �ش� �Ͽ� ���� �α� 3Ÿ���� �ڵ� ���� ������ 5% �����մϴ�."},
                                { "���:������ ������ 80�̻��� ��� ȣ�簡 �߻��մϴ�.", "�Ƚ�:������ Ÿ���� ȣ�� �߻��� ������ 1����մϴ�.", "����:Ÿ ������ ���� ���ɽõ��� ������ ��� ���� �α� 5Ÿ�� ���� ����ִ� ������ Ÿ���� �����մϴ�.", "����:���� ���� Ÿ�� ������ �̻��� 15����մϴ�.",
                                "���:������ Ÿ�Ͽ� ���� ���� Ȥ�� ��� Ȥ�� ������ ȿ���� 30% �����մϴ�." ,"�̷�:������ ȹ��� 10�ϰ� �̻��� 20% ����մϴ�."}};

        int i = transform.parent.GetSiblingIndex();
        int j = transform.GetSiblingIndex();
        GameManager.Inst.ShowTooltip($"{techDesc[i, j]}");
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        GameManager.Inst.ShowTooltip("");
    }
}
