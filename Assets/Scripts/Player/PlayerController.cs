using UnityEngine;
using UnityEngine.UI; // ESSENCIAL: Permite que o código converse com o Slider e imagens da UI

public class PlayerController : MonoBehaviour
{
    // VARIAVEIS PRIVADA
    private Rigidbody2D rb;
    private float moveX;
    private Animator anim;

    // VARIAVEIS PUBLICAS
    public float speed;
    public int addJumps;
    public bool isGrounded;
    public float jumpForce;

    [Header("Sistema de Vida")]
    public int maxLife = 100;
    public int currentLife;
    public Slider healthSlider; // É AQUI QUE VOCÊ VAI ARRASTAR O SEU SLIDER "VidaP1"

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // Enche a vida quando a luta começa
        currentLife = maxLife;
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxLife;
            healthSlider.value = currentLife;
        }
    }

    void Update()
    {
        moveX = Input.GetAxisRaw("Horizontal");

        // Os ataques vieram para o Update para não falharem o clique!
        Attack();
        Attack2();

        if (isGrounded == true)
        {
            addJumps = 0;
            if (Input.GetButtonDown("Jump"))
            {
                Jump();
            }
        }
        else
        {
            if (Input.GetButtonDown("Jump") && addJumps > 0)
            {
                addJumps--;
                Jump();
            }
        }

        // BOTAO DE TESTE: Aperte 'T' no teclado para ver se a barra está perdendo vida
        if (Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(10);
        }
    }

    void FixedUpdate()
    {
        // Aqui dentro deixamos apenas o que é contínuo (andar de um lado para o outro)
        Move();
    }

    void Move()
    {
        rb.velocity = new Vector2(moveX * speed, rb.velocity.y);

        if (moveX > 0)
        {
            transform.eulerAngles = new Vector3(0f, 0f, 0f);
            anim.SetBool("IsRun" , true);
        }

        if(moveX < 0) // SE O PLAYER ESTIVER OLHANDO PARA O LADO ESQUERDO
        {
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
            anim.SetBool("IsRun" , true);
        }

        if (moveX == 0)
        {
            anim.SetBool("IsRun" , false);
        }
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        anim.SetBool("IsJump" , true);
    }

    void Attack()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            anim.Play("Attack", -1);
        }
    }
    
    void Attack2()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            anim.Play("Attack1", -1);
        }
    }

    // --- NOVA LÓGICA DE SOFRER DANO ---
    public void TakeDamage(int damageAmount)
    {
        currentLife -= damageAmount;

        // Impede que a vida fique negativa (ex: -10)
        if (currentLife < 0)
        {
            currentLife = 0;
        }

        // Atualiza a barrinha visual lá no topo da tela
        if (healthSlider != null)
        {
            healthSlider.value = currentLife;
        }

        // Verifica se morreu
        if (currentLife == 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("K.O.! Jogador foi derrotado!");
        // Futuramente colocaremos a animação de morte aqui: anim.Play("Die");
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = true;
            anim.SetBool("IsJump" , false);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = false;
        }
    }
}