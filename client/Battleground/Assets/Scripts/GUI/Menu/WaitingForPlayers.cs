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
                        user.warriors[j] = new Warrior(Warrior.WARRIORS[game.custom[i].data.warriors[j]]);
                    }
                    user.items = new Item[game.custom[i].data.items.Length];
                    for (int j=0 ; j < user.items.Length ; j++) {
                        Item model = Item.ITEMS[game.custom[i].data.items[j]];
                        if (model.GetType() == typeof(Potion)) {
                            user.items[j] = new Potion((Potion) model);
                        }
                        else {
                            // TODO: only potions supported for now
                            Debug.Log("[WAIT FOR PLAYERS] Only potions supported for now");
                        }
                    }

                    players[i] = user.ToPlayer();
                }
            }

            User.gameState = new GameState(User.game_id, players, thisPlayer);

            // Destroy background
            FindObjectOfType<MenuBackground>().Remove();
            // Load the next scene
            SceneManager.LoadScene("Scenes/BattleScene");
        }, () => {WaitTwoPlayers();})); // If still needs to wait, recall this function
    }

    public void Back() {
        // TODO: remove player from the game
        SceneManager.LoadScene("Scenes/Main Menu");
    }
}
