using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class SkeletonBehavior : MonoBehaviour
{

    public Rigidbody2D playerRb;
    private Rigidbody2D skeletonPos;
    private Vector3 playerPos;
    private Vector3 currentPos;
    private bool playDead = false;
    private float playDeadTimer;
    private float playDeadTime = 3.0f;
    Animator animator;
    public AudioClip fallSound;
    AudioSource audioSource;
    public float speed = 1;


    // Start is called before the first frame update
    void Start()
    {
        skeletonPos = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!playDead)
        {
            playerPos = playerRb.position;
            currentPos = skeletonPos.position;
            skeletonPos.position = Vector2.MoveTowards(currentPos, playerPos, speed * Time.deltaTime);
        }
        else
        {
            playDeadTimer -= Time.deltaTime;
            if(playDeadTimer <= 0)
            {
                playDead = false;   
                speed = 1;
                animator.SetBool("IsDead",false);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        RubyController player = other.gameObject.GetComponent<RubyController >();

        if (player != null && playDead != true)
        {
            player.ChangeHealth(-1);
            playDead = true;
            speed = 0;
            playDeadTimer = playDeadTime;
            animator.SetBool("IsDead", true);
            PlaySound(fallSound);
        }
        
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
