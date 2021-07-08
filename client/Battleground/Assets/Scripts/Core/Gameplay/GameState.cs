public class GameState
{
    public enum Phase {
        MOVE_1,
        ATTACK,
        MOVE_2,
        SUMMON
    }

    private int numberOfPlayers;
    private int playerTurn;
    private Phase currentPhase;
    private Player[] players;
    private int thisPlayer;

    public GameState(Player[] players, int thisPlayer) {
        this.players = players;
        this.numberOfPlayers = players.Length;
        this.playerTurn = 0;
        this.currentPhase = Phase.MOVE_1;
        this.thisPlayer = thisPlayer;
    }

    public void NextPhase() {
        switch (this.currentPhase) {
            case Phase.MOVE_1:
                this.currentPhase = Phase.ATTACK;
                break;
            case Phase.ATTACK:
                this.currentPhase = Phase.MOVE_2;
                break;
            case Phase.MOVE_2:
                this.currentPhase = Phase.SUMMON;
                break;
            case Phase.SUMMON:
                this.currentPhase = Phase.MOVE_1;
                EndTurn();
                break;
            default:
                this.currentPhase = Phase.MOVE_1;
                break;
        }
    }
    public Phase GetPhase() {
        return this.currentPhase;
    }
    private void EndTurn() {
        this.playerTurn = (this.playerTurn + 1) % this.numberOfPlayers;
    }

    public Player GetCurrentPlayer() {
        return this.players[this.playerTurn];
    }
    public bool IsThisPlayerTurn() {
        return this.playerTurn == this.thisPlayer;
    }
}
