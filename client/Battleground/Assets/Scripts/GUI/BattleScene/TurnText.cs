using UnityEngine;
using UnityEngine.UI;

public class TurnText : MonoBehaviour
{
    private Text text;

    void Start()
    {
        text = this.GetComponent<Text>();
    }
    void Update() {}

    public void UpdateText(bool thisPlayerTurn) {
        text.text = thisPlayerTurn ? "Your Turn" : "Opponent's Turn";
    }
}
