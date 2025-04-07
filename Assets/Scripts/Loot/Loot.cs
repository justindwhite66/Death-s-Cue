using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour
{
  public LootSO lootSO;
  public SpriteRenderer sr;
  public Animator anim;

  public int quantity;
  public static event Action<LootSO, int> OnLootPickup;

  private void OnValidate()
  {
    if (lootSO == null)
    {
      return;
    }

    sr.sprite = lootSO.lootIcon;
    this.name = lootSO.lootName;
  }

  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (collision.CompareTag("Player"))
    {
      anim.Play("LootPickup");
      OnLootPickup?.Invoke(lootSO, quantity);
      Destroy(gameObject, 0.5f);
    }
  }
}
