using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillUI : MonoBehaviour
{
    [SerializeField] DungeonManager dm;
    [SerializeField] DungeonUIManager dunUI;

    [SerializeField] Image skillIcon;
    [SerializeField] Image skillTypeIcon;
    [SerializeField] TMP_Text skillTitle;
    [SerializeField] TMP_Text skillDescription;
    [SerializeField] TMP_Text skillEffect;

    [SerializeField] Image[] skillSlotIcons = new Image[5];
    [SerializeField] TMP_Text[] skillSlotNames = new TMP_Text[5];
    [SerializeField] GameObject[] upButtons = new GameObject[5];
    [SerializeField] GameObject[] downButtons = new GameObject[5];

    public void Init()
    {

    }

    public void Refresh()
    {
        for(int i=0; i<5; i++)
        {
            if(dm.Player.UnitData.Skills[i] == null)
            {
                skillSlotIcons[i].gameObject.SetActive(false);
                skillSlotNames[i].gameObject.SetActive(false);
                upButtons[i].SetActive(false);
                downButtons[i].SetActive(false);
            }
            else
            {
                SkillData skill = dm.Player.UnitData.Skills[i];
                skillSlotIcons[i].gameObject.SetActive(true);
                skillSlotIcons[i].sprite = skill.Sprite;
                skillSlotNames[i].gameObject.SetActive(true);
                skillSlotNames[i].text = dunUI.GetSkillName(skill.Key);
                skillSlotNames[i].gameObject.SetActive(true);
                switch (i)
                {
                    case 0:
                        downButtons[i].SetActive(true);
                        break;
                    case 1:
                        upButtons[i].SetActive(true);
                        downButtons[i].SetActive(true);
                        break;
                    case 2:
                        upButtons[i].SetActive(true);
                        downButtons[i].SetActive(true);
                        break;
                    case 3:
                        if (dm.Player.UnitData.Skills[i].Type == SkillType.Attack)
                        {
                            upButtons[i].SetActive(true);
                        }
                        else if (dm.Player.UnitData.Skills[i].Type == SkillType.Status)
                        {
                            downButtons[i].SetActive(true);
                        }
                        break;
                    case 4:
                        upButtons[i].SetActive(true);
                        break;
                }
            }
        }
    }

    public void Btn_SkillSlotClicked(int index)
    {
        if (dm.Player.UnitData.Skills[index] != null)
        {
            SkillData skill = dm.Player.UnitData.Skills[index];
            skillIcon.gameObject.SetActive(true);
            skillIcon.sprite = skill.Sprite;
            skillTitle.text = dunUI.GetSkillName(skill.Key);
            skillDescription.text = dunUI.GetSkillDescription(skill.Key);
            skillEffect.text = dunUI.GetSkillEffect(skill.Key, skill.EffectValues);
        }
    }

    public void Btn_UpButtonClicked(int index)
    {
        dm.Player.UnitData.SwapSkillSlots(index - 1, index);
        Refresh();
    }
    public void Btn_DownButtonClicked(int index)
    {
        dm.Player.UnitData.SwapSkillSlots(index, index + 1);
        Refresh();
    }
}
