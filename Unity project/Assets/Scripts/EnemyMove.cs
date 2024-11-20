using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    Rigidbody2D rigid;
    public int nextMove;
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        Invoke("Think", 5);
    }

    void FixedUpdate()
    {
        // Enemy 이동
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);
        // Platform Check
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.5f, rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0)); // Ray를 그려줌
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform")); // 바닥에 닿았을 때만
        if(rayHit.collider == null){
            // Ray에 닿은 물체가 없을 때
            nextMove *= -1;
            CancelInvoke();
            Invoke("Think", 5);
        }
    }

    void Think()
    {
        nextMove = Random.Range(-1, 2);
        Invoke("Think", 5);
    }
}
