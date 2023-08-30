using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilitySlot : MonoBehaviour
{
    AbilityUI abilityUI;
    [SerializeField] Image icon;
    [SerializeField] Image[] levelIndicator = new Image[3];
    public int ListIndex { get; private set; } = -1;

    public void Init(AbilityUI abilityUI)
    {
        this.abilityUI = abilityUI;
    }

    public void SetAbility(AbilityData ability, int listIndex)
    {
        icon.sprite = ability.BaseData.Sprite;
        ListIndex = listIndex;
        ApplyAbilityLevel(ability);
    }

    public void ApplyAbilityLevel(AbilityData ability)
    {
        for (int i = 0; i < levelIndicator.Length; i++)
        {
            if (i < ability.Level)
                levelIndicator[i].gameObject.SetActive(true);
            else
                levelIndicator[i].gameObject.SetActive(false);
        }
    }

    public void Btn_AbilitySlotClicked()
    {
        abilityUI.AbilitySlotClicked(ListIndex);
    }
}
