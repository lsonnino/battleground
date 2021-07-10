using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class Server
{
    public static readonly string SERVER = "https://bg.alfcorp.org";

    /*
        =========   HANDLE PLAYERS   =========
    */
    /*
        Create a new player
    */
    public static IEnumerator CreatePlayer(string username, System.Action<User> callback) {
        NewPlayerSender sender = new NewPlayerSender();
        sender.name = username;

        yield return Send("new_player", JsonUtility.ToJson(sender), (json) => {
            NewPlayerResponse res = JsonUtility.FromJson<NewPlayerResponse>(json);
            User user = new User(res.id, username);
            callback(user);
        });
    }
    public static IEnumerator GetPlayers(System.Action<User[]> callback) {
        yield return Send("get_players", JsonUtility.ToJson(new EmptySender()), (json) => {
            GetPlayersResponse res = JsonUtility.FromJson<GetPlayersResponse>(json);
            callback(res.players);
        });
    }
    /*
        Remove a player
    */
    public static IEnumerator RemovePlayer(string id, System.Action callback) {
        RemovePlayerSender sender = new RemovePlayerSender();
        sender.id = id;

        yield return Send("remove_player", JsonUtility.ToJson(sender), (json) => {
            callback();
        });
    }

    /*
        =========   HANDLE GAMES   =========
    */
    public static IEnumerator NewGame(string playerID, int maxPlayers, int team, System.Action<string> callback) {
        NewGameSender sender = new NewGameSender();
        sender.player_id = playerID;
        sender.max_players = maxPlayers;
        sender.team = team;

        yield return Send("new_game", JsonUtility.ToJson(sender), (json) => {
            NewGameResponse res = JsonUtility.FromJson<NewGameResponse>(json);
            callback(res.id);
        });
    }
    public static IEnumerator JoinGame(string playerID, string gameID, int team, System.Action callback) {
        JoinGameSender sender = new JoinGameSender();
        sender.player_id = playerID;
        sender.game_id = gameID;
        sender.team = team;

        yield return Send("join_game", JsonUtility.ToJson(sender), (json) => {
            callback();
        });
    }
    public static IEnumerator GetGames(string playerID, System.Action<GameWrapper[]> callback) {
        GetGamesSender sender = new GetGamesSender();
        sender.player_id = playerID;

        yield return Send("get_games", JsonUtility.ToJson(sender), (json) => {
            GetGamesResponse res = JsonUtility.FromJson<GetGamesResponse>(json);
            callback(res.games);
        });
    }
    public static IEnumerator RemoveGame(string gameID, System.Action callback) {
        JoinGameSender sender = new JoinGameSender();
        sender.game_id = gameID;

        yield return Send("remove_game", JsonUtility.ToJson(sender), (json) => {
            callback();
        });
    }

    /*
        =========   HANDLE COMMANDS   =========
    */
    public static IEnumerator ReceiveCommands(GameState state, int playerIndex, int seq, System.Action<CommandWrapper[]> callback) {
        GetStateQuerry querry = new GetStateQuerry(state, playerIndex, seq);

        yield return Send("get_state", JsonUtility.ToJson(querry), (json) => {
            GetStateResponse res = JsonUtility.FromJson<GetStateResponse>(json);
            if (res != null) {
                callback(res.commands);
            }
            else {
                callback(null);
            }
        });
    }
    public static IEnumerator SendCommand(GameState state, CommandWrapper command, System.Action<int> callback) {
        UpdateQuerry querry = new UpdateQuerry(state, command);

        yield return Send("update_state", JsonUtility.ToJson(querry), (json) => {
            UpdateResponse res = JsonUtility.FromJson<UpdateResponse>(json);
            if (res != null) {
                callback(res.next_seq);
            }
            else {
                callback(-1);
            }
        });
    }
    /*
        Send a JSON objct to a specific page using a post request
    */
    private static IEnumerator Send(string page, string json, System.Action<string> callback) {
        UnityWebRequest request = new UnityWebRequest(SERVER + "/" + page, "POST");
        request.SetRequestHeader("Content-Type", "application/json");

        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
        request.uploadHandler.contentType = "application/json";

        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError) {
            Debug.Log("Error sending the form to the page " + page + ":" + request.error);
        }
        else {
            callback(request.downloadHandler.text);
        }
    }

    [System.Serializable]
    public class CommandWrapper {
        public int seq;
        public Command data;

        public CommandWrapper(Command command, int seq) {
            this.data = command;
            this.seq = seq;
        }
    }
    [System.Serializable]
    public class GamePlayerWrapper {
        public string player_id;
        public int team;
    }
    [System.Serializable]
    public class GameWrapper {
        public string id;
        public int max_players;
        public GamePlayerWrapper[] teams;
    }

    [System.Serializable]
    public class EmptySender {}
    [System.Serializable]
    public class NewPlayerSender {
        public string name;
    }
    [System.Serializable]
    public class NewPlayerResponse {
        public string id = "";
    }
    [System.Serializable]
    public class GetPlayersResponse {
        public User[] players;
    }
    [System.Serializable]
    public class RemovePlayerSender {
        public string id;
    }

    [System.Serializable]
    public class NewGameSender {
        public string player_id;
        public int max_players;
        public int team;
        // TODO
    }
    [System.Serializable]
    public class NewGameResponse {
        public string id;
    }
    [System.Serializable]
    public class JoinGameSender {
        public string game_id;
        public string player_id;
        public int team;
        // TODO
    }
    [System.Serializable]
    public class GetGamesSender {
        public string player_id;
    }
    [System.Serializable]
    public class GetGamesResponse {
        public GameWrapper[] games;
    }
    [System.Serializable]
    public class RemoveGameSender {
        public string id;
    }

    [System.Serializable]
    public class UpdateQuerry {
        public string game_id;
        public string player_id;
        public CommandWrapper[] commands;

        public UpdateQuerry(GameState state, CommandWrapper command)
        : this(state, new CommandWrapper[]{command}) {}
        public UpdateQuerry(GameState state, CommandWrapper[] commands) {
            this.game_id = state.GetGameID();
            this.player_id = state.GetCurrentPlayer().GetID();
            this.commands = commands;
        }
    }
    [System.Serializable]
    public class UpdateResponse {
        public int next_seq;
    }
    [System.Serializable]
    public class GetStateQuerry {
        public string game_id;
        public string player_id;
        public int seq;

        public GetStateQuerry(GameState state, int playerIndex, int seq) {
            this.game_id = state.GetGameID();
            this.player_id = state.GetPlayer(playerIndex).GetID();
            this.seq = seq;
        }
    }
    [System.Serializable]
    public class GetStateResponse {
        public CommandWrapper[] commands;
    }
}
