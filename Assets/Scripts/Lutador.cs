using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Lutador : MonoBehaviour
{
    [Header("Configuração de Player")]
    public bool isPlayer1; // Marque no Inspector se for o P1
    public string nomeLutador;
    // public LutaManager lutaManager; // Arraste o objeto Juiz aqui

    [Header("Movimentação")]
    public float speed = 5f;
    public float jumpForce = 10f;
    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded;
    private float moveX;

    [Header("Sistema de Vida")]
    public int maxLife = 100;
    public int currentLife;
    public Slider healthSlider; 

    private bool morto = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
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
            moveX = 0;
            if (Input.GetKey(KeyCode.A)) moveX = -1;
            if (Input.GetKey(KeyCode.D)) moveX = 1;

            if (Input.GetKeyDown(KeyCode.W) && isGrounded) Jump();
        }
        else // Controles Player 2
        {
            moveX = 0;
            if (Input.GetKey(KeyCode.LeftArrow)) moveX = -1;
            if (Input.GetKey(KeyCode.RightArrow)) moveX = 1;

            if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded) Jump();
        }
    }

    void Mover()
    {
        rb.velocity = new Vector2(moveX * speed, rb.velocity.y);

        // CORREÇÃO: Usando a Escala para virar o personagem de forma segura!
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
        // CORREÇÃO: Usando Triggers para facilitar a criação no Animator
        if (isPlayer1)
        {
            if (Input.GetKeyDown(KeyCode.F)) anim.SetTrigger("Soco"); 
            if (Input.GetKeyDown(KeyCode.G)) anim.SetTrigger("Chute"); 
        }
        else // Player 2
        {
            if (Input.GetKeyDown(KeyCode.K)) anim.SetTrigger("Soco"); 
            if (Input.GetKeyDown(KeyCode.L)) anim.SetTrigger("Chute"); 
        }
    }

    public void TakeDamage(int damage)
    {
        if (morto) return;

        currentLife -= damage;
        if (healthSlider != null) healthSlider.value = currentLife;

        if (currentLife <= 0)
        {
            currentLife = 0;
            Die();
        }
    }

    void Die()
    {
        morto = true;
        anim.SetTrigger("Die"); 
        //lutaManager.VerificarVencedor(this);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // ATENÇÃO: Confirme se o seu chão tem a Tag "Ground" certinho com letra maiúscula!
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            anim.SetBool("IsJump", false);
        }
    }
}