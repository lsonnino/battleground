public class Potion : Item
{
    public enum PotionType {
        HP,
        WALKING_DISTANCE,
        ATTACK,
    }

    // Item effect
    private PotionType type {get;}
    private int length {get;}
    private int amount {get;}

    // Status
    private int lengthCountdown {get; set;} // How much time the potion has been around

    public Potion(string id, string name, PotionType type, int length, int amount)
    : base(id, name) {

        this.type = type;
        this.length = length;
        this.amount = amount;
    }
    public Potion(Potion model)
    : this(model.id, model.name, model.type, model.length, model.amount) {
        // Nothing else to be done
    }

    /*
     * Start the countdown for the potion and apply it's effect on a target
     * NOTE: this function is supposed to be called inside the Warrior class
     */
    public void Use(Warrior warrior) {
        this.lengthCountdown = length;
        Apply(warrior);
    }
    /*
     * Notify the potion that a turn has end: reduce the countdown for the potion's length
     * and if it reaches zero, remove the effect of the potion from the warrior
     * Returns true if the potion can remain, false if it's effect vanished
     * NOTE: this function is supposed to be called inside the Warrior class
     */
    public bool EndTurn(Warrior warrior) {
        this.lengthCountdown--;

        if (this.lengthCountdown <= 0) {
            End(warrior);
            return false;
        }

        return true;
    }
    private void Apply(Warrior warrior) {
        switch (type) {
            case PotionType.HP:
                warrior.Heal(this.amount);
                break;
            case PotionType.WALKING_DISTANCE:
                warrior.walkingDistanceModifier += this.amount;
                break;
            case PotionType.ATTACK:
                warrior.attackModifier += this.amount;
                break;
            default:
                // Do nothing
                break;
        }
    }
    private void End(Warrior warrior) {
        switch (type) {
            case PotionType.HP:
                // Do nothing: one time potion
                break;
            case PotionType.WALKING_DISTANCE:
                warrior.walkingDistanceModifier -= this.amount;
                break;
            case PotionType.ATTACK:
                warrior.attackModifier -= this.amount;
                break;
            default:
                // Do nothing
                break;
        }
    }
}
