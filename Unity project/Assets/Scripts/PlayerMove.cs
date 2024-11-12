using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float maxSpeed; // 최대 이동속도
    public float jumpPower; // 점프력
    public float jumpCount; // 점프횟수
    Rigidbody2D rigid; // 물리엔진을 위한 변수
    SpriteRenderer spriteRenderer; //방향전환을 위한 변수
    Animator anim; // 애니메이션을 위한 변수

    void Awake() // 초기화
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Update(){ // 물리현상을 제외한 나머지 처리
        //방향키 입력이 없을 때
        if(Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
        }

         // 방향키 입력이 있을 때
        if(Input.GetButton("Horizontal"))
        {
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1; // 방향전환
        }

         // 이동하지 않을 때, Mathf.Abs는 절대값을 반환
        if(Mathf.Abs(rigid.velocity.x) < 0.4)
        {
            anim.SetBool("isWalking", false);
        }
        // 이동할 때
        else
        {
            anim.SetBool("isWalking", true);
        }

        // 점프
        if(Input.GetButtonDown("Jump") && jumpCount < 2)
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            jumpCount++;
            anim.SetBool("isJumping", true);
        }
    }

    void FixedUpdate()
    {
        // 플레이어 이동
        float h = Input.GetAxis("Horizontal");

        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        if(rigid.velocity.x > maxSpeed) // 오른쪽 이동속도 제한
        {
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        }
        else if(rigid.velocity.x < maxSpeed * (-1)) // 왼쪽 이동속도 제한
        {
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);
        }

        if(rigid.velocity.y < 0){
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0)); // Ray를 그려줌
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform")); // 바닥에 닿았을 때만
            if(rayHit.collider != null) // Ray에 닿은 물체가 없을 때
            {
                if(rayHit.distance < 1.5f) // Ray에 닿은 물체와의 거리가 0.5f보다 작을 때
                {
                    jumpCount = 0;
                }
                anim.SetBool("isJumping", false);
            }
        }
    }
}
