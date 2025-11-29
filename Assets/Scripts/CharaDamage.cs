using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CharaDamage : MonoBehaviour, IDamageable
{
    //シリアル化している。charadataを指定。
    [SerializeField] public CharaData charadata;
    //シリアル化。SliderのHPゲージ指定
    [SerializeField] public Slider Slider;


    int HP;
    int MAXHP;
    int ATK;
    int DEF;
    int INT;
    int RES;

    void Start()
    {
        //charadataがnullでないことを確認
        if (charadata != null)
        {
            //valueのHPゲージのスライダーを最大の1に
            Slider.value = 1;


            //charadataの最大HPを代入。
            HP    = charadata.MAXHP;
            MAXHP = charadata.MAXHP;
            ATK   = charadata.ATK;
            DEF   = charadata.DEF;
            INT   = charadata.INT;
            RES   = charadata.RES;
        }
    }

    // ダメージ処理のメソッド　valueにはPlayer1のATKの値が入ってる
    public void Damage(int value)
    {
        // PlayerのATKからMazokusoldierのDEFを引いた値をHPから引く
        HP -= value - DEF;
        // HPゲージに反映
        Slider.value = (float)HP / (float)MAXHP;


        // HPが0以下ならDeath()メソッドを呼び出す。
        if (HP <= 0)
        {
            Death();
        }
    }
    // 死亡処理のメソッド
    public void Death()
    {
        // ゲームオブジェクトを破壊
        Destroy(gameObject);
    }
}
