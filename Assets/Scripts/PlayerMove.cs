using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMove : MonoBehaviour
{
    [SerializeField] float runSpeed = 8f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] Animator playerAnimator;
    [SerializeField] GameObject bullet;
    [SerializeField] Transform gun;
    CapsuleCollider2D myBodyCollider;
    float gravityScaleAtStart;
    BoxCollider2D myFeetCollider;
    Animator myAnimator;
    Vector2 moveInput;
    bool isAlive = true;
    Rigidbody2D myRigidbody;
    
    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponentInChildren<Rigidbody2D>();
        playerAnimator = GetComponentInChildren<Animator>();
        myBodyCollider = GetComponentInChildren<CapsuleCollider2D>();
        myFeetCollider = GetComponent<BoxCollider2D>();
        gravityScaleAtStart = myRigidbody.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAlive)
        {
            return;
        }
        Run();
        FlipSprite();
        ClimbLadder();
        Die();
    }

    void OnMove(InputValue value)
    {
        if (!isAlive)
        {
            return;
        }
        moveInput = value.Get<Vector2>();
        Debug.Log($"OnMove {moveInput}");
    }

    void OnFire(InputValue value)
    {
        if (!isAlive)
        {
            return;
            
        }

        Instantiate(bullet,gun.position, transform.rotation);
    }

    void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(myRigidbody.velocity.x),1f);
        }
    }
    void Run()
    {
        Vector2 placeVelocity = new Vector2(moveInput.x * runSpeed, myRigidbody.velocity.y);
        myRigidbody.velocity = placeVelocity;
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        playerAnimator.SetBool("isRunning",playerHasHorizontalSpeed);
    }

    void OnJump(InputValue value)
    {
        if (!isAlive)
        {
            return;
        }
        if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask(("Ground"))))
        {
            return;
        }
        if(value.isPressed)
        {
            //do stuff
            myRigidbody.velocity += new Vector2(0f, jumpSpeed);
            Debug.Log("jump");
        }
    }

    void ClimbLadder()
    {
        if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask(("Climbing"))))
        {
            myRigidbody.gravityScale = gravityScaleAtStart;
            playerAnimator.SetBool("Climb",false);
            return;
        }
        Vector2 climbVelocity = new Vector2(myRigidbody.velocity.x, moveInput.y*climbSpeed);
        myRigidbody.velocity = climbVelocity;
        myRigidbody.gravityScale = 0f;
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.y) > Mathf.Epsilon;
        playerAnimator.SetBool("Climb",playerHasHorizontalSpeed);
    }

    void Die() {
        if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemies","Hazards")))
        {
            isAlive = false;
            playerAnimator.SetTrigger("Dying");
            myRigidbody.velocity = new Vector2(20f,15f);
            FindObjectOfType<GameSession>().ProcessPlayerDeath();
        }
    }

}
