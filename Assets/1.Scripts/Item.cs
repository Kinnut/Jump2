using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public string itemName;

    public Item(string name)
    {
        itemName = name;
    }

    public virtual void ApplyEffect(Player player)
    {
        Debug.Log("아이템 효과 적용: " + itemName);
    }
}

public class HealItem : Item
{
    public float healAmount;

    public HealItem(float healAmount) : base("Heal")
    {
        this.healAmount = healAmount;
    }

    public override void ApplyEffect(Player player)
    {
        player.Heal(healAmount);
        Debug.Log("체력 회복: " + healAmount);
    }
}

public class AmmoItem : Item
{
    public AmmoItem() : base("Ammo") { }

    public override void ApplyEffect(Player player)
    {
        player.StrengthenBasicAttack();
        Debug.Log("총알 강화");
    }
}

public class BoostItem : Item
{
    public float speedIncrease;
    public float duration;

    public BoostItem(float speedIncrease, float duration) : base("Boost")
    {
        this.speedIncrease = speedIncrease;
        this.duration = duration;
    }

    public override void ApplyEffect(Player player)
    {
        player.IncreaseSpeed(speedIncrease, duration);
        Debug.Log("속도 증가: " + speedIncrease + " for " + duration + " seconds");
    }
}

public class QuestionItem : Item
{
    private List<Item> possibleItems;

    public QuestionItem(List<Item> items) : base("Question")
    {
        possibleItems = items;
    }

    public override void ApplyEffect(Player player)
    {
        int randomIndex = Random.Range(0, possibleItems.Count);
        possibleItems[randomIndex].ApplyEffect(player);
        Debug.Log("랜덤 아이템 효과 적용됨");
    }
}
