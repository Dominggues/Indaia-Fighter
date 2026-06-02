using UnityEngine;

public class AtaqueHitbox : MonoBehaviour
{
    public int danoDoAtaque = 10;
    private Lutador meuLutador;
    private bool jaDeuDano = false;
    private Collider2D meuColisor;

    void Start()
    {
        meuLutador = GetComponentInParent<Lutador>();
        meuColisor = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (meuColisor != null && !meuColisor.enabled)
            jaDeuDano = false;
    }

    void OnEnable()
    {
        jaDeuDano = false;
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (jaDeuDano) return;

        Lutador lutadorAtingido = collision.GetComponent<Lutador>();

        if (lutadorAtingido != null && meuLutador != null)
        {
            if (lutadorAtingido.isPlayer1 != meuLutador.isPlayer1)
            {
                // Passa a posição de quem atacou para calcular a direção do knockback
                lutadorAtingido.TakeDamage(danoDoAtaque, meuLutador.transform.position);
                jaDeuDano = true;
            }
        }
    }
}
