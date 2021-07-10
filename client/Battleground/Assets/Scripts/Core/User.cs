using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class User {
    // Where to save the user
    public static readonly string SAVE_PATH = Path.Combine(Application.persistentDataPath, "bg.save");
    public static User currentUser; // the current user instance
    public static string game_id = ""; // The current game id the user wants to play

    public static GameState gameState {get; set;}

    public string player_id {get;}
    public string username {get;}
    public Warrior[] warriors {get; set;}
    public Item[] items {get; set;}
    public float musicVolume {get; set;}
    public float soundVolume {get; set;}

    public User(string player_id, string username) {
        this.player_id = player_id;
        this.username = username;
        this.warriors = new Warrior[Player.MAX_WARRIORS];
        this.items = new Item[Player.MAX_ITEMS];
        this.musicVolume = 1f;
        this.soundVolume = 1f;

        // TODO: remove this and implement a Team builder
        if (this.username.Equals("Lor")) {
            this.warriors = new Warrior[]{
                new Warrior(Warrior.GLADIATOR),
                new Warrior(Warrior.KNIGHT),
                new Warrior(Warrior.NINJA),
                new Warrior(Warrior.PUMPKIN),
                new Warrior(Warrior.M_ZOMBIE)
            };
            this.items = new Item[]{
                new Potion(Potion.HEAL),
                new Potion(Potion.STRENGTH),
                new Potion(Potion.WEAKNESS),
                new Potion(Potion.RUNNER),
                new Potion(Potion.AGING)
            };
        }
    }

    public Player ToPlayer() {
        return new Player(this.player_id, this.username, this.warriors, this.items);
    }

    /*
        Save the user into a file for the next time the game starts
    */
    public void Save() {
        BinaryFormatter bf = new BinaryFormatter();

        using (FileStream stream = File.Create(SAVE_PATH)) {
            bf.Serialize(stream, this);
        }
    }

    /*
        Get whether or not an user already exists
    */
    public static bool Exists() {
        return File.Exists(SAVE_PATH);
    }
    /*
        Load the user from a File and return's it.
        The file one must exists.
    */
    public static User Load() {
        User user = null;

        BinaryFormatter bf = new BinaryFormatter();
        using (FileStream stream = new FileStream(SAVE_PATH, FileMode.Open, FileAccess.Read)) {
            user = bf.Deserialize(stream) as User;
        }

        return user;
    }
}
