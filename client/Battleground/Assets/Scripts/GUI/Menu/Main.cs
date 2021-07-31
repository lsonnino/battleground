using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    public Image image; // The ALFCorp logo

    private float step = 1.2f; // The biggest, the faster the animation
    private float alpha = 0.0f; // The current alpha channel of the image
    private bool foundUser = false;

    void Start() {
        if (User.Exists()) {
            User.currentUser = User.Load();
            FindObjectOfType<MusicManager>().SetVolume(User.currentUser.musicVolume);
            FindObjectOfType<AudioManager>().SetVolume(User.currentUser.soundVolume);

            foundUser = true;
        }
        FindObjectOfType<MusicManager>().Play("Menu");

        StartCoroutine(GetUser());
    }
    void Update() {
        // Animate the image's alpha channel

        this.image.color = new Color(1, 1, 1, alpha);

        if (alpha >= 1 && step > 0) { // Finished increasing the alpha channel
            // Start decreasing
            step = -step;
            alpha = 1f;
        }
        else if (alpha <= 0 && step < 0) { // Finished decreasing the alpha channel
            return;
        }
        this.alpha += this.step * Time.deltaTime;
    }

    IEnumerator GetUser() {
        // Wait for animation to finish
        while (alpha > 0 || step > 0) {
            yield return null;
        }

        // If the user exists
        if (foundUser) {
            // Load the first menu
            SceneManager.LoadScene("Scenes/Main Menu");
        }
        else {
            // Create a new user
            SceneManager.LoadScene("Scenes/New User");
        }
    }
}
