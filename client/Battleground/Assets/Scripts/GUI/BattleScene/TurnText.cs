using UnityEngine;
using UnityEngine.UI;

public class TurnText : MonoBehaviour
{
    public Text text;

    void Start() {}
    void Update() {}

    public void UpdateText(bool thisPlayerTurn) {
        text.text = thisPlayerTurn ? "Your Turn" : "Opponent's Turn";
    }
}
