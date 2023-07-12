using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillIconEffect : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Show(Unit owner, Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
        spriteRenderer.DOFade(0, 0.5f).SetEase(Ease.InQuint).OnComplete(() =>
        {
            Destroy(gameObject);
        });
        spriteRenderer.sortingOrder = owner.MySpriteRenderer.sortingOrder - (int)LayerOrder.Unit + (int)LayerOrder.Fog;
    }
}
