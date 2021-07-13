using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IO : MonoBehaviour
{
    public EventsHandler handler;

    private List<Command> sendingQueue;
    private int[] seq;
    private int lastReceivedPlayer;

    void Start() {
        sendingQueue = new List<Command>();
        seq = new int[handler.gameMaster.gameState.GetNumberOfPlayers()];
        System.Array.Clear(seq, 0, seq.Length);

        StartCoroutine(Receive());
        StartCoroutine(Send());
    }
    void Update() {}
    public GameState GetGameState() {
        return handler.gameMaster.gameState;
    }

    public IEnumerator Receive() {
        this.lastReceivedPlayer++;
        if (this.lastReceivedPlayer == GetGameState().GetCurrentPlayerIndex()) {
            this.lastReceivedPlayer++;
        }

        if (this.lastReceivedPlayer == GetGameState().GetNumberOfPlayers()) {
            this.lastReceivedPlayer = 0;
        }

        yield return Server.ReceiveCommands(GetGameState(), this.lastReceivedPlayer, seq[this.lastReceivedPlayer], (commands) => {
            if (commands != null) {
                for (int i=0 ; i < commands.Length ; i++) {
                    seq[this.lastReceivedPlayer] = commands[i].seq;
                    commands[i].data.Execute(this.handler);
                }
            }

            StartCoroutine(Receive());
        });
    }
    public IEnumerator Send() {
        if (sendingQueue.Count == 0) {
            yield return null;
            StartCoroutine(Send());
        }
        else {
            Command command = sendingQueue[0];
            Server.CommandWrapper wrapper = new Server.CommandWrapper(command, seq[GetGameState().GetCurrentPlayerIndex()]);
            yield return Server.SendCommand(GetGameState(), wrapper, (next_seq) => {
                if (next_seq > seq[GetGameState().GetCurrentPlayerIndex()]) {
                    seq[GetGameState().GetCurrentPlayerIndex()] = next_seq;
                    sendingQueue.Remove(command);
                }

                StartCoroutine(Send());
            });
        }
    }
    private void AddToSendingQueue(Command command) {
        sendingQueue.Add(command);
    }

    /*
     * NOTE: these are the functions that SHOULD be used whenever an action is taken
     * on this player's side
     */
    public void Summon(Warrior warrior, int toX, int toY) {
        Command command = Command.Summon(GetGameState(), warrior, toX, toY);
        command.Execute(handler);
        this.AddToSendingQueue(command);
    }
    public void Move(Warrior warrior, int toX, int toY) {
        Command command = Command.Move(GetGameState(), warrior, toX, toY);
        command.Execute(handler);
        this.AddToSendingQueue(command);
    }
    public void Attack(Warrior attacker, Warrior defender) {
        Command command = Command.Attack(GetGameState(), attacker, defender);
        command.Execute(handler);
        this.AddToSendingQueue(command);
    }
    public void UseItem(Warrior warrior, int index) {
        Command command = Command.UseItem(GetGameState(), warrior, index);
        command.Execute(handler);
        this.AddToSendingQueue(command);
    }
    public void NextPhase() {
        Command command = Command.NextPhase(GetGameState());
        command.Execute(handler);
        this.AddToSendingQueue(command);
    }
}
