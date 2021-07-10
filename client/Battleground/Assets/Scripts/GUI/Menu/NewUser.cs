using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NewUser : MonoBehaviour
{
    public Text username;

    void Start() {}
    void Update() {}

    public void Submit() {
        if (username.text == null || username.text.Equals("")) {
            return;
        }

        StartCoroutine(Server.CreatePlayer(username.text, (user) => {
            User.currentUser = user;
            User.currentUser.Save();

            SceneManager.LoadScene("Scenes/Main Menu");
        }));
    }
}
