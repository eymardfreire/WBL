using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 10.0f;
    public float movementAmount = 100.0f;
    public float maxMovementAmount = 100.0f;
    private Animator animator;
    private Rigidbody rb;
    private Player player;

    public bool CanMove { get; private set; } = true;

    void Awake()
    {
        // Make sure PlayerMovement is a child of Player in the hierarchy
        player = GetComponentInParent<Player>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        // You can comment out LockPosition here if you're sure that all players should start with locked positions
        // Otherwise, you can leave it to ensure that all players start locked
        LockPosition();
    }

    void Start()
    {
        transform.forward = Vector3.back;
    }

    void Update()
    {

        if (!player.isActivePlayer || ChatController.isChatActive)
        {
            return;
        }

        if (!player.isActivePlayer)
        {
            return;
        }

        float moveHorizontal = Input.GetAxis("Horizontal");

        // Update the character's rotation to face the direction of movement regardless of the movement amount
        if (moveHorizontal > 0)
        {
            transform.forward = Vector3.right; // Faces the positive X-axis
        }
        else if (moveHorizontal < 0)
        {
            transform.forward = Vector3.left; // Faces the negative X-axis
        }

        // Movement is only allowed if there is movement amount left and the player can move
        if (CanMove && movementAmount > 0)
        {
            movementAmount -= Mathf.Abs(moveHorizontal) * Time.deltaTime * speed;
            rb.MovePosition(rb.position + new Vector3(moveHorizontal, 0.0f, 0.0f) * Time.deltaTime * speed);
        }

        UpdateAnimation(moveHorizontal);

        // Check if the player's Z position has deviated from 0
        if (Mathf.Abs(rb.position.z) > 0.001f) // Small threshold to account for floating point precision issues
        {
            // Correct the Z position without directly setting transform.position
            Vector3 correctedPosition = new Vector3(rb.position.x, rb.position.y, 0f);
            rb.MovePosition(correctedPosition);
        }
    }

    void UpdateAnimation(float moveHorizontal)
    {
        // The walking animation should only play if there's horizontal input and the player can move
        bool isMoving = Mathf.Abs(moveHorizontal) > 0.1f && CanMove && movementAmount > 0;
        animator.SetBool("isMoving", isMoving);

        // When movementAmount is 0, we can still change direction, but the walking animation shouldn't play
        if (movementAmount <= 0 && CanMove)
        {
            animator.SetBool("isMoving", false);
        }
    }


    public void ResetMovementAmount()
    {
        movementAmount = maxMovementAmount;
    }

    public void StartChargingAnimation(bool isCharging)
    {
        animator.SetBool("IsCharging", isCharging);
        CanMove = !isCharging;
    }

    public void CastSpellAnimation()
    {
        animator.SetTrigger("CastSpell");
        CanMove = false;
    }

    public bool IsMoving()
    {
        return Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f;
    }

    public void EnableMovement(bool enable)
    {
        CanMove = enable;
    }

    public void PlayVictoryAnimation()
    {
        if (animator != null)
        {
            animator.SetBool("IsVictorious", true);
            animator.SetBool("IsDefeated", false); // Ensure to reset any other states if needed
        }
    }

    public void PlayDefeatAnimation()
    {
        if (animator != null)
        {
            animator.SetBool("IsDefeated", true);
            animator.SetBool("IsVictorious", false); // Ensure to reset any other states if needed
        }
    }
    public void LockPosition()
    {
        // Lock the player's position on the X and Z axis but allow movement along the Y axis (up and down)
        rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
    }

    // Call this method to unlock the player's position when needed
    public void UnlockPosition()
    {
        if (rb != null) // Check that rb is not null before accessing it
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
        else
        {
            Debug.LogError("Rigidbody component not found", this);
        }
    }
}
