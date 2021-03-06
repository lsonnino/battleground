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
    public SelectorPane selectorPane;
    public WarriorStatPanel statPanel;
    public PotionStatPanel potionStatPanel;
    public Sprite potionSprite;
    public IO io;
    public WonLost wonLost;

    // Internal state to keep track of events
    private int turn;
    private GameState.Phase phase;
    private int summoned; // The number of warriors that were summoned by the player
    private int attacked, moved; // The number of warriors that moved/attacked this player's turn
    private int killed; // The number of this player's warriors that were killed

    // Internal state to handle events
    public Warrior selectedWarrior;
    public int selectedItemIndex;
    private List<WarriorGUI> warriorsGUI;
    private int openSelector;

    void Start() {
        HideSelector();
        this.selectorPane.callback = index => this.SelectorPaneEntrySelected(index);
        this.phase = GameState.Phase.NONE;
        this.turn = -1;
        this.summoned = this.killed = 0;
        this.attacked = this.moved = 0;

        this.selectedWarrior = null;
        this.selectedItemIndex = -1;
        this.warriorsGUI = new List<WarriorGUI>();
        this.openSelector = 0;
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

        // Do not register clicks on the open selector (except by the selector itself)
        if (selectorPane.ContainsMouse()) { return; }

        // Summon a warrior
        if (this.phase == GameState.Phase.SUMMON && this.selectedWarrior != null) {
            // Show where the warrior will be placed
            this.map.RemovePads();
            if (gameMaster.field.IsWalkable(selectedTile.x, selectedTile.y)) {
                this.map.AddPad(selectedTile, false);
            }

            // Place it
            if (Input.GetMouseButton(0)) {
                this.io.Summon(this.selectedWarrior, selectedTile.x, selectedTile.y);

                // Remove pads
                this.map.RemovePads();

                // End the turn
                this.io.NextPhase();
            }
        }
        else {
            // Use item
            if (this.selectedItemIndex >= 0) {
                HandleItem(selectedTile);
            }
            // Move a warrior
            else if (this.phase == GameState.Phase.MOVE_1 || this.phase == GameState.Phase.MOVE_2) {
                HandleMovement(selectedTile);
            }
            // Attack a warrior
            else if (this.phase == GameState.Phase.ATTACK) {
                HandleAttack(selectedTile);
            }
        }

    }

    private void NewTurnEvent(int newTurn) {
        if (this.phase == GameState.Phase.SUMMON) {
            if (!gameMaster.gameState.IsThisPlayerTurn()) {
                HideSelector();
            }
            else {
                // This player is the second player to play: the first player
                // summoned his first character, now it is this player's turn
                // -> Trigger a phase change to display the Warrior selector
                this.phase = GameState.Phase.NONE;
            }
        }

        this.turn = newTurn;
        this.attacked = this.moved = 0;
    }
    private void NewPhaseEvent(GameState.Phase newPhase) {
        switch(newPhase) {
            case GameState.Phase.MOVE_1:
                if (gameMaster.gameState.IsThisPlayerTurn() && this.summoned == this.killed) {
                    // Skip the phase
                    this.io.NextPhase();
                }
                break;
            case GameState.Phase.ATTACK:
                if (gameMaster.gameState.IsThisPlayerTurn() && this.attacked == this.summoned - this.killed) { // No warriors can attack
                    // Skip the phase
                    this.io.NextPhase();
                }
                break;
            case GameState.Phase.MOVE_2:
                if (gameMaster.gameState.IsThisPlayerTurn() && this.moved == this.summoned - this.killed) { // No warriors can move
                    // Skip the phase
                    this.io.NextPhase();
                }
                break;
            case GameState.Phase.SUMMON:
                if (gameMaster.gameState.IsThisPlayerTurn()) {
                    if (this.summoned == Player.MAX_WARRIORS) { // No warriors to summon
                        // Skip the phase
                        this.io.NextPhase();
                    }
                    else {
                        SelectWarrior();
                    }
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

    private void HideSelector() {
        selectorPane.gameObject.SetActive(false);

        switch(this.openSelector) {
            case 1:
                HideWarriorSelector();
                break;
            case 2:
                HideItemSelector();
                break;
            default:
                break;
        }
    }
    public void SelectorPaneEntrySelected(int index) {
        switch(this.openSelector) {
            case 1:
                if (index < 0 || index >= Player.MAX_WARRIORS) { return; }
                this.selectedWarrior = gameMaster.gameState.GetThisPlayer().GetWarrior(index);
                break;
            case 2:
                if (index < 0 || index >= Player.MAX_ITEMS) { return; }
                this.selectedItemIndex = index;

                Item item = this.gameMaster.gameState.GetThisPlayer().GetItem(index);
                if (item.GetType() == typeof(Potion)) {
                    potionStatPanel.Show((Potion) item);
                }

                break;
            default:
                break;
        }
    }

    // === SUMMON ============================
    private void SelectWarrior() {
        HideSelector();

        for (int i=0 ; i < Player.MAX_WARRIORS ; i++) {
            Warrior warrior = gameMaster.gameState.GetThisPlayer().GetWarrior(i);

            // No warriors or already placed warrior
            if (warrior == null || warrior.IsPlaced()) {
                selectorPane.SetEntry(i, null);
                continue;
            }

            // Show warrior
            int index = Warrior.GetWarriorIndex(warrior);
            if (index < 0) { continue; }

            selectorPane.SetEntry(i, playableCharacters.GetWarriorImage(index));
        }

        selectorPane.gameObject.SetActive(true);
        this.openSelector = 1;
    }
    private void HideWarriorSelector() {
        this.selectedWarrior = null;
        this.openSelector = 0;
    }

    /*
     * NOTE: should only be accessed by the Command class
     */
    public void Summon(Warrior warrior, int toX, int toY, bool isThisPlayer) {
        // Place the warrior
        this.gameMaster.field.MoveWarrior(warrior, toX, toY);
        // Keep track of him
        if (isThisPlayer) {
            this.summoned++;
            this.map.AddPlayerID(new Vector3Int(toX, toY, 0));
        }

        // Make him appear
        var placer = Instantiate(
            playableCharacters.playableCharacters[Warrior.GetWarriorIndex(warrior)],
            this.map.transform.parent,
            false
        );

        var war = placer.transform.GetChild(0);
        placer.transform.localPosition = new Vector3(toX, toY, 5f + ((float) toY) / 100f);
        WarriorGUI warGUI = war.GetComponent<WarriorGUI>();
        warGUI.Init(warrior, isThisPlayer);
        warriorsGUI.Add(warGUI);
    }

    // === MOVE ============================
    private void HandleMovement(Vector3Int selectedTile) {
        if (this.selectedWarrior == null) {
            this.map.RemovePads();

            // Select the warrior to move
            Warrior hover = this.gameMaster.gameState.GetThisPlayer().GetWarriorAt(selectedTile.x, selectedTile.y);
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
                this.io.Move(this.selectedWarrior, selectedTile.x, selectedTile.y);
                this.selectedWarrior = null;
                this.map.RemovePads();
            }
            else {
                this.map.RemovePads();
                // Did the user select another warrior ?
                Warrior newSelection = this.gameMaster.gameState.GetThisPlayer().GetWarriorAt(selectedTile.x, selectedTile.y);
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

    /*
     * NOTE: should only be accessed by the Command class
     */
    public void MoveWarrior(Warrior warrior, int toX, int toY) {
        // Get the WarriorGUI
        WarriorGUI gui = GetWarriorGUI(warrior);
        if (gui == null) { return; } // Should never happen
        // Keep track of him
        if (gui.isThisPlayer) {
            this.moved++;
            this.map.RemovePlayerID(new Vector3Int(warrior.GetX(), warrior.GetY(), 0));
            this.map.AddPlayerID(new Vector3Int(toX, toY, 0));
        }

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
            Warrior hover = this.gameMaster.gameState.GetThisPlayer().GetWarriorAt(selectedTile.x, selectedTile.y);
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
                this.io.Attack(this.selectedWarrior, defender);
            }

            // Remove selection (even if the user selected something else)
            this.selectedWarrior = null;
            this.map.RemovePads();
        }
    }

    /*
     * NOTE: should only be accessed by the Command class
     */
    public void Attack(Warrior attacker, Warrior defender) {
        // Get the WarriorGUI
        WarriorGUI gui = GetWarriorGUI(attacker);
        if (gui == null) { return; } // Should never happen
        // Keep track of him
        if (gui.isThisPlayer) { this.attacked++; }

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
            
            // Keep track of him
            if (gui.isThisPlayer) {
                this.killed++;
                this.map.RemovePlayerID(new Vector3Int(defender.GetX(), defender.GetY(), 0));
            }

            // Remove him from the field
            this.gameMaster.field.RemoveWarrior(defender);

            // Play the die animation
            gui.Die();
            // Remove him from the list
            warriorsGUI.Remove(gui);

            // Check if someone won
            wonLost.CheckCondition(gameMaster.gameState);
        }
    }

    // === ITEMS ============================
    public void OnInventoryOpen() {
        SelectItem();
    }
    private void SelectItem() {
        HideSelector();

        for (int i=0 ; i < Player.MAX_ITEMS ; i++) {
            Item item = gameMaster.gameState.GetThisPlayer().GetItem(i);

            // No warriors or already placed warrior
            if (item == null) {
                selectorPane.SetEntry(i, null);
                continue;
            }

            if (item.GetType() == typeof(Potion)) {
                selectorPane.SetEntry(i, potionSprite);
            }
            // NOTE: only potions are supported for now
        }

        selectorPane.gameObject.SetActive(true);

        this.openSelector = 2;
    }
    private void HideItemSelector() {
        this.selectedItemIndex = -1;
        this.openSelector = 0;
    }
    private void HandleItem(Vector3Int selectedTile) {
        this.map.RemovePads();

        Warrior hover = null;
        for (int i=0 ; i < this.gameMaster.gameState.GetNumberOfPlayers() ; i++) {
            hover = this.gameMaster.gameState.GetPlayer(i).GetWarriorAt(selectedTile.x, selectedTile.y);
            if (hover != null) {
                break;
            }
        }
        if (hover != null) {
            this.map.AddPad(selectedTile, true);

            // Select it
            if (Input.GetMouseButtonDown(0)) {
                Item selectedItem = this.gameMaster.gameState.GetThisPlayer().GetItem(this.selectedItemIndex);
                if (selectedItem.GetType() == typeof(Potion)) {
                    this.io.UseItem(hover, this.selectedItemIndex);
                    potionStatPanel.Hide();
                    this.gameMaster.gameState.GetThisPlayer().RemoveItem(this.selectedItemIndex);
                    HideSelector();
                }
            }
        }
        else if (Input.GetMouseButtonDown(0)) {
            HideSelector();
            this.selectedItemIndex = -1;
        }
    }

    /*
     * NOTE: should only be accessed by the Command class
     */
    public void UsePotion(Potion potion, Warrior warrior) {
        warrior.ApplyPotion(potion);
    }
}
