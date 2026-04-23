using UnityEngine;

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
    public int life;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        moveX = Input.GetAxisRaw("Horizontal");

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
    }

    void FixedUpdate()
    {
        // Aqui dentro deixamos apenas o que é contínuo (andar de um lado para o outro)
        Move();
        Attack();
        Attack2();
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