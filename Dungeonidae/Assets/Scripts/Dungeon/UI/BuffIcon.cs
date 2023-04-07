using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffIcon : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] Image curtain;
    DungeonUIManager dunUI;

    public void Init(DungeonUIManager dunUI)
    {
        this.dunUI = dunUI;
    }

    public void SetBuff(BuffData buff)
    {
        image.sprite = buff.Sprite;
        curtain.fillAmount = 0;
    }

    public void SetDuration(BuffData buff)
    {
        curtain.fillAmount = 1 - (buff.durationLeft / (float)buff.MaxDuration);
    }

    public void Btn_BuffIconClicked()
    {

    }
}
