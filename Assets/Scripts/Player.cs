using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float moveSpeed;

    private MapCreater mapCreater;

    // Start is called before the first frame update
    void Start()
    {
        mapCreater = GameObject.FindWithTag("MapCreater").GetComponent<MapCreater>();
    }

    // Update is called once per frame
    void Update()
    {
        // アニメーション切り替え
        AnimationChange();

        // キャラクター移動
        Move();
    }

    void FixedUpdate()
    {
        // プレイヤーの一定範囲のマップを読み込む
        mapCreater.CreateAround(this.transform.position.x, this.transform.position.y);

        // プレイヤーの一定範囲外のマップを消す
        mapCreater.DeleteAround(this.transform.position.x, this.transform.position.y);
    }

    private void Move()
    {
        var xPos = this.transform.position.x + Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
        var yPos = this.transform.position.y + Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;

        transform.position = new Vector3(xPos, yPos, 0f);
    }

    private void AnimationChange()
    {
        // 歩き切り替え
        SetWalking();

        // 反転
        SetDirection();
    }

    private void SetWalking()
    {
        if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f || Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f)
        {
            animator.SetBool("walk", true);
        }
        else
        {
            animator.SetBool("walk", false);
        }
    }

    private void SetDirection()
    {
        var horizon = Input.GetAxis("Horizontal");
        var scale = this.transform.localScale;

        if (horizon > 0.1f)
        {
            scale.x = 1;
        }
        else if (horizon < -0.1f)
        {
            scale.x = -1;
        }
        else
        {
            return;
        }
        transform.localScale = scale;
    }
}
