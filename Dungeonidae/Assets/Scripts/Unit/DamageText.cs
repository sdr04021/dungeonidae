using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using DG.Tweening.Core;

public class DamageText : MonoBehaviour
{
    [SerializeField] TMP_Text tmpro;
    [SerializeField] RectTransform rect;
    Color healColor = new(0.5f, 1.244f, 0.5f);

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
