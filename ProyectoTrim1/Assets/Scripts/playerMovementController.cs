using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class playerMovementController : MonoBehaviour
{
    public float moveSpeed = 5f;       // Velocidad de movimiento
    public float jumpForce = 10f;      // Fuerza del salto
    public Transform groundCheck;      // Punto desde donde se detecta el suelo
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;      // Capa que representa el suelo

    private Rigidbody2D rb;
    private bool isGrounded;
    private SpriteRenderer sr;
    private Animator anim;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // Movimiento horizontal
        float moveInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        bool sprint = Input.GetKey(KeyCode.LeftShift);





        if (moveInput < 0)
        {
            sr.flipX = true;
        }
        else
            if (moveInput > 0)
        {
            sr.flipX = false;
        }


        if (moveInput != 0)
        {
            anim.SetBool("walk", true);
        }
        else
        {
            anim.SetBool("walk", false);
        }


        if (sprint)
        {
            rb.linearVelocity = new Vector2(moveInput * moveSpeed * 2, rb.linearVelocity.y);
            anim.SetBool("run", true);

        }
        else
        {
            anim.SetBool("run", false);
        }



        // Verificamos si estamos tocando el suelo
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);


        if (!isGrounded)
        {
            anim.SetBool("fall", true);
        }
        else 
        {
            anim.SetBool("fall", false);
        }

        // Salto
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            anim.SetTrigger("jump");

        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }



}
