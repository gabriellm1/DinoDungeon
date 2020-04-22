using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Prime31;

public class DinoController : MonoBehaviour
{

    
    private Animator animator;


    public static bool isAttackable;
    public static bool firstAttack;
    public static bool gotHit;
    public static bool Vulnerable;

    public bool isDead = false;

    public int Lives;

    public GameObject jogador;




    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        

        animator.SetBool("isAttackable", isAttackable);
        animator.SetBool("firstAttack", firstAttack);
        animator.SetBool("isDead", isDead);

        if (firstAttack)
        {
            isAttackable = true;
            firstAttack = false;
        }


        if (gotHit)
        {
            isAttackable = false;
            gotHit = false;
            Lives--;
        }
        else
        {
            if (transform.position.x > 419.22)
            {
                Vulnerable = true;
            }
        }


        if (animator.GetCurrentAnimatorStateInfo(0).IsName("backing"))
        {
            Vulnerable = false;
        }

        if (Lives == 0)
        {
            jogador.GetComponent<AudioSource>().Stop();
            isDead = true;
        }


        if (transform.localScale.x < 0.001)
        {
            SceneManager.LoadScene("FinalScene");
        }



    }
}



