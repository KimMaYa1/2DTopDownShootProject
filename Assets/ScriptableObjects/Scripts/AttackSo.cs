using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="DefaultAttackData", menuName ="TopDownController/Attacks/Default", order = 0)]
public class AttackSo : ScriptableObject
{
    [Header("Attack Info")]
    public float size;
    public float delay;
    public float power;
    public float speed;
    public LayerMask target;

    [Header("Knock back info")]
    public bool isOnKnockback;
    public float knockbackPower;
    public float knockbackTime;
}
