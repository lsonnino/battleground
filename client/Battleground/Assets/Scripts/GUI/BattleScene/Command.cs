using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Command
{
    public const int SUMMON = 0;
    public const int MOVE = 1;
    public const int ATTACK = 2;
    public const int ITEM = 3;

    public int type {get;}
    public int player {get;}
    public int warrior1 {get;}
    public int warrior2 {get;}
    public int arg1 {get;}
    public int arg2 {get;}

    /*
     * NOTE: should not be called carelessly: use the static functions here-below
     */
    public Command(GameState state, int type, Warrior w1, Warrior w2, int arg1, int arg2) {
        this.type = type;
        this.player = state.GetCurrentPlayerIndex();
        this.warrior1 = WarriorToInt(state, w1);
        this.warrior2 = WarriorToInt(state, w2);
        this.arg1 = arg1;
        this.arg2 = arg2;
    }
    private static int WarriorToInt(GameState state, Warrior warrior) {
        if (warrior == null) { return -1; }

        int player = -1;
        int index = -1;

        for (int i=0 ; i < state.GetNumberOfPlayers() ; i++) {
            Player p = state.GetPlayer(i);
            for (int j=0 ; j < Player.MAX_WARRIORS ; j++) {
                Warrior comp = p.GetWarrior(j);
                if (comp != null && comp.GetX() == warrior.GetX() && comp.GetY() == warrior.GetY()) {
                    player = i;
                    index = j;
                    return Player.MAX_WARRIORS * player + index;
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
                handler.Summon(w, arg1, arg2, this.player == state.GetCurrentPlayerIndex());
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
}
