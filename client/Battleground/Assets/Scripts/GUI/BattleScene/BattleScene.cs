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
        this.gameState = User.gameState;
        this.field = new Field(nonCollidingTilemaps, collidingTilemaps);

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
