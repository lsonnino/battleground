using System.Collections;

public class Warrior
{
    // Available Warriors
    public static readonly Warrior DINO = new Warrior("0", "Dino", 5, 3, 2);

    public static readonly Warrior[] WARRIORS = new Warrior[]{DINO};


    // Warrior ID
    public string id {get;}
    public string name {get;}

    // Initial stat
    private int maxHP {get;}
    private int initWalkingDistance {get;}
    private int initAttack {get;}

    // Current stat and modifiers
    public int x {get; set;}
    public int y {get; set;}
    private int hp {get; set;}
    public int walkingDistanceModifier {get; set;}
    public int attackModifier {get; set;}
    private bool alive {get; set;}
    private ArrayList potions;

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

        this.x = -1;
        this.y = -1;
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

    public void Heal(int amount) {
        this.hp += amount;
        if (this.hp > this.maxHP) {
            this.hp = this.maxHP;
        }
    }
    public void Damage(int amount) {
        this.hp -= amount;
        if (this.hp < 0) {
            this.hp = 0;
            this.alive = false;
        }
    }
    public void ApplyPotion(Potion potion) {
        this.potions.Add(potion);
        potion.Use(this);
    }

    public void EndTurn() {
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
        return this.x >= 0 && this.y >= 0;
    }
}
