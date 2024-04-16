using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCollectible : MonoBehaviour
{
    public AudioClip collectedClip;
    private RubyController rubyController; 

    void Start()
    {

        GameObject rubyControllerObject = GameObject.FindWithTag("RubyController");
        if (rubyControllerObject != null)
        {
            rubyController = rubyControllerObject.GetComponent<RubyController>();
        }
        if (rubyController == null)
        {
            Debug.LogError("Cannot find RubyController script!");
        }
    }


    
    void OnTriggerEnter2D(Collider2D other)
    {
        RubyController controller = other.GetComponent<RubyController>();

        if (controller != null)
        {
            if (controller.sword < controller.maxSword)
            {
                rubyController.ChangeSword(1);
                Destroy(gameObject);
            
                controller.PlaySound(collectedClip);
            }
        }

    }
}