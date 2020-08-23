using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playController : MonoBehaviour
{
    // start() variables
    private Rigidbody2D rb;
    private Animator anim;
    private AudioSource footstep;

    // inspector variables
    [SerializeField]private LayerMask ground;
    [SerializeField]private float speed = 10f;
    [SerializeField]private float jump = 25f;
    [SerializeField]private int cherries = 0;
    [SerializeField]private Text cherryText;
    [SerializeField]private float hurtForce = 20f;
    [SerializeField]private AudioSource footStep;
    [SerializeField]private AudioSource collectingSound;
    [SerializeField]private Collider2D coll;
    [SerializeField]private Collider2D collFeet;

    // FSM
    private enum State {idle, running, jumping, falling, hurt};
    private State state = State.idle;

    private void Start(){
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();    
    }   
    private void Update(){
        if(state != State.hurt){
            // manage the input
        movementControl();
        }
        // change state
        animationState();
        anim.SetInteger("state", (int)state);
    }
    private void Jumpin()
    {
        rb.velocity = new Vector2(rb.velocity.x, jump);
        state = State.jumping;
    }
    private void movementControl(){
        float hDirection = Input.GetAxis("Horizontal");

        // moving left
        if(hDirection < 0){
            rb.velocity = new Vector2(-speed,rb.velocity.y);
            transform.localScale = new Vector2(-1,transform.localScale.y);
        }
        // moving right
        else if(hDirection > 0){
            rb.velocity = new Vector2(speed,rb.velocity.y);
            transform.localScale = new Vector2(1,transform.localScale.y);
        }
        // jumping
        if(Input.GetButton("Jump") && collFeet.IsTouchingLayers(ground)){
            Jumpin();
        }
    }
    private void animationState(){
        if(state == State.jumping){
            if(rb.velocity.y < .1f){
                state = State.falling;
            }
        }
        else if(!collFeet.IsTouchingLayers(ground))
        {
            state = State.falling;
        }
        else if(state == State.falling){
            if(collFeet.IsTouchingLayers(ground)){
                state = State.idle;
            }
        }
        else if(state == State.hurt){
            if(Mathf.Abs(rb.velocity.x) < .1f){
                state = State.idle;
            }
        }
        else if(Mathf.Abs(rb.velocity.x) > 2f){
            state = State.running;
        }
        else{
            state = State.idle;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision){
        if(collision.tag == "collectable")
        {
            collectingSound.Play();
            Destroy(collision.gameObject);
            cherries ++;
            cherryText.text = cherries.ToString();
        }
    }
    private void OnCollisionEnter2D(Collision2D other){
        if(other.gameObject.tag == "enemy"){

            enemy enemy = other.gameObject.GetComponent<enemy>();
            if(state == State.falling){
                enemy.JumpedOn();
                Jumpin();
            }
            else{
                state = State.hurt;
                if(other.gameObject.transform.position.x > this.transform.position.x){
                    //Enemy is to my right, should be damaged and move left
                    rb.velocity = new Vector2(-hurtForce, rb.velocity.y);
                }
                else{
                    //Enemy is to my left, should be damaged and move right
                    rb.velocity = new Vector2(hurtForce, rb.velocity.y);
                }
            }
        }
    }
    private void stepping()
    {
        footStep.Play();
    }
}
