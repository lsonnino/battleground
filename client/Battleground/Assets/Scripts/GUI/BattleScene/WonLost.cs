using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WonLost : MonoBehaviour
{
    public Text wonText;
    public Animator animator;
    public Canvas canvas;

    private bool triggered;

    void Start() {
        triggered = false;
        canvas.gameObject.SetActive(false);
    }

    void Update() {
        if (triggered && wonText.color.a == 1 && Input.anyKey) {
            // TODO: remove the game
            // Return to main menu
            SceneManager.LoadScene("Scenes/Main Menu");
        }
    }

    public void CheckCondition(GameState state) {
        for (int i=0 ; i < state.GetNumberOfPlayers() ; i++) {
            Player p = state.GetPlayer(i);
            bool foundAlive = false;

            for (int j=0 ; j < Player.MAX_WARRIORS ; j++) {
                if (p.GetWarrior(j).IsAlive()) {
                    foundAlive = true;
                    break;
                }
            }

            // TODO: only supports 1vs1
            if (!foundAlive) {
                this.Trigger(i != state.GetThisPlayerIndex());
                return;
            }
        }
    }

    void Trigger(bool won) {
        if (won) {
            wonText.text = "YOU WON";
        }
        else {
            wonText.text = "YOU LOST";
        }

        canvas.gameObject.SetActive(true);
        animator.SetTrigger("Play");

        triggered = true;
    }
}
