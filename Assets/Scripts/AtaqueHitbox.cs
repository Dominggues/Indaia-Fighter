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
        // Se a animação desligar o colisor ou o objeto, reseta a trava do soco!
        if (meuColisor != null && !meuColisor.enabled)
        {
            jaDeuDano = false;
        }
    }

    void OnEnable()
    {
        jaDeuDano = false; // Garante o reset também caso o GameObject seja desativado
    }

    // Usamos OnTriggerStay2D no lugar de Enter2D. 
    // Assim, se o soco ligar enquanto eles estão parados grudados, funciona!
    void OnTriggerStay2D(Collider2D collision)
    {
        if (jaDeuDano) return; 

        Lutador lutadorAtingido = collision.GetComponent<Lutador>();

        if (lutadorAtingido != null && meuLutador != null)
        {
            if (lutadorAtingido.isPlayer1 != meuLutador.isPlayer1)
            {
                lutadorAtingido.TakeDamage(danoDoAtaque);
                jaDeuDano = true; 
            }
        }
    }
}