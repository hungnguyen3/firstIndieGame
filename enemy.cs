using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour
{
    private AudioSource destroy;
    protected Animator anim;
    protected Rigidbody2D rb;

    private void Death()
    {
        Destroy(this.gameObject);
    }

    protected virtual void Start(){
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        destroy = GetComponent<AudioSource>();
    }

    public void JumpedOn(){
        anim.SetTrigger("death");
        destroy.Play();
        rb.bodyType = RigidbodyType2D.Static;
        GetComponent<Collider2D>().enabled = false;
    }
}
