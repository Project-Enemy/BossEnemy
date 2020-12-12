using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossEnemy : MonoBehaviour
{
    NavMeshAgent agent;
    Animator animator;

    [SerializeField] GameObject target;
    [SerializeField] GameObject exp;//大技時呼び出されるオブジェクト
    float AttackCooltime = 1;//攻撃後の隙
    float LightAttack=1;//弱攻撃の隙
    float HeavyAttack = 3;//強攻撃の隙


    float JumpAttackCooltime = 5;//大技の現在のクールタイム
    float JumpAttack = 60;//大技の設定されるクールタイム
    int hp = 100;

    float distance;

    //=========パラメータ==========
    public int param = 100;//プレイヤーパラメータ
    public int score = 0;//敵討伐数
    //=============================

    int damageHash = Animator.StringToHash("damage");

    void Start()
    {
        
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        animator.speed = 2;
        agent.destination = target.transform.position;
    }


    void Update()
    {
        move();
    }

    void move()
    {
        distance = (transform.position - target.transform.position).sqrMagnitude;//プレイヤーとの距離計算
        if (hp > 0)
        {
            //プレイヤーとの距離が5以下かつクールタイムが終わっていたら攻撃へ移行
            if (distance <= 5 && AttackCooltime <= 0)
            {
                animator.SetBool("move", false);//idelへ移行
                //1～3の攻撃をランダムに実行(3の強攻撃だと隙が大きい)
                switch (Random.Range(1, 4))
                {
                    //弱攻撃
                    case 1:
                        AttackCooltime = LightAttack;
                        animator.SetTrigger("Attack1");
                        break;
                    case 2:
                        AttackCooltime = LightAttack;
                        animator.SetTrigger("Attack2");
                        break;
                    //強攻撃
                    case 3:
                        AttackCooltime = HeavyAttack;
                        animator.SetTrigger("Attack3");
                        break;
                    default:
                        break;
                }
            }

            //攻撃後のクールタイム
            if (AttackCooltime >= 0)
            {
                agent.destination = this.transform.position;
                AttackCooltime -= Time.deltaTime;
            }
            //追跡状態
            else
            {
                agent.destination = target.transform.position;
                animator.SetBool("move", true);//moveへ移行
                transform.LookAt(target.transform);


            }
            //大技(予定)
            if (JumpAttackCooltime >= 0)
            {
                JumpAttackCooltime -= Time.deltaTime;
            }
            else//発動
            {
                JumpAttackCooltime = JumpAttack;
                GameObject g = Instantiate(exp, target.transform.position, target.transform.rotation);//現在のプレイヤーの位置に爆発オブジェクトを設置
                animator.SetTrigger("Attack4");
            }
        }
        //死亡判定
        else
        {
            animator.SetBool("death", true);
            Destroy(gameObject, 5f);
        }

    }
//↓攻撃青エネミーから引っこ抜いたから残ってる部分、使う？↓
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("攻撃！");
        }
    }

    public void BlueEnemyDamage()
    {
        Debug.Log("damaged");
        animator.SetTrigger(damageHash);
    }

}
