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
        Debug.Log("������ ȿ�� ����: " + itemName);
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
        Debug.Log("ü�� ȸ��: " + healAmount);
    }
}

public class AmmoItem : Item
{
    public AmmoItem() : base("Ammo") { }

    public override void ApplyEffect(MyPlayer player)
    {
        player.StrengthenBasicAttack();
        Debug.Log("�Ѿ� ��ȭ");
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
        // PlayerMovement ��ũ��Ʈ�� �����ͼ� �̵� �ӵ� ������ ����
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.IncreaseSpeed(speedIncrease, duration);  // PlayerMovement�� IncreaseSpeed ȣ��
            Debug.Log("�ӵ� ����: " + speedIncrease + " for " + duration + " seconds");
        }
        else
        {
            Debug.LogError("PlayerMovement ��ũ��Ʈ�� ã�� �� �����ϴ�.");
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
        Debug.Log("���� ������ ȿ�� �����");
    }
}
