using UnityEngine;
using UnityEngine.UI;

public class PotionStatPanel : MonoBehaviour
{
    public Text potionName, lifetimeValue, typeValue, amountValue;

    void Start() {
        Hide();
    }
    void Update() {}

    public void Show(Potion potion) {
        potionName.text = potion.name;

        lifetimeValue.text = potion.length > 0 ? "" + potion.length : "instant";
        amountValue.text = (potion.amount > 0 ? "+" : "") + potion.amount;

        switch (potion.type) {
            case Potion.PotionType.HP:
                typeValue.text = "health";
                break;
            case Potion.PotionType.WALKING_DISTANCE:
                typeValue.text = "movement";
                break;
            case Potion.PotionType.ATTACK:
                typeValue.text = "attack";
                break;
            default:
                break;
        }

        this.gameObject.SetActive(true);
    }
    public void Hide() {
        this.gameObject.SetActive(false);
    }
}
