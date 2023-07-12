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
    [SerializeField] TMP_Text skillCost;
    [SerializeField] TMP_Text skillDescription;
    [SerializeField] TMP_Text skillEffect;

    [SerializeField] RectTransform content;
    [SerializeField] Image skillSlotPrefab;
    List<Image> skillSlots = new();

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
                Image slot = Instantiate(skillSlotPrefab, content);
                int temp = i;
                slot.sprite = GameManager.Instance.GetSkillBase(skills[i]).Sprite;
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
                skillSlots[i].sprite = GameManager.Instance.GetSkillBase(skills[i]).Sprite;
            }
        }
    }

    public void Btn_SkillSlotClicked(int index)
    {
        List<string> skills = dm.Player.UnitData.acquiredSkills;
        if ((index < skills.Count) && (skills[index] != null))
        {
            SkillBase skillBase = GameManager.Instance.GetSkillBase(dm.Player.UnitData.acquiredSkills[index]);
            skillIcon.gameObject.SetActive(true);
            skillIcon.sprite = skillBase.Sprite;
            skillTitle.text = dunUI.GetSkillName(skillBase.Key);
            skillCost.text = skillBase.Cost.ToString() + " MP";
            skillDescription.text = dunUI.GetSkillDescription(skillBase.Key, skillBase.GetListForDescription());
            //skillEffect.text = dunUI.GetSkillEffect(skillBase.Key, skillBase.EffectValues);
        }
    }
}
