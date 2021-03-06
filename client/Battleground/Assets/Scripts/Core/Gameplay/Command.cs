using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Command
{
    public const int SUMMON = 0;
    public const int MOVE = 1;
    public const int ATTACK = 2;
    public const int ITEM = 3;
    public const int END_PHASE = 4;

    public int type;
    public int player;
    public int warrior1;
    public int warrior2;
    public int arg1;
    public int arg2;

    /*
     * NOTE: should not be called carelessly: use the static functions here-below
     */
    public Command(GameState state, int type, Warrior w1, Warrior w2, int arg1, int arg2) {
        this.type = type;
        this.player = state.GetThisPlayerIndex();
        this.warrior1 = WarriorToInt(state, w1);
        this.warrior2 = WarriorToInt(state, w2);
        this.arg1 = arg1;
        this.arg2 = arg2;
    }
    private static int WarriorToInt(GameState state, Warrior warrior) {
        if (warrior == null) { return -1; }

        if (warrior.IsPlaced()) {
            for (int i=0 ; i < state.GetNumberOfPlayers() ; i++) {
                Player p = state.GetPlayer(i);

                for (int j=0 ; j < Player.MAX_WARRIORS ; j++) {
                    Warrior comp = p.GetWarrior(j);
                    if (comp != null && comp.id.Equals(warrior.id) && comp.GetX() == warrior.GetX() && comp.GetY() == warrior.GetY()) {
                        return Player.MAX_WARRIORS * i + j;
                    }
                }
            }
        }
        else {
            int i = state.GetThisPlayerIndex();
            Player p = state.GetPlayer(i);

            for (int j=0 ; j < Player.MAX_WARRIORS ; j++) {
                Warrior comp = p.GetWarrior(j);
                if (comp != null && comp.id.Equals(warrior.id) && comp.GetX() == warrior.GetX() && comp.GetY() == warrior.GetY()) {
                    return Player.MAX_WARRIORS * i + j;
                }
            }
        }

        return -1;
    }
    private static Warrior IntToWarrior(GameState state, int w) {
        if (w < 0) { return null; }

        Player p = state.GetPlayer((int) (w / Player.MAX_WARRIORS));
        if (p == null) { return null; }

        return p.GetWarrior(w % Player.MAX_WARRIORS);
    }

    public void Execute(EventsHandler handler) {
        GameState state = handler.gameMaster.gameState;
        Warrior w = IntToWarrior(state, this.warrior1);

        switch (type) {
            case SUMMON:
                handler.Summon(w, arg1, arg2, this.player == state.GetThisPlayerIndex());
                break;
            case MOVE:
                handler.MoveWarrior(w, arg1, arg2);
                break;
            case ATTACK:
                handler.Attack(w, IntToWarrior(state, this.warrior2));
                break;
            case ITEM:
                Item selectedItem = state.GetPlayer(this.player).GetItem(this.arg1);
                if (selectedItem.GetType() == typeof(Potion)) {
                    handler.UsePotion((Potion) selectedItem, w);
                }
                else {
                    // Do nothing: only potions are supporter for now
                }
                break;
            case END_PHASE:
                handler.gameMaster.NextPhase();
                break;
            default:
                break;
        }
    }

    public static Command Summon(GameState state, Warrior warrior, int toX, int toY) {
        return new Command(state, SUMMON, warrior, null, toX, toY);
    }
    public static Command Move(GameState state, Warrior warrior, int toX, int toY) {
        return new Command(state, MOVE, warrior, null, toX, toY);
    }
    public static Command Attack(GameState state, Warrior attacker, Warrior defender) {
        return new Command(state, ATTACK, attacker, defender, -1, -1);
    }
    public static Command UseItem(GameState state, Warrior warrior, int index) {
        return new Command(state, ITEM, warrior, null, index, -1);
    }
    public static Command NextPhase(GameState state) {
        return new Command(state, END_PHASE, null, null, -1, -1);
    }
}
