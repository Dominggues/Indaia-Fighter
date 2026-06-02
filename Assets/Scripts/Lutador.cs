using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Lista de tipos de ataques para o sistema identificar os golpes
public enum TipoAtaque
{
    Soco,
    Chute,
    Outro
}

public class Lutador : MonoBehaviour
{
    [Header("Configuração de Player")]
    public bool isPlayer1;
    public string nomeLutador;
    public Sprite fotoPerfil;

    [Header("Movimentação e Status")]
    public float speed = 5f;
    public float jumpForce = 10f;
    public bool estaAbaixado = false;
    public bool estaBloqueando = false;

    [Header("Knockback")]
    public float knockbackForca = 5f;
    public float knockbackAlturaForca = 3f;
    public float knockbackDuracao = 0.2f; // segundos que o Mover() fica suspenso

    private bool emKnockback = false;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;
    private bool isGrounded;
    private float moveX;
    private Vector3 posicaoInicial;

    [Header("Sistema de Vida")]
    public int maxLife = 100;
    public int currentLife;
    public Slider healthSlider;

    [Header("Áudio do Lutador")]
    public AudioSource audioSourceLutador;
    [Tooltip("Som quando o lutador toma um soco/chute")]
    public AudioClip somImpacto;

    private bool morto = false;
    private bool tocandoOutroJogador = false;
    private float direcaoDoOutroJogador = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        posicaoInicial = transform.position;

        if (audioSourceLutador == null) audioSourceLutador = GetComponent<AudioSource>();
    }

    public void ConfigurarUIInicial()
    {
        currentLife = maxLife;

        if (healthSlider == null)
        {
            string nomeDaBarra = isPlayer1 ? "VidaP1" : "VidaP2";
            GameObject barraObj = GameObject.Find(nomeDaBarra);
            if (barraObj != null) healthSlider = barraObj.GetComponent<Slider>();
        }

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxLife;
            healthSlider.value = currentLife;
        }

        string nomeDaFoto = isPlayer1 ? "FotoP1" : "FotoP2";
        GameObject fotoObj = GameObject.Find(nomeDaFoto);

        if (fotoObj != null)
        {
            Image componenteImagem = fotoObj.GetComponent<Image>();
            if (componenteImagem != null) componenteImagem.sprite = fotoPerfil;
        }
    }

    void Update()
    {
        if (morto || Time.timeScale == 0f) return;
        InputsJogador();
        Ataques();
    }

    void FixedUpdate()
    {
        if (morto || Time.timeScale == 0f) return;

        // Enquanto em knockback, não sobrescreve a velocidade — deixa a física agir
        if (emKnockback) return;

        Mover();
    }

    void InputsJogador()
    {
        int meuJoystick = isPlayer1 ? GerenciadorControles.JoystickP1 : GerenciadorControles.JoystickP2;
        string prefixoEixo = isPlayer1 ? "P1" : "P2";

        float lyEixo    = Input.GetAxisRaw($"PS4_LY_{prefixoEixo}");
        float dpadYEixo = Input.GetAxisRaw($"PS4_DpadY_{prefixoEixo}");
        bool abaixadoControle   = (lyEixo > 0.5f) || (dpadYEixo > 0.5f);
        bool bloqueandoControle = Input.GetKey($"joystick {meuJoystick} button 4");

        bool abaixadoTeclado   = isPlayer1 ? Input.GetKey(KeyCode.S)         : Input.GetKey(KeyCode.DownArrow);
        bool bloqueandoTeclado = isPlayer1 ? Input.GetKey(KeyCode.LeftShift)  : Input.GetKey(KeyCode.RightShift);

        estaAbaixado   = abaixadoTeclado   || abaixadoControle;
        estaBloqueando = bloqueandoTeclado || bloqueandoControle;

        if (anim != null) anim.SetBool("estaAbaixando",  estaAbaixado);
        if (anim != null) anim.SetBool("estaBloqueando", estaBloqueando);

        moveX = 0;
        if (!estaAbaixado && !estaBloqueando)
        {
            if (isPlayer1)
            {
                if (Input.GetKey(KeyCode.A)) moveX = -1;
                if (Input.GetKey(KeyCode.D)) moveX =  1;
                if (Input.GetKeyDown(KeyCode.Space) && isGrounded) Jump();
            }
            else
            {
                if (Input.GetKey(KeyCode.LeftArrow))  moveX = -1;
                if (Input.GetKey(KeyCode.RightArrow)) moveX =  1;
                if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded) Jump();
            }

            float lxEixo    = Input.GetAxisRaw($"PS4_LX_{prefixoEixo}");
            float dpadXEixo = Input.GetAxisRaw($"PS4_DpadX_{prefixoEixo}");
            float eixoH     = (Mathf.Abs(lxEixo) > Mathf.Abs(dpadXEixo)) ? lxEixo : dpadXEixo;

            if (Mathf.Abs(eixoH) > 0.3f) moveX = Mathf.Sign(eixoH);

            if (Input.GetKeyDown($"joystick {meuJoystick} button 1") && isGrounded) Jump();
        }
    }

    void Mover()
    {
        float movimentoFinal = moveX;

        if (tocandoOutroJogador)
        {
            if ((moveX > 0 && direcaoDoOutroJogador > 0) || (moveX < 0 && direcaoDoOutroJogador < 0))
            {
                // FIX bug 1: se está bloqueando, afasta levemente em vez de travar
                // para evitar que a física trave os dois personagens juntos
                if (estaBloqueando)
                    movimentoFinal = -direcaoDoOutroJogador * 0.5f;
                else
                    movimentoFinal = 0;
            }
        }

        rb.linearVelocity = new Vector2(movimentoFinal * speed, rb.linearVelocity.y);

        if (moveX > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            if (anim != null) anim.SetBool("IsRun", true);
        }
        else if (moveX < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            if (anim != null) anim.SetBool("IsRun", true);
        }
        else
        {
            if (anim != null) anim.SetBool("IsRun", false);
        }
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        if (anim != null) anim.SetBool("IsJump", true);
        isGrounded = false;
    }

    void Ataques()
    {
        // NOVA REGRA: Se o jogador estiver bloqueando OU abaixado, ele não pode atacar.
        if (estaBloqueando || estaAbaixado) return;

        int meuJoystick = isPlayer1 ? GerenciadorControles.JoystickP1 : GerenciadorControles.JoystickP2;

        if (isPlayer1)
        {
            if (Input.GetKeyDown(KeyCode.F)) if (anim != null) anim.SetTrigger("Soco");
            if (Input.GetKeyDown(KeyCode.G)) if (anim != null) anim.SetTrigger("Chute");
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.K)) if (anim != null) anim.SetTrigger("Soco");
            if (Input.GetKeyDown(KeyCode.L)) if (anim != null) anim.SetTrigger("Chute");
        }

        if (Input.GetKeyDown($"joystick {meuJoystick} button 0")) if (anim != null) anim.SetTrigger("Soco");
        if (Input.GetKeyDown($"joystick {meuJoystick} button 2")) if (anim != null) anim.SetTrigger("Chute");
    }

    public void TakeDamage(int damage, Vector2 posicaoDoAtacante, TipoAtaque tipoDoAtaque)
    {
        if (morto) return;

        if (estaAbaixado && (tipoDoAtaque == TipoAtaque.Soco || tipoDoAtaque == TipoAtaque.Chute))
        {
            Debug.Log(nomeLutador + " evitou o dano pois estava abaixado!");
            return;
        }

        if (estaBloqueando)
        {
            bool atacantePelaDireita = posicaoDoAtacante.x > transform.position.x;
            bool olhandoParaDireita  = transform.localScale.x > 0;
            bool ataqueVeioDeFrente  = atacantePelaDireita == olhandoParaDireita;

            if (ataqueVeioDeFrente) damage = 0;
            // Se veio por trás, o dano passa normal — bloqueio ignorado
        }

        currentLife -= damage;
        if (healthSlider != null) healthSlider.value = currentLife;

        if (damage > 0)
        {
            StartCoroutine(PiscarVermelho());

            if (audioSourceLutador != null && somImpacto != null)
                audioSourceLutador.PlayOneShot(somImpacto);

            // Aplica o knockback e suspende o Mover() pelo tempo definido
            float direcao = transform.position.x > posicaoDoAtacante.x ? 1f : -1f;
            rb.linearVelocity = new Vector2(direcao * knockbackForca, knockbackAlturaForca);
            StartCoroutine(TempoKnockback());
        }

        if (currentLife <= 0)
        {
            currentLife = 0;
            Die();
        }
    }

    IEnumerator TempoKnockback()
    {
        emKnockback = true;
        yield return new WaitForSeconds(knockbackDuracao);
        emKnockback = false;
    }

    IEnumerator PiscarVermelho()
    {
        if (sr != null) sr.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        if (sr != null) sr.color = Color.white;
    }

    void Die()
    {
        morto = true;
        rb.linearVelocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        Collider2D colisorPrincipal = GetComponent<Collider2D>();
        if (colisorPrincipal != null) colisorPrincipal.enabled = false;

        if (anim != null)
        {
            anim.SetBool("IsRun",         false);
            anim.SetBool("IsJump",        false);
            anim.SetBool("estaAbaixando", false);
            anim.SetBool("estaBloqueando",false);
            anim.SetTrigger("Morte");
        }

        this.enabled = false;

        ControladorLuta juiz = FindObjectOfType<ControladorLuta>();
        if (juiz != null) juiz.FinalizarRoundPorNocaute(this);
    }

    public void ComemorarVitoria()
    {
        if (anim != null) anim.SetTrigger("Vitoria");
        rb.linearVelocity = Vector2.zero;
        this.enabled = false;
    }

    public void ResetarParaNovoRound()
    {
        morto = false;
        emKnockback = false;
        currentLife = maxLife;
        if (healthSlider != null) healthSlider.value = currentLife;

        transform.position = posicaoInicial;

        rb.linearVelocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.None;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        Collider2D colisorPrincipal = GetComponent<Collider2D>();
        if (colisorPrincipal != null) colisorPrincipal.enabled = true;

        if (anim != null)
        {
            anim.Rebind();
            anim.Update(0f);
        }

        if (sr != null) sr.color = Color.white;
        tocandoOutroJogador = false;
        direcaoDoOutroJogador = 0f;
        estaAbaixado = false;
        estaBloqueando = false;
    
        this.enabled = true;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            if (anim != null) anim.SetBool("IsJump", false);
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Lutador>() != null)
        {
            tocandoOutroJogador = true;
            direcaoDoOutroJogador = Mathf.Sign(collision.transform.position.x - transform.position.x);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Lutador>() != null)
        {
            tocandoOutroJogador = false;
            direcaoDoOutroJogador = 0f;
        }
    }
}