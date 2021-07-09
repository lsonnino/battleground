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
    public WarriorStatPanel statPanel;

    // Internal state to keep track of events
    private int turn;
    private GameState.Phase phase;

    // Internal state to handle events
    public Warrior selectedWarrior;
    private List<WarriorGUI> warriorsGUI;

    void Start() {
        HideWarriorSelector();
        this.phase = GameState.Phase.NONE;
        this.turn = -1;

        this.selectedWarrior = null;
        this.warriorsGUI = new List<WarriorGUI>();
    }
    void Update() {
        // === Check for events =================

        if (this.turn != gameMaster.gameState.GetPlayerTurn()) {
            NewTurnEvent(gameMaster.gameState.GetPlayerTurn());
        }

        if (this.phase != gameMaster.gameState.GetPhase()) {
            NewPhaseEvent(gameMaster.gameState.GetPhase());
        }

        Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int selectedTile = map.padsTilemap.WorldToCell(point);

        Warrior w = GetWarriorAt(selectedTile.x, selectedTile.y);
        if (w != null) {
            statPanel.Show(w);
        }
        else {
            statPanel.Hide();
        }


        // === Handle current events =================

        if (!this.gameMaster.gameState.IsThisPlayerTurn()) {
            return;
        }

        // Summon a warrior
        if (this.phase == GameState.Phase.SUMMON && this.selectedWarrior != null) {
            // Show where the warrior will be placed
            this.map.RemovePads();
            if (gameMaster.field.IsWalkable(selectedTile.x, selectedTile.y)) {
                this.map.AddPad(selectedTile, false);
            }

            // Place it
            if (Input.GetMouseButton(0)) {
                this.Summon(this.selectedWarrior, selectedTile.x, selectedTile.y, true);

                // Remove pads
                this.map.RemovePads();

                // End the summoning phase
                this.gameMaster.NextPhase();
            }
        }
        // Move a warrior
        else if (this.phase == GameState.Phase.MOVE_1 || this.phase == GameState.Phase.MOVE_2) {
            HandleMovement(selectedTile);
        }
        else if (this.phase == GameState.Phase.ATTACK) {
            HandleAttack(selectedTile);
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
        this.map.RemovePads();
    }

    private WarriorGUI GetWarriorGUI(Warrior warrior) {
        WarriorGUI gui = null;
        for (int i=0 ; i < warriorsGUI.Count ; i++) {
            if (warriorsGUI[i].warrior.Equals(warrior)) {
                gui = warriorsGUI[i];
                break;
            }
        }

        return gui;
    }
    private Warrior GetWarriorAt(int x, int y) {
        for (int i=0 ; i < this.gameMaster.gameState.GetNumberOfPlayers() ; i++) {
            Player p = this.gameMaster.gameState.GetPlayer(i);
            Warrior w = p.GetWarriorAt(x, y);
            if (w != null) {
                return w;
            }
        }

        return null;
    }

    // === SUMMON ============================
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

    public void Summon(Warrior warrior, int toX, int toY, bool isThisPlayer) {
        // Place the warrior
        this.gameMaster.field.MoveWarrior(warrior, toX, toY);

        // Make him appear
        var placer = Instantiate(
            warriorPlacer,
            new Vector3(toX + 0.5f, toY + 1, 5f + ((float) toY) / 100f),
            Quaternion.identity
        );
        var war = Instantiate(
            playableCharacters.playableCharacters[Warrior.GetWarriorIndex(this.selectedWarrior)],
            Vector3.zero,
            Quaternion.identity
        );
        Vector3 initialScale = placer.transform.localScale;
        placer.transform.parent = this.map.transform.parent;
        placer.transform.localScale = initialScale;
        war.transform.parent = placer.transform;
        war.transform.localScale = Vector3.one;
        war.transform.localPosition = Vector3.zero;

        WarriorGUI warGUI = war.GetComponent<WarriorGUI>();
        warGUI.Init(warrior, isThisPlayer);
        warriorsGUI.Add(warGUI);
    }

    // === MOVE ============================
    private void HandleMovement(Vector3Int selectedTile) {
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
                this.MoveWarrior(this.selectedWarrior, selectedTile.x, selectedTile.y);
                this.selectedWarrior = null;
                this.map.RemovePads();
            }
            else {
                this.map.RemovePads();
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
        }
    }

    public void MoveWarrior(Warrior warrior, int toX, int toY) {
        // Get the WarriorGUI
        WarriorGUI gui = GetWarriorGUI(warrior);
        if (gui == null) { return; } // Should never happen

        // Start the movement graphically
        gui.Move(this.gameMaster.field.GetPath(
            warrior.GetX(), warrior.GetY(),
            toX, toY
        ));

        // Move the warrior internally
        this.gameMaster.field.MoveWarrior(warrior, toX, toY);
    }

    // === ATTACK ============================
    private void HandleAttack(Vector3Int selectedTile) {
        if (this.selectedWarrior == null) {
            this.map.RemovePads();

            // Select the attacker
            Warrior hover = this.gameMaster.gameState.GetCurrentPlayer().GetWarriorAt(selectedTile.x, selectedTile.y);
            if (hover != null && !hover.attacked) {
                this.map.AddPad(selectedTile, true);

                // Select it
                if (Input.GetMouseButtonDown(0)) {
                    this.selectedWarrior = hover;
                }
            }
        }
        else if (Input.GetMouseButtonDown(0)){
            // Has the user selected another warrior ?
            Warrior defender = this.GetWarriorAt(selectedTile.x, selectedTile.y);
            if (defender != null) {
                if (defender.Equals(this.selectedWarrior)) {
                    // The same warrior has been selected: ignore
                    return;
                }

                if (Mathf.Abs(defender.GetX() - this.selectedWarrior.GetX()) +
                    Mathf.Abs(defender.GetY() - this.selectedWarrior.GetY()) > 1) {
                    // The two are too far away
                    return;
                }

                // Attack
                Attack(this.selectedWarrior, defender);
            }

            // Remove selection (even if the user selected something else)
            this.selectedWarrior = null;
            this.map.RemovePads();
        }
    }

    public void Attack(Warrior attacker, Warrior defender) {
        // Get the WarriorGUI
        WarriorGUI gui = GetWarriorGUI(attacker);
        if (gui == null) { return; } // Should never happen

        // Play the animation
        gui.Attack();

        // Hurt the other
        defender.Damage(attacker.GetAttack());
        attacker.attacked = true;

        // Handle death
        if (!defender.IsAlive()) {
            // Get the WarriorGUI
            gui = GetWarriorGUI(defender);
            if (gui == null) { return; } // Should never happen

            // Play the die animation
            gui.Die();
            // Remove him from the list
            warriorsGUI.Remove(gui);
        }
    }
}
