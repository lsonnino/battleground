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
        Player player1 = new Player(
            "0", "Player 1",
            new Warrior[]{new Warrior(Warrior.DINO), new Warrior(Warrior.DINO), null, null, null},
            new Potion[]{new Potion(Potion.HEAL), new Potion(Potion.STRENGTH), new Potion(Potion.WEAKNESS), new Potion(Potion.RUNNER), new Potion(Potion.AGING)}
        );
        Player player2 = new Player(
            "1", "Player 2",
            new Warrior[]{new Warrior(Warrior.DINO), new Warrior(Warrior.DINO), null, null, null},
            new Potion[]{new Potion(Potion.HEAL), new Potion(Potion.HEAL), new Potion(Potion.STRENGTH), null, null}
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