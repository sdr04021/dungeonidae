using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class DamageText : MonoBehaviour
{
    [SerializeField] TMP_Text tmpro;
    [SerializeField] RectTransform rect;
    Color healColor = new(0.5f, 1.244f, 0.5f);
    Color expColor = new(0.8745f, 1, 0);
    Color blockColor = new(0.5f, 0.5f, 0.5f);
    Color criticalColor = new(1, 0, 0);

    public void SetValue(int amount, DamageType damageType)
    {
        switch (damageType)
        {
            case DamageType.Heal:
                tmpro.color = healColor;
                break;
            case DamageType.Critical:
                tmpro.color = criticalColor;
                break;
        }
        tmpro.SetText(amount.ToString());

        //if (localScaleX < 0)
        //    rect.localScale = new Vector3(-rect.localScale.x, rect.localScale.y, rect.localScale.z);
        GeneralFade();
    }
    public void SetMiss()
    {
        tmpro.SetText("Miss");
        GeneralFade();
    }
    public void SetBlock()
    {
        tmpro.color = blockColor;
        tmpro.SetText("Block");
        GeneralFade();
    }
    public void SetExpValue(int amount)
    {
        tmpro.color = expColor;
        tmpro.SetText("+" + amount.ToString());
        GeneralFade();
    }
    public void SetLevelUp()
    {
        tmpro.color = expColor;
        tmpro.SetText("Level Up");
        rect.DOAnchorPos(new Vector2(0, 75), 1);
        tmpro.DOFade(0, 1.33f).OnComplete(() => { Destroy(gameObject); });
    }

    void GeneralFade()
    {
        rect.DOAnchorPos(new Vector2(0, 85), 0.6f).OnComplete(() => { Destroy(gameObject); });
        tmpro.DOFade(0, 0.6f).SetEase(Ease.InQuint);
        //rect.DOScale(0.75f, 0.5f).SetEase(Ease.Linear);
    }

    private void OnDestroy()
    {
        rect.DOKill();
        tmpro.DOKill();
    }
}
