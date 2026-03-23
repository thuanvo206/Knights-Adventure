using UnityEngine;
using Fusion;

public class GiveHealth : MonoBehaviour
{
    public int healthAmount = 20;

void OnTriggerEnter2D(Collider2D col) {
    if (col.CompareTag("Player")) {
        Player p = col.GetComponent<Player>();
        if (p != null && p.Object.HasStateAuthority) {
            p.currentPlayerHealth = Mathf.Min(p.currentPlayerHealth + 20, p.maxPlayerHealth);
            Destroy(gameObject);
        }
    }
}
}