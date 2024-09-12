using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public string itemType;

    private Item item;

    private void Start()
    {
        switch (itemType)
        {
            case "Heal":
                item = new HealItem(20f);
                break;
            case "Ammo":
                item = new AmmoItem();
                break;
            case "Boost":
                item = new BoostItem(3f, 10f);
                break;
            case "Question":
                item = new QuestionItem(new System.Collections.Generic.List<Item>
                {
                    new HealItem(20f),
                    new AmmoItem(),
                    new BoostItem(3f, 10f)
                });
                break;
            default:
                Debug.LogError("아이템 타입이 잘못되었습니다!");
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();

        if (player != null && item != null)
        {
            item.ApplyEffect(player);
            Destroy(gameObject);
        }
        else
        {
            Debug.LogWarning("Player or Item is missing!");
        }
    }
}
