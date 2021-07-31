using System.Collections;

[System.Serializable]
public class Warrior
{
    // Available Warriors
    public static readonly Warrior DINO = new Warrior("0", "Dino", 5, 2, 5);
    public static readonly Warrior GLADIATOR = new Warrior("1", "Gladiator", 8, 2, 3);
    public static readonly Warrior KNIGHT = new Warrior("2", "Knight", 5, 2, 4);
    public static readonly Warrior NINJA = new Warrior("3", "Ninja", 5, 5, 2);
    public static readonly Warrior PUMPKIN = new Warrior("4", "Pumpkin", 5, 3, 3);
    public static readonly Warrior F_ZOMBIE = new Warrior("5", "Zombie F.", 2, 1, 4);
    public static readonly Warrior M_ZOMBIE = new Warrior("6", "Zombie M.", 2, 1, 4);

    public static readonly Warrior[] WARRIORS = new Warrior[] {
        // DINO,
        GLADIATOR,
        KNIGHT,
        NINJA,
        PUMPKIN,
        F_ZOMBIE,
        M_ZOMBIE
    };


    // Warrior ID
    public string id {get;}
    public string name {get;}

    // Initial stat
    private int maxHP {get;}
    private int initWalkingDistance {get;}
    private int initAttack {get;}

    // Current stat and modifiers
    private bool placed;
    private int x;
    private int y;
    private int hp;
    public int walkingDistanceModifier {get; set;}
    public int attackModifier {get; set;}
    private bool alive;
    private ArrayList potions;
    public bool moved {get; set;}
    public bool attacked {get; set;}

    public Warrior(string id, string name, int maxHP, int walkingDistance, int attack) {
        this.id = id;
        this.name = name;

        this.maxHP = maxHP;
        this.initWalkingDistance = walkingDistance;
        this.initAttack = attack;

        Initialize();
    }
    public Warrior(Warrior model)
    : this(model.id, model.name, model.maxHP, model.initWalkingDistance, model.initAttack) {
        // Nothing else to be done
    }
    private void Initialize() {
        this.hp = this.maxHP;
        this.walkingDistanceModifier = 0;
        this.attackModifier = 0;
        this.alive = true;

        this.potions = new ArrayList();

        this.placed = false;
        this.x = 0;
        this.y = 0;
        this.moved = false;
        this.attacked = false;
    }

    public int GetMaxHP() {
        return this.maxHP;
    }
    public int GetHP() {
        return this.hp;
    }
    public int GetAttack() {
        int attack = this.initAttack + this.attackModifier;
        if (attack < 0) {
            return 0;
        }

        return attack;
    }
    public int GetWalkingDistance() {
        int wd = this.initWalkingDistance + this.walkingDistanceModifier;
        if (wd < 0) {
            return 0;
        }

        return wd;
    }

    public bool IsAlive() {
        return this.alive;
    }
    public void Heal(int amount) {
        this.hp += amount;
        if (this.hp > this.maxHP) {
            this.hp = this.maxHP;
        }
    }
    public void Damage(int amount) {
        this.hp -= amount;
        if (this.hp <= 0) {
            this.hp = 0;
            this.alive = false;
        }
    }
    public void ApplyPotion(Potion potion) {
        this.potions.Add(potion);
        potion.Use(this);
    }

    /*
     * NOTE: should only be called at the end of the player's turn:
     * this is done in GameState: EndTurn()
     */
    public void EndTurn() {
        this.moved = false;
        this.attacked = false;

        for (int i=0 ; i < this.potions.Count ; i++) {
            Potion potion = (Potion) this.potions[i];

            if (!potion.EndTurn(this)) {
                // The effect of the potion vanished
                this.potions.Remove(potion);
                i--; // Do not skip the next one
            }
        }
    }

    public bool IsPlaced() {
        return this.placed;
    }
    public int GetX() {
        return this.x;
    }
    public int GetY() {
        return this.y;
    }
    /*
     * NOTE: this function should not be called outside Field. Indeed, the field's
     * state must be updated as well when a warrior is placed or moves
     */
    public void Place(int x, int y) {
        this.placed = true;
        this.x = x;
        this.y = y;
    }

    public static int GetWarriorIndex(Warrior warrior) {
        for (int i=0 ; i < WARRIORS.Length ; i++) {
            if (WARRIORS[i].id.Equals(warrior.id)) {
                return i;
            }
        }

        return -1;
    }
}
