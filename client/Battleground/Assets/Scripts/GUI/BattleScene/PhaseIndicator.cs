using UnityEngine;
using UnityEngine.UI;

public class PhaseIndicator : MonoBehaviour
{
    public Sprite move1Indicator, attackIndicator, move2Indicator, summonIndicator;
    public Image indicator;

    void Start() {
        indicator = this.GetComponent<Image>();
    }
    void Update() {}

    public void UpdateIndicator(GameState.Phase phase) {
        switch (phase) {
            case GameState.Phase.MOVE_1:
                this.indicator.sprite = move1Indicator;
                break;
            case GameState.Phase.ATTACK:
                this.indicator.sprite = attackIndicator;
                break;
            case GameState.Phase.MOVE_2:
                this.indicator.sprite = move2Indicator;
                break;
            case GameState.Phase.SUMMON:
                this.indicator.sprite = summonIndicator;
                break;
            default:
                this.indicator.sprite = move1Indicator;
                break;
        }
    }
}
