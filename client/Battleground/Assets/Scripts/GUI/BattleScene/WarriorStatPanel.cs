using UnityEngine;
using UnityEngine.UI;

public class WarriorStatPanel : MonoBehaviour
{
    public Text warriorName, atkValue, wdValue, lifeValue;
    public Image image;
    public RectTransform life, lifeContainer;
    public PlayableCharacters playableCharacters;

    private float maxSize;

    void Start() {
        Hide();

        maxSize = lifeContainer.rect.width;
    }
    void Update() {}

    public void Show(Warrior warrior) {
        warriorName.text = warrior.name;
        image.sprite = playableCharacters.GetWarriorImage(Warrior.GetWarriorIndex(warrior));

        atkValue.text = "" + warrior.GetAttack();
        wdValue.text = "" + warrior.GetWalkingDistance();
        lifeValue.text = "" + warrior.GetHP();

        float hp = (float) warrior.GetHP();
        float max = (float) warrior.GetMaxHP();
        life.offsetMax = new Vector2(
            - (1f - hp / max) * maxSize,
            life.offsetMax.y
        );

        this.gameObject.SetActive(true);
    }
    public void Hide() {
        this.gameObject.SetActive(false);
    }
}
