using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float maxSpeed; // 최대 이동속도
    public float jumpPower; // 점프력
    public float jumpCount; // 점프횟수

    public float dashSpeed; // 대시 속도
    private int maxDashCount = 3;      // 최대 대시 가능 횟수
    private int currentDashCount = 0; // 현재 사용한 대시 횟수
    private float dashDuration = 0.2f; // 대시 지속 시간
    private float dashTime; // 대시 시간 추적
    private bool isDashing; // 대시 중인지 확인
    private bool canDash = true; // 대시가 가능한 상태인지 여부

    Rigidbody2D rigid; // 물리엔진을 위한 변수
    SpriteRenderer spriteRenderer; //방향전환을 위한 변수
    Animator anim; // 애니메이션을 위한 변수
    void Awake() // 초기화
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }
    void Start()
    {
        // 초기화 및 대시 충전 루프 시작
        StartCoroutine(RechargeDash());
    }

    // 물리현상을 제외한 나머지 처리
    private void Update()
    {
        //방향키 입력이 없을 때
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
        }

        // 방향키 입력이 있을 때
        if (Input.GetButton("Horizontal"))
        {
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1; // 방향전환
        }

        // 멈췄을 때, Mathf.Abs는 절대값을 반환 // Mathf : 수학 관련 함수를 제공하는 클래스
        if (Mathf.Abs(rigid.velocity.x) < 0.4)
        {
            anim.SetBool("isWalking", false);
        }
        // 이동할 때
        else
        {
            anim.SetBool("isWalking", true);
        }

        // 점프(더블 점프)
        if (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.UpArrow) && jumpCount < 2)
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            jumpCount++;
            anim.SetBool("isJumping", true);
        }

        // 대시 입력 처리
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing) // Shift 키 입력 시 대시 시작
        {
            StartDash();
        }
    }

    void FixedUpdate()
    {
        // 대시 중에는 이동하지 않음
        if (isDashing)
        {
            return;
        }

        // 플레이어 이동
        float h = Input.GetAxis("Horizontal");

        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        if (rigid.velocity.x > maxSpeed) // 오른쪽 이동속도 제한
        {
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        }
        else if (rigid.velocity.x < maxSpeed * (-1)) // 왼쪽 이동속도 제한
        {
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);
        }

        // 점프 후 착지
        if (rigid.velocity.y < 0)
        {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0)); // 에디터 상에서 Ray를 그려줌
            // 바닥(Platform)에 닿았을 때만 // GetMask() : 레이어 이름에 해당하는 정수값을 리턴
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));
            if (rayHit.collider != null) // Ray에 닿은 물체가 있을 때, RayCastHit : 변수의 콜라이더로 검색 확인 가능
            {
                if (rayHit.distance < 0.3f) // Ray에 닿은 물체와의 거리가 0.3f(플레이어의 절반)보다 작을 때
                {
                    jumpCount = 0;
                    anim.SetBool("isJumping", false);
                }
            }
        }
    }
    IEnumerator RechargeDash()
    {
        while (true) // 무한 루프를 돌며 대시를 충전
        {
            yield return new WaitForSeconds(3f); // 3초 대기

            if (currentDashCount < maxDashCount) // 최대 대시 수를 초과하지 않는 경우만 충전
            {
                currentDashCount++;
                Debug.Log("대시 충전됨!");
            }
        }
    }
    void StartDash()
    {
        if (!canDash || currentDashCount <= 0)
        {
            return; // 대시가 불가능하면 대시를 시작하지 않음
        }
        isDashing = true;
        currentDashCount--; // 대시 횟수 증가
        Debug.Log("대시 시작!");
        Debug.Log(currentDashCount);
        dashTime = dashDuration; // 대시 지속 시간 초기화

        // 플레이어의 이동 방향에 따른 대시 방향 결정
        int dashDirection = (int)Input.GetAxisRaw("Horizontal");
        rigid.velocity = new Vector2(dashDirection * dashSpeed, rigid.velocity.y); // y 속도 유지하면서 x 방향 대시

        // anim.SetBool("isDashing", true); // 필요 시 대시 애니메이션 실행

        // 일정 시간이 지나면 대시 종료
        Invoke("EndDash", dashDuration);

        // 대시 횟수가 최대 횟수를 초과하면 쿨타임 시작
        if (currentDashCount == 0)
        {
            Debug.Log("대시 다 씀!");
        }
    }

    void EndDash()
    {
        isDashing = false; // 대시 상태 해제
        rigid.velocity = Vector2.zero; // 대시 후 멈추기
        Debug.Log("대시 종료!");

        // anim.SetBool("isDashing", false); // 필요 시 대시 애니메이션 종료

        // 쿨타임 중 대시가 가능하도록
        if (currentDashCount < maxDashCount)
        {
            canDash = true;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Item") // 아이템에 닿을 때
        {
            // 점수 획득


            collision.gameObject.SetActive(false); // 아이템 사라짐(먹음)
        }
    }
}
