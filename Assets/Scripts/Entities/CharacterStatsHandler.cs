using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatsHandler : MonoBehaviour
{
    [SerializeField] private CharacterStats baseStats;

    public CharacterStats CurrentStates { get; private set; }
    public List<CharacterStats> statsModifiers = new List<CharacterStats>();

    private void Awake()
    {
        UpdateCharacterStats();
    }

    private void UpdateCharacterStats()
    {
        AttackSo attackSo = null;
        if(baseStats.attackSo != null)
        {
            attackSo = Instantiate(baseStats.attackSo);
        }
        //�����ϸ鼭 �ʱ�ȭ�� ���� {}���
        CurrentStates = new CharacterStats { attackSo = attackSo };
        //�ش� �κ� �����δ� �ӽ� ����
        CurrentStates.statsChangeType = baseStats.statsChangeType;
        CurrentStates.maxHealth = baseStats.maxHealth;
        CurrentStates.speed = baseStats.speed;
    }
}
