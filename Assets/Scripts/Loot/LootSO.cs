using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Loot", menuName = "Inventory/Loot")]
public class LootSO : ScriptableObject
{
  public string lootName;
  [TextArea] public string lootDescription;
  public Sprite lootIcon;

  [Header("Loot Stats")]
  public int currentHealth;
  public int maxHealth;
  public float moveSpeed;

  [Header("Temporary Stats")]
  public float duration;
}
