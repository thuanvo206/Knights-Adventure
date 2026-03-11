using UnityEngine;
using TMPro;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    internal Rigidbody2D body2D;
    public float knockBackForce = 15000;

    BoxCollider2D box2D;
    CircleCollider2D cir2D;

    [Tooltip("Xác định tốc độ di chuyển của nhân vật.")]
    [Range(0, 20)]
    public float playerSpeed = 15;

    [Tooltip("Xác định độ cao khi nhân vật nhảy.")]
    [Range(500, 1500)]
    public float jumpPower = 1000;

    [Tooltip("Xác định độ cao khi nhân vật nhảy lần thứ 2 (Double Jump).")]
    [Range(500, 1000)]
    public float doubleJumpPower = 600;

    internal bool canDoubleJump;
    internal bool canDamage;

    // Player Scale (Hướng mặt của nhân vật)
    bool facingRight = true;

    [Tooltip("Kiểm tra xem nhân vật có đang chạm đất không.")]
    public bool isGround = true;

    Transform groundCheck;
    const float GroundCheckRadius = .1f;

    [Tooltip("Xác định Layer của mặt đất (để check va chạm).")]
    public LayerMask groundLayer;

    // Anim Controller
    Animator playerAnimController;

    // Player Health
    internal int maxPlayerHealth = 100;
    public int currentPlayerHealth;
    internal bool isHurt;
    internal bool addHealth;
    internal bool earnCoin;
    GiveDamage giveDamage;
    GiveHealth giveHealth;

    public int currentCoin = 0;
    AddCoin addCoin;

    internal bool isDead;
    public float deadForce = 5;

    TextMeshProUGUI coinText;

    AudioSource audioSource;
    AudioClip audioJump;
    AudioClip audioHurt;
    AudioClip audioCoin;
    AudioClip audioHealth;

    void Start()
    {
        body2D = GetComponent<Rigidbody2D>();
        body2D.gravityScale = 5;
        body2D.freezeRotation = true;
        body2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        box2D = GetComponent<BoxCollider2D>();
        cir2D = GetComponent<CircleCollider2D>();

        groundCheck = transform.Find("GroundCheck");

        playerAnimController = GetComponent<Animator>();

        currentPlayerHealth = maxPlayerHealth;
        // Lưu ý: Các script GiveDamage, GiveHealth, AddCoin phải tồn tại trong Scene
        giveDamage = FindObjectOfType<GiveDamage>();
        giveHealth = FindObjectOfType<GiveHealth>();
        addCoin = FindObjectOfType<AddCoin>();

        // Tìm UI Text Coin (Đảm bảo đường dẫn này đúng trong Hierarchy của bạn)
        var coinObj = GameObject.Find("HUD/CoinCanvas/CoinCounterText");
        if (coinObj != null)
            coinText = coinObj.GetComponent<TextMeshProUGUI>();

        // Audio Paths - Load âm thanh từ thư mục Resources/Sounds
        audioSource = GetComponent<AudioSource>();
        audioJump = Resources.Load("Sounds/Jump") as AudioClip;
        audioHurt = Resources.Load("Sounds/Hurt") as AudioClip;
        audioCoin = Resources.Load("Sounds/Coin") as AudioClip;
        audioHealth = Resources.Load("Sounds/Health") as AudioClip;
    }

    void Update()
    {
        UpdateAnimations();
        ReduceHealth();
        BoostHealth();
        AddCoin();

        isDead = currentPlayerHealth <= 0;

        if (currentPlayerHealth > maxPlayerHealth)
            currentPlayerHealth = maxPlayerHealth;

        // Nếu nhân vật rơi xuống vực (y <= -6) thì chết
        if (transform.position.y <= -6)
            isDead = true;

        if (isDead)
            KillPlayer();
    }

    void FixedUpdate()
    {
        isGround = Physics2D.OverlapCircle(groundCheck.position, GroundCheckRadius, groundLayer);

        float horizontal = Input.GetAxis("Horizontal");

        body2D.velocity = new Vector2(horizontal * playerSpeed, body2D.velocity.y);

        Flip(horizontal);

        if (isGround)
            canDamage = false;
    }

    public void Jump()
    {
        body2D.AddForce(new Vector2(0, jumpPower));
        audioSource.PlayOneShot(audioJump);
    }

    public void DoubleJump()
    {
        body2D.AddForce(new Vector2(0, doubleJumpPower));
        canDamage = true;
        audioSource.PlayOneShot(audioJump);
    }

    void Flip(float horizontal)
    {
        if (horizontal > 0 && !facingRight || horizontal < 0 && facingRight)
        {
            facingRight = !facingRight;

            Vector2 theScale = transform.localScale;

            theScale.x *= -1;

            transform.localScale = theScale;
        }
    }

    void UpdateAnimations()
    {
        playerAnimController.SetFloat("VelocityX", Mathf.Abs(body2D.velocity.x));
        playerAnimController.SetFloat("VelocityY", body2D.velocity.y);
        playerAnimController.SetBool("isGround", isGround);
        playerAnimController.SetBool("isDead", isDead);
        if (isHurt && !isDead)
            playerAnimController.SetTrigger("isHurt");
    }

    void ReduceHealth()
    {
        if (isHurt)
        {
            if (giveDamage != null) 
                currentPlayerHealth -= giveDamage.damage;
            
            isHurt = false;
            audioSource.PlayOneShot(audioHurt);

            // Xử lý lực đẩy lùi (Knockback) khi bị thương
            if (facingRight && !isGround)
                body2D.AddForce(new Vector2(-knockBackForce, 1000), ForceMode2D.Force);
            else if (!facingRight && !isGround)
                body2D.AddForce(new Vector2(knockBackForce, 1000), ForceMode2D.Force);

            if (facingRight && isGround)
                body2D.AddForce(new Vector2(-knockBackForce, 0), ForceMode2D.Force);
            else if (!facingRight && isGround)
                body2D.AddForce(new Vector2(knockBackForce, 0), ForceMode2D.Force);
        }
    }

    void BoostHealth()
    {
        if (addHealth)
        {
            if (giveHealth != null)
                currentPlayerHealth += giveHealth.health;
            
            addHealth = false;
            audioSource.PlayOneShot(audioHealth);
        }
    }

    void AddCoin()
    {
        if (earnCoin)
        {
            if (addCoin != null)
                currentCoin += addCoin.coin;
            
            if (coinText != null)
                coinText.text = currentCoin.ToString();
            
            earnCoin = false;
            audioSource.PlayOneShot(audioCoin);
        }
    }

    // Biến cờ để đảm bảo âm thanh chết chỉ phát 1 lần
    private bool hasPlayedDeadSound = false; 

    void KillPlayer()
    {
        isHurt = false;
        
        // Logic để nhân vật "bay" lên một chút khi chết rồi rơi xuống
        body2D.AddForce(new Vector2(0, deadForce), ForceMode2D.Impulse);
        body2D.drag = Time.deltaTime * 20;
        deadForce -= Time.deltaTime * 25;
        
        // Khóa di chuyển ngang
        body2D.constraints = RigidbodyConstraints2D.FreezePositionX;
        
        box2D.enabled = false;
        cir2D.enabled = false;

        // Sửa lỗi: Chỉ phát âm thanh chết 1 lần
        if (!hasPlayedDeadSound)
        {
            audioSource.PlayOneShot(audioHurt);
            hasPlayedDeadSound = true;
        }
    }
}