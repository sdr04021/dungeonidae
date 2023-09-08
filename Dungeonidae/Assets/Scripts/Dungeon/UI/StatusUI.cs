using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatusUI : MonoBehaviour
{
    [SerializeField] DungeonManager dm;
    [SerializeField] TMP_Text[] statusValues = new TMP_Text[30];

    public void UpdateStatus()
    {
        statusValues[0].text = (dm.Player.UnitData.level + 1).ToString();
        statusValues[1].text = "Warrior";
        statusValues[2].text = dm.Player.UnitData.Hp.ToString() + "/" + dm.Player.UnitData.maxHp.Total().ToString();
        statusValues[3].text = dm.Player.UnitData.Mp.ToString() + "/" + dm.Player.UnitData.maxMp.Total().ToString();
        statusValues[4].text = dm.Player.UnitData.exp.ToString() + "/" + dm.Player.UnitData.maxExp.ToString();
        statusValues[5].text = dm.Player.UnitData.Hunger.ToString() + "/" + dm.Player.UnitData.maxHunger.Total().ToString();
        statusValues[6].text = dm.Player.UnitData.atk.Total().ToString();
        statusValues[7].text = dm.Player.UnitData.def.Total().ToString();
        statusValues[8].text = dm.Player.UnitData.mAtk.Total().ToString();
        statusValues[9].text = dm.Player.UnitData.mDef.Total().ToString();
        statusValues[10].text = dm.Player.UnitData.acc.Total().ToString();
        statusValues[11].text = dm.Player.UnitData.eva.Total().ToString();
        statusValues[12].text = dm.Player.UnitData.aspd.Total().ToString() + "%";
        statusValues[13].text = dm.Player.UnitData.atkRange.Total().ToString();
        statusValues[14].text = dm.Player.UnitData.pen.Total().ToString() + "%";
        statusValues[15].text = dm.Player.UnitData.mPen.Total().ToString() + "%";
        statusValues[16].text = dm.Player.UnitData.cri.Total().ToString() + "%";
        statusValues[17].text = dm.Player.UnitData.criDmg.Total().ToString() + "%";
        statusValues[18].text = dm.Player.UnitData.proficiency.Total().ToString() + "%";
        statusValues[19].text = dm.Player.UnitData.speed.Total().ToString() + "%";
        statusValues[20].text = dm.Player.UnitData.hpRegen.Total().ToString();
        statusValues[21].text = dm.Player.UnitData.mpRegen.Total().ToString();
        statusValues[22].text = dm.Player.UnitData.sight.Total().ToString();
        statusValues[23].text = dm.Player.UnitData.searchRange.Total().ToString();
        statusValues[24].text = dm.Player.UnitData.lifeSteal.Total().ToString() + "%";
        statusValues[25].text = dm.Player.UnitData.manaSteal.Total().ToString() + "%";
        statusValues[26].text = dm.Player.UnitData.resist.Total().ToString() + "%";
        statusValues[27].text = dm.Player.UnitData.tolerance.Total().ToString() + "%";
        statusValues[28].text = dm.Player.UnitData.dmgIncrease.Total().ToString() + "%";
        statusValues[29].text = dm.Player.UnitData.dmgReduction.Total().ToString() + "%";
    }
}
