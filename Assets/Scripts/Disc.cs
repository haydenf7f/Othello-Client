using UnityEngine;

public class Disc : MonoBehaviour{

    // The player that owns this disc (Black or White)
    [SerializeField]
    private Player faceUp;

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Flip() {
        Debug.Log("Flipping disc at position: " + "(" + transform.position.z + ", " + transform.position.x + ")");

        // If Black is face up, play the animation to flip to white
        if (faceUp == Player.Black) {
            animator.Play("BlackToWhite");
            faceUp = Player.White;
        }
        // If White is face up, play the animation to flip to black
        else {
            animator.Play("WhiteToBlack");
            faceUp = Player.Black;
        }
    }

    public void Jump() {
        animator.Play("LittleJump");
    }
}
