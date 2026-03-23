using UnityEngine;
using Fusion;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : NetworkBehaviour 
{
    private Rigidbody2D body2D;
    public float playerSpeed = 15;
    public float jumpPower = 10;
    
    [Header("Ground Check")]
    public bool isGround;
    public Transform groundCheck;
    public LayerMask groundLayer;
    const float GroundCheckRadius = .2f;

    private bool facingRight = true;
    private Animator playerAnimController;

    // --- BIẾN ĐỒNG BỘ MẠNG (QUAN TRỌNG) ---
    [Networked] public int currentPlayerHealth { get; set; } = 100;
    [Networked] public int maxPlayerHealth { get; set; } = 100;
    [Networked] public NetworkBool isDead { get; set; }
    [Networked] public NetworkBool isHurt { get; set; }
    [Networked] public NetworkBool earnCoin { get; set; }
    [Networked] public NetworkBool addHealth { get; set; }
    [Networked] public NetworkBool canDamage { get; set; } // Fix lỗi cho EnemyHealth

    [HideInInspector] public bool canDoubleJump; 

    public override void Spawned()
    {
        body2D = GetComponent<Rigidbody2D>();
        playerAnimController = GetComponent<Animator>();

        if (Object.HasStateAuthority)
        {
            currentPlayerHealth = maxPlayerHealth;
            isDead = false;
            canDamage = true; 
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (isDead) return;

        isGround = Physics2D.OverlapCircle(groundCheck.position, GroundCheckRadius, groundLayer);

        if (GetInput(out BasicSpawner.NetworkInputData data))
        {
            body2D.linearVelocity = new Vector2(data.move.x * playerSpeed, body2D.linearVelocity.y);

            if (data.move.x > 0 && !facingRight) Flip();
            else if (data.move.x < 0 && facingRight) Flip();

            if (data.jump && isGround) Jump();
        }

        if (Object.HasStateAuthority && currentPlayerHealth <= 0)
        {
            isDead = true;
        }

        UpdateAnimations();
    }

    public void Jump()
    {
        body2D.linearVelocity = new Vector2(body2D.linearVelocity.x, jumpPower); 
    }

    public void DoubleJump() 
    {
        body2D.linearVelocity = new Vector2(body2D.linearVelocity.x, jumpPower * 0.8f);
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    void UpdateAnimations()
    {
        if (playerAnimController != null)
        {
            playerAnimController.SetFloat("VelocityX", Mathf.Abs(body2D.linearVelocity.x));
            playerAnimController.SetBool("isGround", isGround);
            playerAnimController.SetBool("isDead", (bool)isDead);
        }
    }
}