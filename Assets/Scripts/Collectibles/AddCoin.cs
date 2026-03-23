using UnityEngine;
using Fusion;

public class AddCoin : MonoBehaviour
{
    public int coinValue = 1;

void OnTriggerEnter2D(Collider2D col) {
    if (col.CompareTag("Player")) {
        Player p = col.GetComponent<Player>();
        if (p != null && p.Object.HasStateAuthority) { p.earnCoin = true; Destroy(gameObject); }
    }
}
}