[System.Serializable]
﻿public class Item
{
    // Item ID
    public string id {get;}
    public string name {get;}

    public Item(string id, string name) {
        this.id = id;
        this.name = name;
    }
}
