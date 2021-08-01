# Rulebook

## General information

The game is ment to be played by two players (more players will be supported in future releases). Before the match, each player builds his own team composed of 5 *warriors* and 5 *items* (currently, only potions are supported). A player can pick multiple times the same item or warrior.

Each warrior has three stat:

* **HP:** the amount of damage he is able to take before fainting and being removed from the game
* **Attack:** the amount of damage he deals each time he attacks
* **Walking distance (or wd):** the maximum distance he can travel each turn

Once a warrior faints, he is removed from the game.

A potion has a given duration (which can be zero for instant potions) and modifies a warrior's stat by a given amount. A few examples are:

* **Healing potion:** has no duration as it is an instant potion. Heals a warrior on the field by a given amount (note: a warrior cannot exceeds his maximum HP).
* **Strength potion:** increases the attack stat of a warrior on the field by a given amount for a given number of turns
* **Weakness potion:** decreases the attack stat of a warrior on the field by a given amount for a given number of turns

Potions can only be applied once and can be applied on any warrior on the field (even enemy's).

## How a match plays

Each player starts by selecting one warrior which is not on the field and plays it wherever he wants on the field. This action is called *summoning*. Then the players walk through four *phases*, one at a time. This is called a *turn*. After a player's turn, the next player's turn starts. The four phases are as follow and must be played in this order:

* **Move phase 1:** the player can (but does not have to) move any number of warriors he has on the field. A warrior can only move up to a Manhattan (L-1 norm) distance less or equal to his *walking distance* stat.
* **Attack phase:** every warrior on the field can (but does not have to) attack an adjacent warrior. Two warriors are considered adjacent if and only if they are right next to each other on the field (diagonals are not considered adjacent).
* **Move phase 2:** this phase is the same as the *Move Phase 1*
* **Summoning phase:** the player can (but does not have to) summon a warrior on the field.

**Note:** each warrior can only move <u>once</u> per <u>turn</u> and can only attack <u>once</u> per <u>turn</u> as well. So a warrior that moved during the *Move Phase 1* can no longer move during the *Move Phase 2* of the same turn.

The player who has no warriors alive (whether it is on the field or unsummoned) looses. By extent, the other player wins.

