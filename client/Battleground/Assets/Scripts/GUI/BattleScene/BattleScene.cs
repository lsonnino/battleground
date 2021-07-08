using UnityEngine;
using UnityEngine.UI;

public class BattleScene : MonoBehaviour
{
    // Debug variables
    public bool debug = true;

    // State
    public GameState gameState;
    public bool isLooking;

    // Graphical Elements
    public GameObject nextPhaseButton, inventoryButton;
    public PhaseIndicator phaseIndicator;
    public TurnText turnText;

    // TODO: has to be modified once starting menu's have been done
    void Start() {
        // TODO: Only keep this part until the menu's are not complete
        // begin {
        Warrior dino = new Warrior("0", "Dino", 5, 3, 2);

        Potion heal = new Potion("0", "Heal Potion", Potion.PotionType.HP, 0, 5);
        Potion strength = new Potion("1", "Strength Potion", Potion.PotionType.ATTACK, 5, 2);
        Potion weakness = new Potion("2", "Weakness Potion", Potion.PotionType.ATTACK, 5, -2);
        Potion runner = new Potion("3", "Runner Potion", Potion.PotionType.WALKING_DISTANCE, 5, 2);
        Potion aging = new Potion("4", "Aging Potion", Potion.PotionType.WALKING_DISTANCE, 5, -2);

        Player player1 = new Player(
            "0", "Player 1",
            new Warrior[]{new Warrior(dino), new Warrior(dino), null, null, null},
            new Potion[]{new Potion(heal), new Potion(strength), new Potion(weakness), new Potion(runner), new Potion(aging)}
        );
        Player player2 = new Player(
            "0", "Player 1",
            new Warrior[]{new Warrior(dino), new Warrior(dino), null, null, null},
            new Potion[]{new Potion(heal), new Potion(heal), new Potion(strength), null, null}
        );
        this.gameState = new GameState(new Player[]{player1, player2}, 0);
        // } end


        // Initialize state
        this.isLooking = false;

        // Initialize indicatiors
        UpdateUIIndicators();
    }
    void Update() {}

    public void NextPhase() {
        // End the turn
        gameState.NextPhase();

        // Update UI
        UpdateUIIndicators();
    }
    public void ToggleLooking() {
        this.isLooking = !this.isLooking;

        // Update UI
        UpdateUIIndicators();
    }
    private void UpdateUIIndicators() {
        phaseIndicator.UpdateIndicator(gameState.GetPhase());
        turnText.UpdateText(gameState.IsThisPlayerTurn());

        bool activeButtons = !isLooking && (debug || gameState.IsThisPlayerTurn());
        inventoryButton.SetActive(activeButtons);
        nextPhaseButton.SetActive(activeButtons);
    }
}
