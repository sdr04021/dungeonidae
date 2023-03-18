using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class DamageText : MonoBehaviour
{
    [SerializeField] TMP_Text tmpro;
    [SerializeField] RectTransform rect;

    public void SetValue(int amount)
    {
        tmpro.SetText(amount.ToString());

        //if (localScaleX < 0)
        //    rect.localScale = new Vector3(-rect.localScale.x, rect.localScale.y, rect.localScale.z);
        rect.DOAnchorPos(new Vector2(0, 65), 1);
        tmpro.DOFade(0, 1).OnComplete(DestroyThis);
    }

    void DestroyThis()
    {
        Destroy(gameObject);    
    }
}
