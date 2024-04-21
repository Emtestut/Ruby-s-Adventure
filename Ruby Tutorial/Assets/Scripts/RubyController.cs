using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Runtime.Serialization;
using System.Diagnostics;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine.SceneManagement;

public class RubyController : MonoBehaviour
{
    public float speed = 3.0f;
    
    public int maxHealth = 5;
    public int maxSword = 1;
    
    public GameObject projectilePrefab;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI swordText;

    public TextMeshProUGUI overText;
    public GameObject gameOver;
    public ParticleSystem hitParticles;
    public ParticleSystem healParticles;
    public AudioClip throwSound;
    public AudioClip hitSound;

    public AudioClip gameOverSound;

    public AudioClip gameWonSound;
    
    public int health { get { return currentHealth; }}
    public int sword;
    int currentHealth;
    public int score;
    private string currentScene;
    public float timeInvincible = 2.0f;
    bool isInvincible;
    float invincibleTimer;
    
    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;
    private bool isWon = false;

    public GameObject backgroundMusic;

    Animator animator;
    Vector2 lookDirection = new Vector2(1,0);
    
    AudioSource audioSource;
    
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        
        currentHealth = maxHealth;
        sword = 0;
        backgroundMusic = GameObject.Find("BackgroundMusic");

        audioSource = GetComponent<AudioSource>();
        hitParticles.Stop();
        healParticles.Stop();
        currentScene = SceneManager.GetActiveScene().name;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isWon)
        {
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");
        
            Vector2 move = new Vector2(horizontal, vertical);
        
            if(!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
            {
                lookDirection.Set(move.x, move.y);
                lookDirection.Normalize();
            }
        
            animator.SetFloat("Look X", lookDirection.x);
            animator.SetFloat("Look Y", lookDirection.y);
            animator.SetFloat("Speed", move.magnitude);
        
            if (isInvincible)
            {
                invincibleTimer -= Time.deltaTime;
                if (invincibleTimer < 0)
                    isInvincible = false;
            }
        
            if(Input.GetKeyDown(KeyCode.C))
            {
                Launch();
            }
        
            if (Input.GetKeyDown(KeyCode.X))
            {
                RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
                if (hit.collider != null)
                {
                    NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                    if (character != null)
                    {
                        character.DisplayDialog();
                    }
                }
            }
        }
        else
        {
            if(Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(currentScene);
            }
        }
    }
    
    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
                return;
            
            isInvincible = true;
            invincibleTimer = timeInvincible;
            
            PlaySound(hitSound);
            hitParticles.Play();
        }
        else
        {
            healParticles.Play();
        }
        
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);

        if(currentHealth <= 0)
        {
            isWon = true;
            overText.text = "You lost! Press R to restart!";
            gameOver.SetActive(true);
            backgroundMusic.SetActive(false);
            PlaySound(gameOverSound);
            
        }
    }

   
    
    public void ChangeScore(int scoreAmount)
    {
       score = score + scoreAmount;
        
        scoreText.text = "Fixed Robots: " + score.ToString();
        if(score >= 4 && sword >= 1)
        {
            isWon = true;
            speed = 0;
            overText.text = "You Win! Game Created by Group #34\n" +
                            "Press R to Restart!";
            gameOver.SetActive(true);
            backgroundMusic.SetActive(false);
            PlaySound(gameWonSound);
        }
    }

    public void ChangeSword(int swordAmount)
   {
    sword = sword + swordAmount; 
    //Debug.Log("Sword count: " + sword); // Move this line to the next line
    swordText.text = "Sword Found " + sword.ToString();

    if(score >= 4 && sword >= 1)
        {
            isWon = true;
            speed = 0;
            overText.text = "You Win! Game Created by Group #34\n" +
                            "Press R to Restart!";
            gameOver.SetActive(true);
        }
   }


    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);

        animator.SetTrigger("Launch");
        
        PlaySound(throwSound);
    } 
    
    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}

