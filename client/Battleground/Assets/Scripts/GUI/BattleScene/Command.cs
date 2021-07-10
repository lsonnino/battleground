using System.Collections;
using System.Collections.Generic;

public class Command
{
    public const int SUMMON = 0;
    public const int MOVE = 1;
    public const int ATTACK = 2;
    public const int ITEM = 3;

    public int type {get;}
    public int player {get;}
    public Warrior warrior1 {get;}
    public Warrior warrior2 {get;}
    public int arg1 {get;}
    public int arg2 {get;}

    /*
     * NOTE: should not be called carelessly: use the static functions here-below
     */
    public Command(GameState state, int type, Warrior w1, Warrior w2, int arg1, int arg2) {
        this.type = type;
        this.player = state.GetCurrentPlayerIndex();
        this.warrior1 = w1;
        this.warrior2 = w2;
        this.arg1 = arg1;
        this.arg2 = arg2;
    }

    public void Execute(EventsHandler handler) {
        GameState state = handler.gameMaster.gameState;

        switch (type) {
            case SUMMON:
                handler.Summon(this.warrior1, arg1, arg2, this.player == state.GetCurrentPlayerIndex());
                break;
            case MOVE:
                handler.MoveWarrior(this.warrior1, arg1, arg2);
                break;
            case ATTACK:
                handler.Attack(this.warrior1, this.warrior2);
                break;
            case ITEM:
                Item selectedItem = state.GetPlayer(this.player).GetItem(this.arg1);
                if (selectedItem.GetType() == typeof(Potion)) {
                    handler.UsePotion(this.arg1, this.warrior1);
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
