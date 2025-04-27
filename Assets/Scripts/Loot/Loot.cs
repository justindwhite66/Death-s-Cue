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

  // Event trigger when loot picked up
  public static event Action<LootSO, int> OnLootPickup;

  // Assign sprite and name
  public void OnValidate()
  {
    if (lootSO == null)
    {
      return;
    }

    sr.sprite = lootSO.lootIcon;
    this.name = lootSO.lootName;
  }

  // Plays animation and destroys object on pickup
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