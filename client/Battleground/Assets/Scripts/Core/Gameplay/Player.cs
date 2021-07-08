public class Player
{
    public const int MAX_WARRIORS = 5;
    public const int MAX_ITEMS = 5;

    private string id {get;}
    private string name {get;}
    private Warrior[] warriors {get; set;}
    private Item[] items {get; set;}

    public Player(string id, string name, Warrior[] warriors, Item[] items) {
        this.id = id;
        this.name = name;

        this.warriors = warriors;
        this.items = items;
    }

    public void RemoveWarrior(int index) {
        this.warriors[index] = null;
    }
    public void RemoveItem(int index) {
        this.items[index] = null;
    }
}
