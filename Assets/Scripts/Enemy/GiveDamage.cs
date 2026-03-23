using UnityEngine;
using Fusion;

public class GiveDamage : MonoBehaviour
{
    public int damage = 10;

void OnTriggerEnter2D(Collider2D col) {
    if (col.CompareTag("Player")) {
        Player p = col.GetComponent<Player>();
        if (p != null && p.Object.HasStateAuthority) { p.currentPlayerHealth -= 10; p.isHurt = true; }
    }
}
}