using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Lutador : MonoBehaviour
{
    [Header("Configuração de Player")]
    public bool isPlayer1; 
    public string nomeLutador;

    [Header("Movimentação")]
    public float speed = 5f;
    public float jumpForce = 10f;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr; // <-- ADICIONADO: Para controlar a cor
    private bool isGrounded;
    private float moveX;

    [Header("Estados do Lutador")]
    public bool estaAbaixado = false;
    public bool estaBloqueando = false;

    [Header("Sistema de Vida")]
    public int maxLife = 100;
    public int currentLife;
    public Slider healthSlider;

    private bool morto = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>(); // <-- ADICIONADO: Puxa o SpriteRenderer
        currentLife = maxLife;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxLife;
            healthSlider.value = currentLife;
        }
    }

    void Update()
    {
        if (morto) return;

        InputsJogador();
        Ataques();
    }

    void FixedUpdate()
    {
        if (morto) return;
        Mover();
    }

    void InputsJogador()
    {
        if (isPlayer1)
        {
            estaAbaixado = Input.GetKey(KeyCode.S); 
            anim.SetBool("estaAbaixando", estaAbaixado);

            estaBloqueando = Input.GetKey(KeyCode.LeftShift);
            anim.SetBool("estaBloqueando", estaBloqueando);

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
            estaAbaixado = Input.GetKey(KeyCode.PageDown);
            anim.SetBool("estaAbaixando", estaAbaixado);

            estaBloqueando = Input.GetKey(KeyCode.RightShift);
            anim.SetBool("estaBloqueando", estaBloqueando);

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
        rb.velocity = new Vector2(moveX * speed, rb.velocity.y);

        if (moveX > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            anim.SetBool("IsRun", true);
        }
        else if (moveX < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            anim.SetBool("IsRun", true);
        }
        else
        {
            anim.SetBool("IsRun", false);
        }
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        anim.SetBool("IsJump", true);
        isGrounded = false;
    }

    void Ataques()
    {
        if (estaBloqueando) return;

        if (isPlayer1)
        {
            if (Input.GetKeyDown(KeyCode.F)) anim.SetTrigger("Soco");
            if (Input.GetKeyDown(KeyCode.G)) anim.SetTrigger("Chute");
        }
        else 
        {
            if (Input.GetKeyDown(KeyCode.K)) anim.SetTrigger("Soco");
            if (Input.GetKeyDown(KeyCode.L)) anim.SetTrigger("Chute");
        }
    }

    public void TakeDamage(int damage)
    {
        if (morto) return;

        if (estaBloqueando)
        {
            damage = damage / 2; 
        }

        currentLife -= damage;
        if (healthSlider != null) healthSlider.value = currentLife;
        
        StartCoroutine(PiscarVermelho()); // <-- ADICIONADO: Chama a animação de piscar

        if (currentLife <= 0)
        {
            currentLife = 0;
            Die();
        }
    }

    // <-- ADICIONADO: Sistema que faz o boneco ficar vermelho por 0.15 segundos
    IEnumerator PiscarVermelho()
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        sr.color = Color.white;
    }

    void Die()
    {
        morto = true;
        anim.SetTrigger("Morte");
        rb.velocity = Vector2.zero;
        this.enabled = false; 
    }

    public void ComemorarVitoria()
    {
        anim.SetTrigger("Vitoria");
        rb.velocity = Vector2.zero;
        this.enabled = false; 
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            anim.SetBool("IsJump", false);
        }
    }
}