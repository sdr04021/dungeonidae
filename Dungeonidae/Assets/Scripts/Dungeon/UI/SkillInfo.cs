using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillInfo : MonoBehaviour
{
    [SerializeField] TMP_Text tmpTitle;
    [SerializeField] Image icon;
    [SerializeField] TMP_Text tmpCost;
    [SerializeField] TMP_Text tmpDescription;

    public void SetSkill(SkillBase skill)
    {
        tmpTitle.text = GameManager.Instance.GetSkillName(skill.Key);
        icon.sprite = skill.Sprite;
        tmpCost.text = skill.Cost.ToString() + " MP";
        tmpDescription.text = GameManager.Instance.GetSkillDescription(skill.Key, skill.GetListForDescription());
    }
}
