using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    void Start() {
        MusicManager music = FindObjectOfType<MusicManager>();
        if (!music.playing.Equals("Menu")) {
            music.Play("Menu");
        }
    }
    void Update() {}

    public void ToTeams() {
        SceneManager.LoadScene("Scenes/Team");
    }
    public void CreateRoom() {
        StartCoroutine(Server.NewGame(User.currentUser, 2, 0, (game_id) => {
            User.game_id = game_id;
            SceneManager.LoadScene("Scenes/Waiting For Players");
        }));
    }
    public void JoinRoom() {
        SceneManager.LoadScene("Scenes/Join Room");
    }
    public void ToSettings() {
        SceneManager.LoadScene("Scenes/Settings");
    }
    public void ToCredits() {
        SceneManager.LoadScene("Scenes/About");
    }
    public void Quit() {
        Application.Quit();
    }
}
