using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DungeonUIManager : MonoBehaviour
{
    [SerializeField] DungeonManager dm;

    [SerializeField] TMP_Text lvText;
    [SerializeField] Image hpBar;
    [SerializeField] TMP_Text hpText;
    [SerializeField] Image mpBar;
    [SerializeField] TMP_Text mpText;
    [SerializeField] Image expBar;
    [SerializeField] TMP_Text expText;

    [SerializeField] GameObject menuCanvas;
    [SerializeField] GameObject status;
    [SerializeField] TMP_Text classText;
    [SerializeField] TMP_Text[] statusValues = new TMP_Text[30];

    enum Menu { Status, Equip, Inven, Ability, Skill, Soulstone }
    Menu currentMenu = Menu.Status;

    public void Init()
    {
        UpdateLevelText();
        UpdateHpBar();
        UpdateMpBar();
        UpdateExpBar();
    }

    public void UpdateLevelText()
    {
        lvText.text = "LV " + (dm.Player.Data.Level + 1).ToString();
    }

    public void UpdateHpBar()
    {
        int hp = dm.Player.Data.Hp;
        int maxHp = dm.Player.Data.maxHp.Total();
        hpBar.fillAmount = hp / (float)maxHp;
        hpText.text = hp.ToString() + "/" + maxHp.ToString();
    }

    public void UpdateMpBar()
    {
        int mp = dm.Player.Data.Mp;
        int maxMp = dm.Player.Data.maxMp.Total();
        mpBar.fillAmount = mp / (float)maxMp;
        mpText.text = mp.ToString() + "/" + maxMp.ToString();
    }

    public void UpdateExpBar()
    {
        int exp = dm.Player.Data.Exp;
        int maxExp = dm.Player.Data.MaxExp;
        expBar.fillAmount = exp / (float)maxExp;
        expText.text = exp.ToString() + "/" + maxExp.ToString() +"(" + Mathf.Round(exp/(float)maxExp*100) + "%)";
    }

    public void OpenMenuCanvas()
    {
        menuCanvas.SetActive(true);
        switch(currentMenu)
        {
            case Menu.Status:
                ShowStatus();
                break;
        }
    }
    public void CloseMenuCanvas()
    {
        menuCanvas.SetActive(false);
    }

    public void ShowStatus()
    {
        statusValues[0].text = (dm.Player.Data.Level + 1).ToString();
        statusValues[1].text = dm.Player.Data.Hp.ToString() + "/" + dm.Player.Data.maxHp.Total().ToString();
        statusValues[2].text = dm.Player.Data.Mp.ToString() + "/" + dm.Player.Data.maxMp.Total().ToString();
        statusValues[3].text = dm.Player.Data.Exp.ToString() + "/" + dm.Player.Data.MaxExp.ToString();
        statusValues[4].text = dm.Player.Data.Hunger.ToString() + "/" + dm.Player.Data.maxHunger.Total().ToString();
        statusValues[5].text = dm.Player.Data.atk.Total().ToString();
        statusValues[6].text = dm.Player.Data.mAtk.Total().ToString();
        statusValues[7].text = dm.Player.Data.atkRange.Total().ToString();
        statusValues[8].text = dm.Player.Data.pen.Total().ToString();
        statusValues[9].text = dm.Player.Data.mPen.Total().ToString();
        statusValues[10].text = dm.Player.Data.acc.Total().ToString();
        statusValues[11].text = dm.Player.Data.aspd.Total().ToString() + "%";
        statusValues[12].text = dm.Player.Data.cri.Total().ToString() + "%";
        statusValues[13].text = dm.Player.Data.criDmg.Total().ToString() + "%";
        statusValues[14].text = dm.Player.Data.proficiency.Total().ToString() + "%";
        statusValues[15].text = dm.Player.Data.lifeSteal.Total().ToString() + "%";
        statusValues[16].text = dm.Player.Data.manaSteal.Total().ToString() + "%";
        statusValues[17].text = dm.Player.Data.dmgIncrease.Total().ToString() + "%";
        statusValues[18].text = dm.Player.Data.def.Total().ToString();
        statusValues[19].text = dm.Player.Data.mDef.Total().ToString();
        statusValues[20].text = dm.Player.Data.eva.Total().ToString();
        statusValues[21].text = dm.Player.Data.block.Total().ToString() + "%";
        statusValues[22].text = dm.Player.Data.resist.Total().ToString() + "%";
        statusValues[23].text = dm.Player.Data.dmgReduction.Total().ToString() + "%";
        statusValues[24].text = dm.Player.Data.sight.Total().ToString();
        statusValues[25].text = dm.Player.Data.instinct.Total().ToString() + "%";
        statusValues[26].text = dm.Player.Data.searchRange.Total().ToString();
        statusValues[27].text = dm.Player.Data.hpRegen.Total().ToString();
        statusValues[28].text = dm.Player.Data.mpRegen.Total().ToString();
        statusValues[29].text = dm.Player.Data.speed.Total().ToString() + "%";
    }
}
