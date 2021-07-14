using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorGUI : MonoBehaviour
{
    public float movementSpeed = 1f;
    public Warrior warrior;
    public bool isThisPlayer;
    private Animator animator;

    private float x, y;
    private List<PathFind.Point> path;
    private int indexInPath;

    void Start() {
        this.animator = this.GetComponent<Animator>();
        this.indexInPath = -1;
    }
    void Update() {
        // Check if there is the need to move
        if (indexInPath < 0) { return; }

        // Check if movement has finished
        if (indexInPath == path.Count) {
            this.animator.SetBool("Move", false);
            indexInPath = -1;
            path = null;

            this.GetComponent<SpriteRenderer>().flipX = !isThisPlayer;

            return;
        }

        PathFind.Point target = path[indexInPath];

        // Go to next step in animation
        if (this.x == target.x && this.y == target.y) {
            indexInPath++;
        }

        // Play current step
        float addedX = ComputeAdded(this.x, target.x, movementSpeed * Time.deltaTime);
        float addedY = ComputeAdded(this.y, target.y, movementSpeed * Time.deltaTime);

        if (addedX < 0) {
            this.GetComponent<SpriteRenderer>().flipX = true;
        }
        else {
            this.GetComponent<SpriteRenderer>().flipX = false;
        }

        this.x += addedX; this.y += addedY;
        this.transform.parent.localPosition += new Vector3(addedX, addedY, addedY / 100f);
    }
    private float ComputeAdded(float cur, float targ, float speed) {
        if (cur == targ) {
            return 0f;
        }
        else if (cur < targ) {
            return Mathf.Min(targ - cur, speed);
        }
        else {
            return Mathf.Max(targ - cur, -speed);
        }
    }

    public void Init(Warrior warrior, bool isThisPlayer) {
        this.warrior = warrior;
        this.isThisPlayer = isThisPlayer;

        this.GetComponent<SpriteRenderer>().flipX = !isThisPlayer;
    }

    public void Move(List<PathFind.Point> path) {
        this.path = path;
        this.x = warrior.GetX();
        this.y = warrior.GetY();
        this.indexInPath = 0;

        this.animator.SetBool("Move", true);
    }

    public void Attack() {
        this.animator.SetTrigger("Attack");
    }

    public void Die() {
        /*
         * NOTE: at the end of the animation, the SpriteRenderer is destroyed but
         * not the Empty who is his parent object. This is because of the script
         * Scripts/GUI/BattleScene/AutodestroyAnimation.cs which destroys the object
         * which has the Animator and not his parent. Therefore, at the end of
         * the game there will be an amount of invisible Empties instances around
         * the map.
         */
        this.animator.SetTrigger("Die");
    }
}
