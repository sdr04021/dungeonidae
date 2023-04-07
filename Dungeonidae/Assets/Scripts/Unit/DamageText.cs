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
    Color expColor = new(1, 0.66f, 0.11f);

    public void SetValue(int amount, DamageType damageType)
    {
        switch (damageType)
        {
            case DamageType.Heal:
                tmpro.color = healColor;
                break;
        }

        tmpro.SetText(amount.ToString());
        //if (localScaleX < 0)
        //    rect.localScale = new Vector3(-rect.localScale.x, rect.localScale.y, rect.localScale.z);
        rect.DOAnchorPos(new Vector2(0, 75), 1);
        tmpro.DOFade(0, 1.5f).OnComplete(DestroyThis);
    }
    public void SetExpValue(int amount)
    {
        tmpro.color = expColor;
        tmpro.SetText("+" + amount.ToString());
        rect.DOAnchorPos(new Vector2(0, 75), 1);
        tmpro.DOFade(0, 1.5f).OnComplete(DestroyThis);
    }
    public void SetLevelUp()
    {
        tmpro.color = expColor;
        tmpro.SetText("Level Up");
        rect.DOAnchorPos(new Vector2(0, 75), 1);
        tmpro.DOFade(0, 1.5f).OnComplete(DestroyThis);
    }

    void DestroyThis()
    {
        Destroy(gameObject);  
    }

    private void OnDestroy()
    {
        rect.DOKill();
        tmpro.DOKill();
    }
}
