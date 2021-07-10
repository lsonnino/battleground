using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/*
    Join a room
*/
public class JoinRoom : MonoBehaviour
{
    public InputField inputField; // Where the user puts the game ID

    void Start() {}
    void Update() {}

    public void OnClick() {
        if (inputField.text == "") { // Input Field is empty
            // Do nothing else
            return;
        }
        else { // The inputField contains a game ID
            // Join the game
            User.game_id = inputField.text;
            StartCoroutine(Server.JoinGame(User.currentUser, User.game_id, 1, () => {
                SceneManager.LoadScene("Scenes/Waiting For Players");
            }));
        }
    }
}
