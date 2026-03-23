using UnityEngine;
using Fusion;

public class EnemyHealth : NetworkBehaviour
{
    public int maxEnemyHealth = 100;
    
    [Networked] 
    public float currentEnemyHealth { get; set; }

    public float playerDamageToEnemy = 25;
    public GameObject deathParticle;

    public override void Spawned()
    {
        // Khởi tạo máu trên máy chủ
        if (Object.HasStateAuthority)
        {
            currentEnemyHealth = maxEnemyHealth;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Khi trúng vũ khí người chơi (Tag: PlayerItem)
        if (other.CompareTag("PlayerItem"))
        {
            // Lấy component Player từ cha của vũ khí
            Player p = other.GetComponentInParent<Player>();
            
            // Kiểm tra quyền Server để trừ máu
            if (p != null && Object.HasStateAuthority)
            {
                currentEnemyHealth -= playerDamageToEnemy;
                
                if (currentEnemyHealth <= 0)
                {
                    Die();
                }
            }
        }
    }

    void Die()
    {
        // Hiệu ứng hạt (Particle)
        if (deathParticle != null)
        {
            Instantiate(deathParticle, transform.position, Quaternion.identity);
        }
        
        // Xóa quái khỏi hệ thống mạng
        Runner.Despawn(Object);
    }
}