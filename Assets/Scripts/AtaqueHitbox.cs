using UnityEngine;

public class AtaqueHitbox : MonoBehaviour
{
    public int danoDoAtaque = 10;
    public bool pertenceAoPlayer1; // Para ele não bater nele mesmo

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica se bateu em um lutador
        Lutador lutadorAtingido = collision.GetComponent<Lutador>();

        if (lutadorAtingido != null)
        {
            // Verifica se o lutador atingido NÃO é o dono do ataque
            if (lutadorAtingido.isPlayer1 != pertenceAoPlayer1)
            {
                lutadorAtingido.TakeDamage(danoDoAtaque);
                
                // Desliga a hitbox imediatamente após acertar para não dar dano duplo
                gameObject.SetActive(false); 
            }
        }
    }
}