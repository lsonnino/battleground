using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WaitingForPlayers : MonoBehaviour
{
    public InputField gameID; // Where to display the game id

    void Start() {
        // Display the game id
        gameID.text = User.game_id;

        WaitTwoPlayers();
    }
    void Update() {}

    /*
        Waits for all the players to join the game.
    */
    public void WaitTwoPlayers() {
        StartCoroutine(Server.WaitForAllPlayers(User.currentUser.player_id, User.game_id, (game) => {
            // When all the players joined:
            int thisPlayer = -1;

            Player[] players = new Player[game.max_players];
            for (int i=0 ; i < game.max_players ; i++) {
                if (game.custom[i].player.Equals(User.currentUser.player_id)) {
                    thisPlayer = i;
                    players[i] = User.currentUser.ToPlayer();
                }
                else {
                    User user = new User(game.custom[i].player, "Player " + i);
                    user.warriors = new Warrior[game.custom[i].data.warriors.Length];
                    for (int j=0 ; j < user.warriors.Length ; j++) {
                        user.warriors[j] = Warrior.WARRIORS[game.custom[i].data.warriors[j]];
                    }
                    user.items = new Item[game.custom[i].data.items.Length];
                    for (int j=0 ; j < user.items.Length ; j++) {
                        user.items[j] = Item.ITEMS[game.custom[i].data.items[j]];
                    }

                    players[i] = user.ToPlayer();
                }
            }

            User.gameState = new GameState(User.game_id, players, thisPlayer);

            // Load the next scene
            SceneManager.LoadScene("Scenes/BattleScene");
        }, () => {WaitTwoPlayers();})); // If still needs to wait, recall this function
    }
}
