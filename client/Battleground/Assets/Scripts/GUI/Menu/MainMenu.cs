﻿using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    void Start() {}
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
}