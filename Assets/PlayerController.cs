using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Prime31;


public class PlayerController : MonoBehaviour
{
    public CharacterController2D.CharacterCollisionState2D flags;
    public float walkSpeed = 4.0f;     // Depois de incluido, alterar no Unity Editor
    public float jumpSpeed = 8.0f;     // Depois de incluido, alterar no Unity Editor
    public float gravity = 9.8f;       // Depois de incluido, alterar no Unity Editor

    public bool isGrounded;     // Se está no chão
    public bool isJumping;      // Se está pulando
    public bool isFalling;      // Se estiver caindo
    public bool isDucking;      // Se estiver agachando
    public bool isFacingRight;      // Se está olhando para a direita
    public static bool isAttacking;
    public bool isDead;
    public bool hasCollectedAll;
    public bool godmode;
    private int num_coins = 0;

    public GameObject Final_Start;
    public GameObject Coin_counter;


    public LayerMask mask;  // para filtrar os layers a serem analisados
    public LayerMask toxic;
    public LayerMask dino;


    public float doubleJumpSpeed = 6.0f; //Depois de incluido, alterar no Editor
    public bool doubleJumped; // informa se foi feito um pulo duplo

    private BoxCollider2D boxCollider;
    private float colliderSizeY;
    private float colliderOffsetY;
    private float originalBoxColliderY;
    private Animator animator;

    public AudioClip coin;
    public AudioClip sword;
    public AudioClip death;
    public AudioClip normal_level;
    public AudioClip final_level;

    




    IEnumerator PassPlatform(GameObject platform)
    {
        platform.GetComponent<EdgeCollider2D>().enabled = false;
        yield return new WaitForSeconds(1.0f);
        platform.GetComponent<EdgeCollider2D>().enabled = true;
    }


    IEnumerator Die()
    {
        isDead = true;
        AudioSource.PlayClipAtPoint(death, this.gameObject.transform.position);
        yield return new WaitForSeconds(0.15f);
        Destroy(gameObject);
        yield return new WaitForSeconds(2.5f);
    }

    IEnumerator play_godmode()
    {
        godmode = true;
        yield return new WaitForSeconds(2.0f);
        godmode = false;

    }



    private Vector3 moveDirection = Vector3.zero; // direção que o personagem se move
    private CharacterController2D characterController;  //Componente do Char. Controller
    private object collision;

    void Start()
    {
        characterController = GetComponent<CharacterController2D>(); //identif. o componente
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        colliderSizeY = boxCollider.size.y;
        colliderOffsetY = boxCollider.offset.y;
        hasCollectedAll = false;
        AudioSource audio = this.gameObject.GetComponent<AudioSource>();
        audio.clip = normal_level;
        audio.Play();

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Coins") && num_coins <= 6)
        {
            AudioSource.PlayClipAtPoint(coin, this.gameObject.transform.position);
            Destroy(other.gameObject);
            num_coins++;
            Coin_counter.GetComponent<Text>().text = ("X " + num_coins);
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("Coins") && num_coins > 6)
        {
            AudioSource.PlayClipAtPoint(coin, this.gameObject.transform.position);
            Destroy(other.gameObject);
            DinoController.firstAttack = true;
            this.gameObject.GetComponent<AudioSource>().Stop();
            this.gameObject.GetComponent<AudioSource>().clip = final_level;
            this.gameObject.GetComponent<AudioSource>().Play();
        }

    }

    void Update()
    {

        moveDirection.x = Input.GetAxis("Horizontal"); // recupera valor dos controles
        moveDirection.x *= walkSpeed;

        // Atualizando Animator com estados do jogo
        animator.SetFloat("movementX", Mathf.Abs(moveDirection.x / walkSpeed)); // +Normalizado
        animator.SetFloat("movementY", moveDirection.y);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isJumping", isJumping);
        animator.SetBool("isDucking", isDucking);
        animator.SetBool("isFalling", isFalling);
        animator.SetBool("hasCollectedAll", hasCollectedAll);
        animator.SetBool("isAttacking", isAttacking);
        animator.SetBool("isDead", isDead);

        if (!isDead)
        {
            if (moveDirection.x < 0)
            {
                transform.eulerAngles = new Vector3(0, 180, 0);
                isFacingRight = false;
            }
            else if (moveDirection.x > 0)
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
                isFacingRight = true;
            }

            if (moveDirection.y < 0)
                isFalling = true;
            else
                isFalling = false;


            if (isGrounded)
            {    // caso esteja no chão
                moveDirection.y = 0.0f; // se no chão nem subir nem descer
                isJumping = false;
                doubleJumped = false;
                if (Input.GetButton("Jump"))
                {
                    moveDirection.y = jumpSpeed;
                    isJumping = true;
                }
            }
            else
            {    // caso esteja pulando 
                if (Input.GetButtonUp("Jump") && moveDirection.y > 0) // Soltando botão diminui pulo
                    moveDirection.y *= 0.5f;

                if (Input.GetButtonDown("Jump") && !doubleJumped) // Segundo clique faz pulo duplo
                {
                    moveDirection.y = doubleJumpSpeed;
                    doubleJumped = true;
                }

            }

            RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, 4f, mask);
            if (hit.collider != null && isGrounded)
            {
                transform.SetParent(hit.transform);

                if (Input.GetAxis("Vertical") < 0 && Input.GetButtonDown("Jump"))
                {
                    moveDirection.y = -jumpSpeed;
                    StartCoroutine(PassPlatform(hit.transform.gameObject));
                }
            }
            else
            {
                transform.SetParent(null);
            }


            RaycastHit2D tox = Physics2D.Raycast(transform.position, -Vector2.up, 1f, toxic);
            if (tox.collider != null)
            {
                StartCoroutine(Die());
            }

            RaycastHit2D tox2 = Physics2D.Raycast(transform.position, -Vector2.down, 0.8f, toxic);
            if (tox2.collider != null)
            {
                StartCoroutine(Die());
            }


            RaycastHit2D dinoright = Physics2D.Raycast(transform.position, -Vector2.right, 1f, dino);
            RaycastHit2D dinoleft = Physics2D.Raycast(transform.position, -Vector2.left, 1f, dino);
            if (dinoleft.collider != null && dinoright.collider != null && !DinoController.Vulnerable && !godmode)
            {
                StartCoroutine(Die());
            }
            if (dinoleft.collider != null && dinoright.collider != null && transform.position.x > 421 && !godmode)
            {
                StartCoroutine(Die());
            }

            if (dinoleft.collider != null && dinoright.collider != null && transform.position.x < 416 && DinoController.Vulnerable && isAttacking)
            {

                godmode = true;
                DinoController.Vulnerable = false;
                DinoController.gotHit = true;
            }

            if (transform.position.x < 413.5 || transform.position.x > 417)
            {
                godmode = false;
            }


            if (Final_Start == null)
            {
                if (transform.position.x > 419 && transform.position.y < 136)
                {
                    DinoController.isAttackable = true;
                }
            }


            moveDirection.y -= gravity * Time.deltaTime;    // aplica a gravidade
            characterController.move(moveDirection * Time.deltaTime);   // move personagem	

            flags = characterController.collisionState;     // recupera flags
            isGrounded = flags.below;               // define flag de chão


            if (Input.GetAxis("Vertical") < 0 && moveDirection.x == 0)
            {
                if (!isDucking)
                {
                    boxCollider.size = new Vector2(boxCollider.size.x, 2 * colliderSizeY / 3);
                    boxCollider.offset = new Vector2(boxCollider.offset.x, colliderOffsetY - colliderSizeY / 6);
                    characterController.recalculateDistanceBetweenRays();
                }
                isDucking = true;
            }
            else
            {
                if (isDucking)
                {
                    boxCollider.size = new Vector2(boxCollider.size.x, colliderSizeY);
                    boxCollider.offset = new Vector2(boxCollider.offset.x, colliderOffsetY);
                    characterController.recalculateDistanceBetweenRays();
                    isDucking = false;
                }
            }

            if (num_coins == 6)
            {
                hasCollectedAll = true;
            }

            if ((Input.GetKeyDown(KeyCode.Z) == true) && hasCollectedAll)
            {
                isAttacking = true;
                AudioSource.PlayClipAtPoint(coin, this.gameObject.transform.position);
            }
            else
            {
                isAttacking = false;
            }
        }
        else
        {
            SceneManager.LoadScene("Try Again");
        }
    }
}
