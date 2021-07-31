using UnityEngine;
using UnityEngine.SceneManagement;

public class Team : MonoBehaviour
{
    public SelectorPane teamWarriors, allWarriors, teamItems, allItems;
    public WarriorStatPanel warriorStatPanel;
    public PotionStatPanel potionStatPanel;
    public PlayableCharacters playableCharacters;
    public Sprite potionSprite;

    private int teamWSelected, allWSelected, teamISelected, allISelected;

    void Start() {
        // Nothing is selected
        this.teamWSelected = -1;
        this.allWSelected  = -1;
        this.teamISelected = -1;
        this.allISelected  = -1;

        // Set callbacks
        this.teamWarriors.callback = index => SelectWarrior(true, index);
        this.allWarriors.callback  = index => SelectWarrior(false, index);
        this.teamItems.callback    = index => SelectItem(true, index);
        this.allItems.callback     = index => SelectItem(false, index);

        // Initialize selectors
        for (int i=0 ; i < Player.MAX_WARRIORS ; i++) {
            teamWarriors.SetEntry(i, playableCharacters.GetWarriorImage(Warrior.GetWarriorIndex(User.currentUser.warriors[i])));
            allWarriors.SetEntry(i, playableCharacters.GetWarriorImage(i));
        }
        for (int i=0 ; i < Player.MAX_ITEMS ; i++) {
            teamItems.SetEntry(i, potionSprite);
            allItems.SetEntry(i, potionSprite);
        }
    }

    private void UpdateStatPanel(Warrior w, Item i) {
        if (w != null) {
            potionStatPanel.Hide();
            warriorStatPanel.Show(w);
        }
        else if (i.GetType() == typeof(Potion)) {
            Potion p = (Potion) i;
            warriorStatPanel.Hide();
            potionStatPanel.Show(p);
        }
    }

    private void SelectWarrior(bool team, int index) {
        // Show selection
        UpdateStatPanel(
            team ? User.currentUser.warriors[index] : Warrior.WARRIORS[index],
            null
        );

        // Update counters
        if (team) {
            if (index == teamWSelected) {
                teamWSelected = -1;
                this.teamWarriors.ResetSelection();
                return;
            }

            teamWSelected = index;
        }
        else {
            if (index == allWSelected) {
                allWSelected = -1;
                this.allWarriors.ResetSelection();
                return;
            }

            allWSelected = index;
        }

        // Swap
        if (teamWSelected >= 0 && allWSelected >= 0) {
            User.currentUser.warriors[teamWSelected] = Warrior.WARRIORS[allWSelected];
            teamWarriors.SetEntry(teamWSelected, playableCharacters.GetWarriorImage(allWSelected));

            // Reset selection
            this.teamWSelected = -1;
            this.allWSelected  = -1;
            this.teamWarriors.ResetSelection();
            this.allWarriors.ResetSelection();

            User.currentUser.Save();
        }
    }
    private void SelectItem(bool team, int index) {
        // Show selection
        UpdateStatPanel(
            null,
            team ? User.currentUser.items[index] : Item.ITEMS[index]
        );

        // Update counters
        if (team) {
            if (index == teamISelected) {
                teamISelected = -1;
                this.teamItems.ResetSelection();
                return;
            }

            teamISelected = index;
        }
        else {
            if (index == allISelected) {
                allISelected = -1;
                this.allItems.ResetSelection();
                return;
            }

            allISelected = index;
        }

        // Swap
        if (teamISelected >= 0 && allISelected >= 0) {
            User.currentUser.items[teamISelected] = Item.ITEMS[allISelected];

            // Reset selection
            this.teamISelected = -1;
            this.allISelected  = -1;
            this.teamItems.ResetSelection();
            this.allItems.ResetSelection();

            User.currentUser.Save();
        }
    }

    public void Back() {
        SceneManager.LoadScene("Scenes/Main Menu");
    }
}
