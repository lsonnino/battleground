[System.Serializable]
﻿public class Item
{
    // Available Potions
    public static readonly Potion HEAL = new Potion("0", "Heal Potion", Potion.PotionType.HP, 0, 5);
    public static readonly Potion STRENGTH = new Potion("1", "Strength Potion", Potion.PotionType.ATTACK, 3, 2);
    public static readonly Potion WEAKNESS = new Potion("2", "Weakness Potion", Potion.PotionType.ATTACK, 3, -2);
    public static readonly Potion RUNNER = new Potion("3", "Runner Potion", Potion.PotionType.WALKING_DISTANCE, 3, 2);
    public static readonly Potion AGING = new Potion("4", "Aging Potion", Potion.PotionType.WALKING_DISTANCE, 3, -2);

    // Available Items
    public static readonly Item[] ITEMS = new Item[]{HEAL, STRENGTH, WEAKNESS, RUNNER, AGING};

    // Item ID
    public string id {get;}
    public string name {get;}

    public Item(string id, string name) {
        this.id = id;
        this.name = name;
    }

    public static int GetItemIndex(Item item) {
        for (int i=0 ; i < ITEMS.Length ; i++) {
            if (item.id.Equals(ITEMS[i].id)) {
                return i;
            }
        }

        return -1;
    }
}
