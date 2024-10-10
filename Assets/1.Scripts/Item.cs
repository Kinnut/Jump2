using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public string itemName;

    public Item(string name)
    {
        itemName = name;
    }

    public virtual void ApplyEffect(MyPlayer player)
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

    public override void ApplyEffect(MyPlayer player)
    {
        player.Heal(healAmount);
        Debug.Log("체력 회복: " + healAmount);
    }
}

public class AmmoItem : Item
{
    public AmmoItem() : base("Ammo") { }

    public override void ApplyEffect(MyPlayer player)
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

    public override void ApplyEffect(MyPlayer player)
    {
        // PlayerMovement 스크립트를 가져와서 이동 속도 증가를 적용
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.IncreaseSpeed(speedIncrease, duration);  // PlayerMovement의 IncreaseSpeed 호출
            Debug.Log("속도 증가: " + speedIncrease + " for " + duration + " seconds");
        }
        else
        {
            Debug.LogError("PlayerMovement 스크립트를 찾을 수 없습니다.");
        }
    }
}


public class QuestionItem : Item
{
    private List<Item> possibleItems;

    public QuestionItem(List<Item> items) : base("Question")
    {
        possibleItems = items;
    }

    public override void ApplyEffect(MyPlayer player)
    {
        int randomIndex = Random.Range(0, possibleItems.Count);
        possibleItems[randomIndex].ApplyEffect(player);
        Debug.Log("랜덤 아이템 효과 적용됨");
    }
}
