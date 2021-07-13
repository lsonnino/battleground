public class GameState
{
    public enum Phase {
        NONE, // Not a valid game phase, just to ease programming
        MOVE_1,
        ATTACK,
        MOVE_2,
        SUMMON
    }

    private string gameID;
    private int numberOfPlayers;
    private int playerTurn;
    private Phase currentPhase;
    private Player[] players;
    private int thisPlayer;
    private int start;

    public GameState(string gameID, Player[] players, int thisPlayer) {
        this.gameID = gameID;
        this.players = players;
        this.numberOfPlayers = players.Length;
        this.playerTurn = 0;
        this.currentPhase = Phase.SUMMON; // First turn of each player is just a summon
        this.thisPlayer = thisPlayer;
        this.start = numberOfPlayers - 1;
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
                if (this.start == 0) {
                    this.currentPhase = Phase.MOVE_1;
                }
                else { // First turn of each player is just a summon
                    this.start--;
                }
                EndTurn();
                break;
            default:
                this.currentPhase = Phase.MOVE_1;
                break;
        }
    }
    public int GetPlayerTurn() {
        return this.playerTurn;
    }
    public Phase GetPhase() {
        return this.currentPhase;
    }
    private void EndTurn() {
        for (int j=0 ; j < Player.MAX_WARRIORS ; j++) {
            Warrior w = this.players[this.playerTurn].GetWarrior(j);
            if (w != null) {
                w.EndTurn();
            }
        }

        this.playerTurn = (this.playerTurn + 1) % this.numberOfPlayers;
    }

    public int GetNumberOfPlayers() {
        return this.numberOfPlayers;
    }
    public Player GetPlayer(int index) {
        return this.players[index];
    }
    public Player GetCurrentPlayer() {
        return this.players[this.playerTurn];
    }
    public int GetCurrentPlayerIndex() {
        return this.playerTurn;
    }
    public bool IsThisPlayerTurn() {
        return this.playerTurn == this.thisPlayer;
    }

    public string GetGameID() {
        return this.gameID;
    }
}
