using RPGCharacterAnims.Actions;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

using static UnityEditor.PlayerSettings;
using static UnityEngine.ParticleSystem;

public class Player : MonoBehaviour
{
    Animator anim;
    Rigidbody rb;
    RaycastHit zimen;

    float moveZ;
    float moveX;
    [SerializeField]
    int mspeed = 10;

    float normalSpeed = 6f; //通常の移動速度

    bool atGround = true;  //地面ついてる

    Transform GroundObject;
    Vector3 Groundtarget;   //地面ターゲット
    Vector3 raypos;
    float raydistance;
    Vector3 rayDirection = Vector3.down;
    RaycastHit rayinf;


    float jumpSpeed = 450f; // ジャンプ速度
    float dashSpeed = 1500f; //ダッシュ力
    float dashtime = 0.3f; //ダッシュ時間
    float dashcool = 1.5f; //ダッシュクールタイム
    bool dashkyoka = true; //ダッシュ許可


    Vector3 moveDirection = Vector3.zero; //移動方向
    Vector3 dashDirection = Vector3.zero; //ダッシュ移動方向

    Vector3 spos;   //開始位置
    Vector3 face;   //回転
    Vector3 Fmemory; //回転記憶
    Vector3 raykeisan; //光計算
    Vector3 rayhitpos; //光当たった位置

    float mx;   //マウス移動量x
    float my;   //マウス移動量y

    float xrotateSpeed = 10f;   //x回転速度
    float yrotateSpeed = 10f;   //y回転速度


    float upMax = 290f;   //上限界角度
    float downMax = 70f;   //下限界角度

    float attack;   //攻撃
    bool Combo = false;   //コンボ許容
    bool canattack = true;   //攻撃可能
    float ComboEndtime = 1f;   //コンボ終了時のスキの時間

    [SerializeField] Transform FootObjectR;
    [SerializeField] Transform FootObjectL;
    [SerializeField] GameObject TrailObject;
    [SerializeField] GameObject FireAttack1Object;
    [SerializeField] GameObject FireAttack2Object;

    ParticleSystem Fire1;   //ファイアアタック1のパーティクル
    private TrailRenderer locus;   //軌跡
    TrailRenderer locus2;   //軌跡2

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>(); //animatorのコンポーネントを取得
        rb = GetComponent<Rigidbody>(); //Rigidbodyのコンポーネントを取得

        // マウスカーソルを非表示にし、位置を固定
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        locus = TrailObject.GetComponent<TrailRenderer>();
        locus.emitting = false;

        Fire1 = FireAttack1Object.GetComponent<ParticleSystem>();
        Fire1.Stop();

        locus2 = FireAttack2Object.GetComponent<TrailRenderer>();
        locus2.emitting = false;

        spos = transform.position;
    }

    private void FixedUpdate()
    {
        Move();
    }
    // Update is called once per frame
    void Update()
    {
        Attack();
    }

    void Move()
    {
        // 移動設定
        face = transform.eulerAngles;
        Fmemory = transform.eulerAngles;
        face.x = 0f;

        face.z = 0f;

        transform.eulerAngles = face;

        //前後移動
        moveZ = (Input.GetAxis("Vertical"));
        //左右移動
        moveX = (Input.GetAxis("Horizontal"));


        moveDirection = new Vector3(moveX, 0, moveZ).normalized * normalSpeed * Time.deltaTime;

        this.transform.Translate(moveDirection.x, moveDirection.y, moveDirection.z);

        // 移動のアニメーション
        anim.SetFloat("MoveSpeed", moveDirection.magnitude);

        // ダッシュ
        if (Input.GetButtonDown("Fire3") && dashkyoka == true)
        {

            dashDirection = new Vector3(moveX, 0, moveZ).normalized;
            rb.AddForce(transform.TransformDirection(dashDirection) * dashSpeed, ForceMode.Impulse);
            dashkyoka = false;

            StartCoroutine(Dashowari());

        }

        transform.eulerAngles = Fmemory;


        // 地面判定とジャンプ
        raykeisan = transform.position;
        raykeisan.y += 0.5f;

        if (Physics.SphereCast(raykeisan, 0.3f, Vector3.down, out zimen, 0.6f))
        {
            anim.SetBool("Jump", false);
            if (Input.GetButtonDown("Jump"))
            {
                atGround = true;
                rb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
            }

        }
        else
        {
            atGround = false;
            anim.SetBool("Jump", true);
        }
        // マウス向き

        mx = Input.GetAxis("Mouse X");
        my = Input.GetAxis("Mouse Y");

        mx += mx * xrotateSpeed * Time.deltaTime * mspeed;
        my += my * yrotateSpeed * Time.deltaTime;

        transform.Rotate(0, mx, 0);

        face = transform.eulerAngles;

        if (face.x > downMax)
        {
            if (face.x > 180f)
            {

                if (upMax > face.x)
                {

                    face.x = upMax;
                }
            }
            else
            {
                face.x = downMax;
            }

        }

        face.z = 0f;

        transform.eulerAngles = face;

        //移動方向によりアニメーションの向き・停止決定
        float zmoveZ = Mathf.Abs(moveDirection.z);
        float zmoveX = Mathf.Abs(moveDirection.x);



        if (zmoveZ > 0f || zmoveX > 0f)
        {
            if (zmoveZ > zmoveX)
            {

                if (moveDirection.z >= 0f)
                {
                    anim.SetBool("Forward", true);
                    anim.SetBool("Back", false);
                    anim.SetBool("Right", false);
                    anim.SetBool("Left", false);
                }
                else
                {
                    anim.SetBool("Forward", false);
                    anim.SetBool("Back", true);
                    anim.SetBool("Right", false);
                    anim.SetBool("Left", false);
                }
            }
            else
            {
                if (moveDirection.x >= 0)
                {
                    anim.SetBool("Forward", false);
                    anim.SetBool("Back", false);
                    anim.SetBool("Right", true);
                    anim.SetBool("Left", false);
                }
                else
                {
                    anim.SetBool("Forward", false);
                    anim.SetBool("Back", false);
                    anim.SetBool("Right", false);
                    anim.SetBool("Left", true);
                }
            }
        }
        else
        {
            anim.SetBool("Forward", false);
            anim.SetBool("Back", false);
            anim.SetBool("Right", false);
            anim.SetBool("Left", false);
        }
    }

    void OnAnimatorIK()
    {


        if (atGround == true)
        {

            float idouryou = anim.GetFloat("MoveSpeed");

            raypos = FootObjectR.position;

            Physics.Raycast(raypos, rayDirection, out rayinf, 1f);

            raydistance = rayinf.distance;

            float raykeisan1 = raypos.y - raydistance + 0.12f;

            FootObjectR.position = new Vector3(raypos.x, raykeisan1, raypos.z);

            // 右足のIKを有効化する

            if (idouryou == 0)
            {
                anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1.0f);
            }
            else
            {
                anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0.02f);
            }

            anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1.0f);

            // 右足のIKのターゲットを設定する
            anim.SetIKPosition(AvatarIKGoal.RightFoot, FootObjectR.position);
            anim.SetIKRotation(AvatarIKGoal.RightFoot, FootObjectR.rotation);

            raypos = FootObjectL.position;

            Physics.Raycast(raypos, rayDirection, out rayinf, 1f);
            raydistance = rayinf.distance;
            raykeisan1 = raypos.y - raydistance + 0.11f;

            FootObjectL.position = new Vector3(raypos.x, raykeisan1, raypos.z);

            // 左足のIKを有効化する

            if (idouryou == 0)
            {
                anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1.0f);
            }
            else
            {
                anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0.02f);
            }

            anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1.0f);

            // 左足のIKのターゲットを設定する
            anim.SetIKPosition(AvatarIKGoal.LeftFoot, FootObjectL.position);
            anim.SetIKRotation(AvatarIKGoal.LeftFoot, FootObjectL.rotation);
        }
    }


    IEnumerator Dashowari()
    {
        // ダッシュを終わらせる  
        yield return new WaitForSeconds(dashtime);

        rb.linearVelocity = Vector3.zero;


        // ダッシュクールタイムだけ待機  
        yield return new WaitForSeconds(dashcool);
        dashkyoka = true;

    }

    IEnumerator ComboEnd()
    {
        // コンボ終了時待機  
        yield return new WaitForSeconds(ComboEndtime);
        canattack = true;

    }

    void Attack()
    {
        anim = GetComponent<Animator>();
        attack = anim.GetFloat("Attack");
        if (canattack == true)
        {
            if (Combo == true)
            {
                //通常攻撃1回以上時に右クリックでのファイアアタック処理。
                if (Input.GetMouseButtonDown(1))
                {
                    //攻撃時にダッシュとジャンプによる移動停止。

                    rb.linearVelocity = Vector3.zero;

                    anim.SetFloat("Attack", 10f);

                    canattack = false;
                    Combo = false;
                    Fire1 = FireAttack1Object.GetComponent<ParticleSystem>();
                    Fire1.Play();

                    return;
                }

                //コンボ中の通常攻撃処理。
                if (Input.GetMouseButtonDown(0))
                {
                    if (attack < 4)
                    {
                        //攻撃時にダッシュとジャンプによる移動停止。

                        rb.linearVelocity = Vector3.zero;
                        Combo = false;
                        attack += 1f;
                        anim.SetFloat("Attack", attack);

                    }
                }
            }
            else
            {
                //通常攻撃の最初の1回の処理。
                if (attack == 0)
                {

                    if (Input.GetMouseButtonDown(0))
                    {
                        //攻撃時にダッシュとジャンプによる移動停止。
                        rb = GetComponent<Rigidbody>(); //Rigidbodyのコンポーネントを取得
                        rb.linearVelocity = Vector3.zero;
                        anim.SetFloat("Attack", 1f);
                    }
                }

            }
        }
    }
    public void FootR()
    {
        // 足音入れる
    }


    public void FootL()
    {
        // 足音入れる
    }

    public void AttackStart()
    {
        //ファイアアタックか通常攻撃か条件分岐
        anim = GetComponent<Animator>();
        attack = anim.GetFloat("Attack");
        if (attack == 10f)
        {
            locus2.emitting = true;
        }
        if (attack > 0.0f && attack < 5.0f)
        {
            locus.emitting = true;
        }
    }

    public void Hit()
    {
        attack = anim.GetFloat("Attack");
        if (attack == 10f)
        {
            locus2.emitting = false;
        }
        else
        {
            Combo = true;
            locus.emitting = false;
        }
    }

    public void AttackEnd()
    {
        //攻撃アニメーション終了時の処理
        Combo = false;

        attack = anim.GetFloat("Attack");
        anim.SetFloat("Attack", 0f);
        //通常攻撃4回目終了
        if (attack == 4f)
        {
            anim.SetFloat("Attack", 0f);
            canattack = false;
            StartCoroutine(ComboEnd());
        }

        //ファイアアタック終了
        if (attack == 10f)
        {

            Fire1.Stop();

            anim.SetFloat("Attack", 0f);

            StartCoroutine(ComboEnd());
        }
    }
}

