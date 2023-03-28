using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityUI : MonoBehaviour
{
    [SerializeField] DungeonManager dm;
    [SerializeField] DungeonUIManager dunUI;
    public DungeonUIManager DunUI { get => dunUI; }

    [SerializeField] TMP_Text apText;

    [SerializeField] RectTransform content;
    [SerializeField] AbilitySlot abilitySlotPrefab;
    List<AbilitySlot> abilitySlot = new();

    int currentIndex = -1;
    [SerializeField] Image abilityIcon;
    [SerializeField] GameObject levelUpButton;
    [SerializeField] TMP_Text currentLevel;
    [SerializeField] GameObject maxImage;
    [SerializeField] TMP_Text abilityTitle;
    [SerializeField] TMP_Text abilityDescription;
    [SerializeField] TMP_Text abilityEffect;

    public void Init()
    {
        for(int i=0; i<dm.Player.PlayerData.Abilities.Count; i++)
        {
            AbilitySlot slot = Instantiate(abilitySlotPrefab, content);
            slot.Init(this);
            abilitySlot.Add(slot);
        }
    }

    public void Refresh()
    {
        apText.text = dm.Player.PlayerData.abilityPoint.ToString();
        for (int i = 0; i < dm.Player.PlayerData.Abilities.Count; i++)
        {
            if (i >= abilitySlot.Count)
            {
                AbilitySlot slot = Instantiate(abilitySlotPrefab, content);
                slot.Init(this);
                abilitySlot.Add(slot);
            }
            abilitySlot[i].SetAbility(dm.Player.PlayerData.Abilities[i], i);
        }
    }

    public void AbilitySlotClicked(int index)
    {
        if (index >= 0)
        {
            currentIndex = index;
            abilityIcon.gameObject.SetActive(true);
            AbilityData ability = dm.Player.PlayerData.Abilities[index];
            abilityTitle.text = DunUI.GetAbilityName(ability.Key);
            abilityIcon.sprite = ability.Sprite;
            abilityDescription.text = DunUI.GetAbilityDescription(ability.Key);
            abilityEffect.text = DunUI.GetAbilityEffect(ability.Key, ability.EffectValues);
            SetCurrentLevel(ability);
        }
    }

    public void SetCurrentLevel(AbilityData ability)
    {
        if (ability.Level == 3)
        {
            levelUpButton.SetActive(false);
            maxImage.SetActive(true);
        }
        else
        {
            levelUpButton.SetActive(true);
            maxImage.SetActive(false);
            currentLevel.text = ability.Level.ToString();
        }
    }

    public void Btn_LevelUpClicked()
    {
        if (currentIndex >= 0)
        {
            if (dm.Player.IncreaseAbilityLevel(currentIndex))
            {
                AbilityData ability = dm.Player.PlayerData.Abilities[currentIndex];
                abilitySlot[currentIndex].ApplyAbilityLevel(ability);
                SetCurrentLevel(ability);
                apText.text = dm.Player.PlayerData.abilityPoint.ToString();
            }
            else
            {
                //ap ∫Œ¡∑

            }
        }
    }
}
