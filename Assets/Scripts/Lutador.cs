using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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
        // Se estiver morto OU o jogo estiver pausado (Time.timeScale == 0), ignora os botões!
        if (morto || Time.timeScale == 0f) return; 
        
        InputsJogador();
        Ataques();
    }

    void FixedUpdate()
    {
        // Se estiver morto OU o jogo estiver pausado, não se move!
        if (morto || Time.timeScale == 0f) return;
        
        Mover();
    }

    void InputsJogador()
    {
        if (isPlayer1)
        {
            estaAbaixado = Input.GetKey(KeyCode.S); 
            if (anim != null) anim.SetBool("estaAbaixando", estaAbaixado);
            estaBloqueando = Input.GetKey(KeyCode.LeftShift);
            if (anim != null) anim.SetBool("estaBloqueando", estaBloqueando);

            moveX = 0;
            if (!estaAbaixado && !estaBloqueando)
            {
                if (Input.GetKey(KeyCode.A)) moveX = -1;
                if (Input.GetKey(KeyCode.D)) moveX = 1;
                if (Input.GetKeyDown(KeyCode.Space) && isGrounded) Jump();
            }
        }
        else 
        {
            estaAbaixado = Input.GetKey(KeyCode.DownArrow);
            if (anim != null) anim.SetBool("estaAbaixando", estaAbaixado);
            estaBloqueando = Input.GetKey(KeyCode.RightShift);
            if (anim != null) anim.SetBool("estaBloqueando", estaBloqueando);

            moveX = 0;
            if (!estaAbaixado && !estaBloqueando)
            {
                if (Input.GetKey(KeyCode.LeftArrow)) moveX = -1;
                if (Input.GetKey(KeyCode.RightArrow)) moveX = 1;
                if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded) Jump();
            }
        }
    }

    void Mover()
    {
        float movimentoFinal = moveX;

        if (tocandoOutroJogador) 
        {
            if ((moveX > 0 && direcaoDoOutroJogador > 0) || (moveX < 0 && direcaoDoOutroJogador < 0))
            {
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
        if (estaBloqueando) return;

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
    }

    public void TakeDamage(int damage)
    {
        if (morto) return;

        if (estaBloqueando) 
        {
            damage = 0; 
        }

        currentLife -= damage;
        if (healthSlider != null) healthSlider.value = currentLife;
        
        // TOCA O SOM DE IMPACTO SE LEVOU DANO!
        if (damage > 0) 
        {
            StartCoroutine(PiscarVermelho());
            if (audioSourceLutador != null && somImpacto != null)
            {
                audioSourceLutador.PlayOneShot(somImpacto);
            }
        }

        if (currentLife <= 0)
        {
            currentLife = 0;
            Die();
        }
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
            anim.SetBool("IsRun", false);
            anim.SetBool("IsJump", false);
            anim.SetBool("estaAbaixando", false);
            anim.SetBool("estaBloqueando", false);
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