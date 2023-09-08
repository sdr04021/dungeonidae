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

    [SerializeField] GameObject skillInfoBg;
    [SerializeField] SkillInfo skillInfo;

    [SerializeField] RectTransform content;
    [SerializeField] SkillSlot skillSlotPrefab;
    List<SkillSlot> skillSlots = new();

    public void Init()
    {

    }

    public void Refresh()
    {
        List<string> skills = dm.Player.UnitData.acquiredSkills;

        int count = Mathf.Max(skills.Count, skillSlots.Count);
        for(int i=0; i<count; i++)
        {
            if (i >= skillSlots.Count)
            {
                SkillSlot slot = Instantiate(skillSlotPrefab, content);
                int temp = i;
                slot.icon.sprite = GameManager.Instance.GetSkillBase(skills[i]).Sprite;
                slot.GetComponent<Button>().onClick.AddListener(() => Btn_SkillSlotClicked(temp));
                skillSlots.Add(slot);
            }
            else if (i >= skills.Count)
            {
                skillSlots[i].gameObject.SetActive(false);
            }
            else
            {
                skillSlots[i].gameObject.SetActive(true);
                skillSlots[i].icon.sprite = GameManager.Instance.GetSkillBase(skills[i]).Sprite;
            }
        }
        skillInfoBg.SetActive(false);
    }

    public void Btn_SkillSlotClicked(int index)
    {
        List<string> skills = dm.Player.UnitData.acquiredSkills;
        if ((index < skills.Count) && (skills[index] != null))
        {
            skillInfoBg.SetActive(true);
            skillInfo.SetSkill(GameManager.Instance.GetSkillBase(dm.Player.UnitData.acquiredSkills[index]));
        }
    }
}
