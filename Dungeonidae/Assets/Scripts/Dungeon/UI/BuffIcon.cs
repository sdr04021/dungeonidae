using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffIcon : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] Image curtain;
    [SerializeField] TMP_Text stackText;
    DungeonUIManager dunUI;

    public void Init(DungeonUIManager dunUI)
    {
        this.dunUI = dunUI;
    }

    public void SetBuff(BuffData buff)
    {
        image.sprite = buff.BaseData.Sprite;
        curtain.fillAmount = 0;
    }

    public void SetDuration(BuffData buff)
    {
        curtain.fillAmount = 1 - (buff.durationLeft / (float)buff.MaxDuration);
    }

    public void SetStack(BuffData buff)
    {
        if (buff.stack > 0) stackText.text = buff.stack.ToString();
    }

    public void Btn_BuffIconClicked()
    {

    }
}
