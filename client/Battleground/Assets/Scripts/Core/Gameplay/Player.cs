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

    public Warrior GetWarrior(int index) {
        return this.warriors[index];
    }
    public Warrior GetWarriorAt(int x, int y) {
        for (int i=0 ; i < warriors.Length ; i++) {
            if (warriors[i] != null && warriors[i].IsPlaced() && warriors[i].IsAlive() &&
                warriors[i].GetX() == x && warriors[i].GetY() == y) {
                return warriors[i];
            }
        }

        return null;
    }

    public string GetID() {
        return this.id;
    }

    public Item GetItem(int index) {
        return this.items[index];
    }
    public void RemoveItem(int index) {
        this.items[index] = null;
    }
}
