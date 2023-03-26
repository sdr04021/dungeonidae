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
        statusValues[0].text = (dm.Player.UnitData.Level + 1).ToString();
        statusValues[1].text = dm.Player.UnitData.Hp.ToString() + "/" + dm.Player.UnitData.MaxHp.Total().ToString();
        statusValues[2].text = dm.Player.UnitData.Mp.ToString() + "/" + dm.Player.UnitData.maxMp.Total().ToString();
        statusValues[3].text = dm.Player.UnitData.Exp.ToString() + "/" + dm.Player.UnitData.MaxExp.ToString();
        statusValues[4].text = dm.Player.UnitData.Hunger.ToString() + "/" + dm.Player.UnitData.MaxHunger.Total().ToString();
        statusValues[5].text = dm.Player.UnitData.Atk.Total().ToString();
        statusValues[6].text = dm.Player.UnitData.MAtk.Total().ToString();
        statusValues[7].text = dm.Player.UnitData.AtkRange.Total().ToString();
        statusValues[8].text = dm.Player.UnitData.Pen.Total().ToString();
        statusValues[9].text = dm.Player.UnitData.MPen.Total().ToString();
        statusValues[10].text = dm.Player.UnitData.Acc.Total().ToString();
        statusValues[11].text = dm.Player.UnitData.Aspd.Total().ToString() + "%";
        statusValues[12].text = dm.Player.UnitData.Cri.Total().ToString() + "%";
        statusValues[13].text = dm.Player.UnitData.CriDmg.Total().ToString() + "%";
        statusValues[14].text = dm.Player.UnitData.Proficiency.Total().ToString() + "%";
        statusValues[15].text = dm.Player.UnitData.LifeSteal.Total().ToString() + "%";
        statusValues[16].text = dm.Player.UnitData.ManaSteal.Total().ToString() + "%";
        statusValues[17].text = dm.Player.UnitData.DmgIncrease.Total().ToString() + "%";
        statusValues[18].text = dm.Player.UnitData.Def.Total().ToString();
        statusValues[19].text = dm.Player.UnitData.MDef.Total().ToString();
        statusValues[20].text = dm.Player.UnitData.Eva.Total().ToString();
        statusValues[21].text = dm.Player.UnitData.Block.Total().ToString() + "%";
        statusValues[22].text = dm.Player.UnitData.Resist.Total().ToString() + "%";
        statusValues[23].text = dm.Player.UnitData.DmgReduction.Total().ToString() + "%";
        statusValues[24].text = dm.Player.UnitData.Sight.Total().ToString();
        statusValues[25].text = dm.Player.UnitData.Instinct.Total().ToString() + "%";
        statusValues[26].text = dm.Player.UnitData.SearchRange.Total().ToString();
        statusValues[27].text = dm.Player.UnitData.HpRegen.Total().ToString();
        statusValues[28].text = dm.Player.UnitData.MpRegen.Total().ToString();
        statusValues[29].text = dm.Player.UnitData.Speed.Total().ToString() + "%";
    }
}
