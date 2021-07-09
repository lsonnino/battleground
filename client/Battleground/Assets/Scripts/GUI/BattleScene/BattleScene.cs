using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class BattleScene : MonoBehaviour
{
    // Debug variables
    public bool debug = true;

    // State
    public GameState gameState;
    public Field field;
    public bool isLooking;
    public bool start;

    // Graphical Elements
    public GameObject nextPhaseButton, inventoryButton;
    public PhaseIndicator phaseIndicator;
    public TurnText turnText;

    // Tilemaps
    public Tilemap[] nonCollidingTilemaps, collidingTilemaps;

    // TODO: has to be modified once starting menu's have been done
    void Start() {
        // TODO: Only keep this part until the menu's are not complete
        // begin {
        Player player1 = new Player(
            "0", "Player 1",
            new Warrior[]{new Warrior(Warrior.GLADIATOR), new Warrior(Warrior.GLADIATOR), null, null, null},
            new Potion[]{new Potion(Potion.HEAL), new Potion(Potion.STRENGTH), new Potion(Potion.WEAKNESS), new Potion(Potion.RUNNER), new Potion(Potion.AGING)}
        );
        Player player2 = new Player(
            "1", "Player 2",
            new Warrior[]{new Warrior(Warrior.GLADIATOR), new Warrior(Warrior.GLADIATOR), null, null, null},
            new Potion[]{new Potion(Potion.HEAL), new Potion(Potion.HEAL), new Potion(Potion.STRENGTH), null, null}
        );
        this.gameState = new GameState(new Player[]{player1, player2}, 0);

        this.field = new Field(nonCollidingTilemaps, collidingTilemaps);
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

        // Do not permit to use inventory during summoning phase
        if (gameState.GetPhase() == GameState.Phase.SUMMON) {
            inventoryButton.SetActive(false);
        }
    }
}
