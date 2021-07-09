using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventsHandler : MonoBehaviour
{
    // Elements to be set in Editor
    public BattleScene gameMaster;
    public PlayableCharacters playableCharacters;
    public MapEvents map;
    public GameObject selectorPane;
    public Sprite transparentImage;
    public GameObject warriorPlacer;

    // Internal state to keep track of events
    private int turn;
    private GameState.Phase phase;

    // Internal state to handle events
    public Warrior selectedWarrior;

    void Start() {
        HideWarriorSelector();
        this.phase = GameState.Phase.NONE;
        this.turn = -1;

        this.selectedWarrior = null;
    }
    void Update() {
        // === Check for events =================

        if (this.turn != gameMaster.gameState.GetPlayerTurn()) {
            NewTurnEvent(gameMaster.gameState.GetPlayerTurn());
        }

        if (this.phase != gameMaster.gameState.GetPhase()) {
            NewPhaseEvent(gameMaster.gameState.GetPhase());
        }


        // === Handle current events =================

        if (!this.gameMaster.gameState.IsThisPlayerTurn()) {
            return;
        }

        Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int selectedTile = map.padsTilemap.WorldToCell(point);

        // Summon a warrior
        if (this.phase == GameState.Phase.SUMMON && this.selectedWarrior != null) {
            // Show where the warrior will be placed
            this.map.RemovePads();
            if (gameMaster.field.IsWalkable(selectedTile.x, selectedTile.y)) {
                this.map.AddPad(selectedTile, false);
            }

            // Place it
            if (Input.GetMouseButton(0)) {
                // Place the warrior
                this.gameMaster.field.MoveWarrior(
                    this.selectedWarrior,
                    selectedTile.x, selectedTile.y
                );

                // Make him appear
                var placer = Instantiate(warriorPlacer, new Vector3(selectedTile.x + 0.5f, selectedTile.y + 1, 0), Quaternion.identity);
                var war = Instantiate(
                    playableCharacters.playableCharacters[Warrior.GetWarriorIndex(this.selectedWarrior)],
                    Vector3.zero,
                    Quaternion.identity
                );
                placer.transform.parent = this.map.transform.parent;
                war.transform.parent = placer.transform;
                war.transform.localScale = Vector3.one;
                war.transform.localPosition = Vector3.zero;

                // Remove pads
                this.map.RemovePads();

                // End the summoning phase
                this.gameMaster.NextPhase();
            }
        }
        // Move a warrior
        else if (this.phase == GameState.Phase.MOVE_1 || this.phase == GameState.Phase.MOVE_2) {
            if (this.selectedWarrior == null) {
                this.map.RemovePads();

                // Select the warrior to move
                Warrior hover = this.gameMaster.gameState.GetCurrentPlayer().GetWarriorAt(selectedTile.x, selectedTile.y);
                if (hover != null && !hover.moved) {
                    this.map.AddPad(selectedTile, false);

                    // Select it
                    if (Input.GetMouseButtonDown(0)) {
                        this.selectedWarrior = hover;
                        this.map.ShowPositions(this.selectedWarrior);
                        this.map.AddPad(selectedTile, true);
                    }
                }
            }
            else if (Input.GetMouseButtonDown(0)){
                // Move the warrior
                if (map.padsTilemap.HasTile(selectedTile) &&
                    (selectedTile.x != this.selectedWarrior.GetX() || selectedTile.y != this.selectedWarrior.GetY())) {
                    // The warrior can be moved to that position
                    this.gameMaster.field.MoveWarrior(this.selectedWarrior, selectedTile.x, selectedTile.y);
                    this.selectedWarrior = null;
                }
                else {
                    // Has the user selected another warrior ?
                    Warrior newSelection = this.gameMaster.gameState.GetCurrentPlayer().GetWarriorAt(selectedTile.x, selectedTile.y);
                    if (newSelection != null && !newSelection.moved) {
                        this.selectedWarrior = newSelection;
                        this.map.ShowPositions(this.selectedWarrior);
                        this.map.AddPad(selectedTile, true);
                    }
                    else {
                        // Remove selection
                        this.selectedWarrior = null;
                    }
                }
                this.map.RemovePads();
            }
        }
    }

    private void NewTurnEvent(int newTurn) {
        if (this.phase == GameState.Phase.SUMMON) {
            if (!gameMaster.gameState.IsThisPlayerTurn()) {
                HideWarriorSelector();
            }
        }

        this.turn = newTurn;
    }
    private void NewPhaseEvent(GameState.Phase newPhase) {
        switch(newPhase) {
            case GameState.Phase.SUMMON:
                if (gameMaster.gameState.IsThisPlayerTurn()) {
                    SelectWarrior();
                }
                break;
            default:
                break;
        }

        this.phase = newPhase;
        this.selectedWarrior = null;
    }

    private void SelectWarrior() {
        for (int i=0 ; i < Player.MAX_WARRIORS ; i++) {
            Warrior warrior = gameMaster.gameState.GetCurrentPlayer().GetWarrior(i);

            // No warriors or already placed warrior
            if (warrior == null || warrior.IsPlaced()) {
                continue;
            }

            // Show warrior
            int index = Warrior.GetWarriorIndex(warrior);
            if (index < 0) { continue; }

            Image img = selectorPane.transform.GetChild(i).GetComponent<Image>();
            img.sprite = playableCharacters.GetWarriorImage(index);
        }

        selectorPane.SetActive(true);
    }
    private void HideWarriorSelector() {
        for (int i=0 ; i < Player.MAX_WARRIORS ; i++) {
            Image img = selectorPane.transform.GetChild(i).GetComponent<Image>();
            img.sprite = transparentImage;
        }

        this.selectorPane.SetActive(false);
        this.selectedWarrior = null;
    }
    public void SelectorPaneEntrySelected(int index) {
        if (index < 0 || index >= Player.MAX_WARRIORS) { return; }

        this.selectedWarrior = gameMaster.gameState.GetCurrentPlayer().GetWarrior(index);
    }
}
