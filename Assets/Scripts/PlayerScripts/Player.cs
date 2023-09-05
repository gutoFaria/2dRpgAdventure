using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class Player : Entity
{
   // private Rigidbody2D rb;
    //private Animator anim;

    [Header("Actions Info")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;

    [Header("Dash info")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashDuration;
    private float dashTime;

    [SerializeField] private float dashCooldown;
    private float dashCooldownTimer;

    [Header("Attack info")]
    [SerializeField] private float comboTime = .3f;
    private float comboTimeWindow;
    private bool isAttack;
    private int comboAttack = 0;

    private float xInput;
    

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        CheckInput();
        Movement();
        //CollisionChecks();

        dashTime -= Time.deltaTime;
        dashCooldownTimer -= Time.deltaTime;
        comboTimeWindow -= Time.deltaTime;

        FlipController();
        AnimatorController();
    }

    private void FlipController()
    {
        if(rb.velocity.x > 0 && !facingRight)
            Flip();
        else if(rb.velocity.x < 0 && facingRight)
            Flip();
    }

    #region ENDATTACK
    public void AttackOver()
    {
        isAttack = false;

        comboAttack++;

        if(comboAttack > 2)
            comboAttack = 0;  
    }
    #endregion

    #region INPUTS
    private void CheckInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");

        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            StartAttackEvent();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
           Jump();
        }

        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            DashAbility();
        }
    }
    #endregion

    #region ATTACK
    private void StartAttackEvent()
    {
        if(!isGrounded)
            return;

        
        if (comboTimeWindow < 0)
            comboTimeWindow = 0;

        isAttack = true;
        comboTimeWindow = comboTime;
    }
    #endregion

    #region DASH 
    private void DashAbility()
    {
        if(dashCooldownTimer < 0 && !isAttack)
        {
            dashCooldownTimer = dashCooldown;
            dashTime = dashDuration;
        }
    }
    #endregion
    
    #region MOVEMENT
    private void Movement()
    {
        if(isAttack)
        {
            rb.velocity = new Vector2(0,0);
        }
        else if(dashTime > 0)
        {
            rb.velocity = new Vector2(facingDir * dashSpeed, 0);
        }
        else
        {
             //xInput = Input.GetAxisRaw("Horizontal");
            rb.velocity = new Vector2(xInput * moveSpeed,rb.velocity.y);
        }
       
    }
    #endregion
    
    #region JUMP
    private void Jump()
    {
        if(isGrounded)
            rb.velocity = new Vector2(rb.velocity.x,jumpForce);
    }
    #endregion
   
    #region ANIMATOR
    private void AnimatorController()
    {
        bool isMoving = rb.velocity.x !=0;
        anim.SetFloat("yVelocity",rb.velocity.y);

        anim.SetBool("isMoving",isMoving);
        anim.SetBool("isGrounded",isGrounded);
        anim.SetBool("isDashing",dashTime > 0);
        anim.SetBool("isAttack", isAttack);
        anim.SetInteger("comboAttack",comboAttack);
    }
    #endregion
        
}
