using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    Player player;

    void Start()
    {
        player = GetComponent<Player>();
    }

    void Update()
    {
        if (player == null) return;

        // Xử lý nhảy (nên kết hợp với BasicSpawner để đồng bộ Input)
        if (Input.GetButtonDown("Jump"))
        {
            // Logic nhảy local để tạo cảm giác mượt (Client-side prediction)
            if (player.isGround)
            {
                player.Jump();
                player.canDoubleJump = true;
            }
            else if (player.canDoubleJump)
            {
                player.DoubleJump();
                player.canDoubleJump = false;
            }
        }
    }
}