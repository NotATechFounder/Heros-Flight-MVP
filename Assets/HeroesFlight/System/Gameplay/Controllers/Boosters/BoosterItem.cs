using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoosterItem : MonoBehaviour
{
    public Func<BoosterItem, bool> OnBoosterInteracted;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private BoosterSO boosterSO;

    public BoosterSO BoosterSO => boosterSO;

    public void Initialize(BoosterSO booster,Func<BoosterItem, bool>  func)
    {
        boosterSO = booster;
        OnBoosterInteracted = func;
        spriteRenderer.sprite = boosterSO.BoosterSprite;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out CharacterStatController characterStatController))
        {
            if (OnBoosterInteracted != null)
            {
                if (OnBoosterInteracted.Invoke(this))
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
