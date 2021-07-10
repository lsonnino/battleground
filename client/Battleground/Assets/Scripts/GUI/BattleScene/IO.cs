using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IO : MonoBehaviour
{
    public EventsHandler handler;

    void Start() {}
    void Update() {}

    public void Summon(Warrior warrior, int toX, int toY) {
        Command command = Command.Summon(this.handler.gameMaster.gameState, warrior, toX, toY);
        command.Execute(handler);
    }
    public void Move(Warrior warrior, int toX, int toY) {
        Command command = Command.Move(this.handler.gameMaster.gameState, warrior, toX, toY);
        command.Execute(handler);
    }
    public void Attack(Warrior attacker, Warrior defender) {
        Command command = Command.Attack(this.handler.gameMaster.gameState, attacker, defender);
        command.Execute(handler);
    }
    public void UseItem(Warrior warrior, int index) {
        Command command = Command.UseItem(this.handler.gameMaster.gameState, warrior, index);
        command.Execute(handler);
    }
}
