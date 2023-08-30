using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEffect : MonoBehaviour
{
    public Animator Animator { get; private set; }
    SpriteRenderer spriteRenderer;
    bool started = false;

    private void Awake()
    {
        Animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void ShowEffect(string key, Unit owner)
    {
        spriteRenderer.sortingOrder = 1000; //owner.MySpriteRenderer.sortingOrder - (int)LayerOrder.Unit + (int)LayerOrder.ItemObject;
        transform.localScale = owner.transform.localScale;
        Animator.SetTrigger(key);
        started = true;
    }

    private void Update()
    {
        if (started)
        {
            if (Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                Destroy(gameObject);
            }
        }
    }
}
