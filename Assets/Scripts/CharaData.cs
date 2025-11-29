using RPGCharacterAnims.Actions;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Create StatusData")]
public class CharaData : ScriptableObject
{
    public string NAME; // ƒLƒƒƒ‰E“G–¼
    public int MAXHP; // Å‘åHP
    public int MAXMP; // Å‘åMP
    public int ATK; // UŒ‚—Í
    public int DEF; // –hŒä—Í
    public int INT; // –‚—Í
    public int RES; // –‚–@’ïR—Í
    public int AGI; // ˆÚ“®‘¬“x
    public int LV; // ƒŒƒxƒ‹
    public int GETEXP; // æ“¾ŒoŒ±’l
    public int GETGOLD; // æ“¾‚Å‚«‚é‚¨‹à
    public float ShortAttackRange; // ‹ßÚUŒ‚‚ğs‚¦‚é‹——£
    public float Enemytime; // s“®‚ğ”»’f‚·‚éˆ—‚ğs‚¤ŠÔŠu
}
