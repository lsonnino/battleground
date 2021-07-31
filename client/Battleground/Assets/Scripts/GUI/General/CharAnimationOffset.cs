using UnityEngine;

public class CharAnimationOffset : MonoBehaviour
{
    private enum Anim {
        IDLE, MOVE, ATTACK, DIE
    }

    public float idleXOffset, moveXOffset, attackXOffset, dieXOffset;

    private Animator animator;
    private Anim played;

    void Start() {
        this.animator = this.GetComponent<Animator>();
    }
    void Update() {
        Anim cur = GetCurrentAnim();
        
        if (played != cur) {
            RevertOffset(played);
            played = cur;
            ApplyOffset(played);
        }
    }

    private Anim GetCurrentAnim() {
        if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) {
            return Anim.IDLE;
        }
        else if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Move")) {
            return Anim.MOVE;
        }
        else if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")) {
            return Anim.ATTACK;
        }
        else if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Die")) {
            return Anim.DIE;
        }
        else {
            return Anim.IDLE;
        }
    }
    private float GetOffset(Anim anim) {
        switch (anim) {
            case Anim.IDLE:
                return idleXOffset;
            case Anim.MOVE:
                return moveXOffset;
            case Anim.ATTACK:
                return attackXOffset;
            case Anim.DIE:
                return dieXOffset;
            default:
                break;
        }

        return 0;
    }
    private void RevertOffset(Anim anim) {
        this.transform.Translate(new Vector3(-GetOffset(anim), 0, 0));
    }
    private void ApplyOffset(Anim anim) {
        this.transform.Translate(new Vector3(GetOffset(anim), 0, 0));
    }
}
