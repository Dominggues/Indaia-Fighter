using UnityEngine;

public class AtaqueHitbox : MonoBehaviour
{
    public int danoDoAtaque = 10;
    
    [Header("Configuração do Golpe")]
    [Tooltip("Selecione se este objeto de colisão representa um SOCO ou um CHUTE")]
    public TipoAtaque tipoDoAtaque = TipoAtaque.Soco; // Aparecerá como uma lista de seleção na Unity!

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
        {
            jaDeuDano = false;
        }
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
                // MODIFICADO: Enviamos o dano junto com o tipo do ataque configurado neste objeto
                lutadorAtingido.TakeDamage(danoDoAtaque, tipoDoAtaque);
                jaDeuDano = true; 
            }
        }
    }
}