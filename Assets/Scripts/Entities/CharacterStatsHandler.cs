using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterStatsHandler : MonoBehaviour
{
    private const float MinAttackDelay = 0.03f;
    private const float MinAttackPower = 0.5f;
    private const float MinAttackSize = 0.4f;
    private const float MinAttackSpeed = 0.1f;

    private const float MinSpeed = 0.8f;

    private const int MinMaxHealth = 5;

    [SerializeField] private CharacterStats baseStats;

    public CharacterStats CurrentStates { get; private set; }
    public List<CharacterStats> statsModifiers = new List<CharacterStats>();

    private void Awake()
    {
        UpdateCharacterStats();
    }

    public void AddStatModifier(CharacterStats statModifier)
    {
        statsModifiers.Add(statModifier);
        UpdateCharacterStats() ;
    }

    public void RemoveStatModifier(CharacterStats statModifier)
    {
        statsModifiers.Remove(statModifier);
        UpdateCharacterStats() ;
    }

    private void UpdateCharacterStats()
    {
        AttackSo attackSo = null;
        if(baseStats.attackSo != null)
        {
            attackSo = Instantiate(baseStats.attackSo);
        }
        //생성하면서 초기화를 위해 {}사용
        CurrentStates = new CharacterStats { attackSo = attackSo };
        UpdateStats((a, b) => b, baseStats);
        if(CurrentStates.attackSo != null)
        {
            CurrentStates.attackSo.target = baseStats.attackSo.target;
        }

        foreach (CharacterStats modifier in statsModifiers.OrderBy(o => o.statsChangeType))
        {
            if(modifier.statsChangeType == StatsChangeType.Override)
            {
                UpdateStats((o, o1) => o1, modifier);
            }
            else if(modifier.statsChangeType == StatsChangeType.Add)
            {
                UpdateStats((o, o1) => o + o1, modifier);
            }
            else if(modifier.statsChangeType == StatsChangeType.Multiple)
            {
                UpdateStats((o, o1) => o * o1, modifier);
            }
        }

        LimitAllStats();
    }

    private void UpdateStats(Func<float, float, float> operation, CharacterStats newModifier)
    {
        CurrentStates.maxHealth = (int)operation(CurrentStates.maxHealth, newModifier.maxHealth);
        CurrentStates.speed = operation(CurrentStates.speed, newModifier.speed);
        
        if(CurrentStates.attackSo == null || newModifier.attackSo == null)
        {
            return;
        }

        UpdateAttackStats(operation, CurrentStates.attackSo,newModifier.attackSo);

        if(CurrentStates.attackSo.GetType() != newModifier.attackSo.GetType())
        {
            return;
        }

        switch (CurrentStates.attackSo)
        {
            case RangedAttackData _:
                ApplyRangedStats(operation, newModifier);
                break;
        }
    }

    private void UpdateAttackStats(Func<float, float, float> operation,AttackSo currentAttack, AttackSo newAttack)
    {
        if(currentAttack == null || newAttack == null)
        {
            return;
        }

        currentAttack.delay = operation(currentAttack.delay, newAttack.delay);
        currentAttack.power = operation(currentAttack.power, newAttack.power);
        currentAttack.size = operation(currentAttack.size, newAttack.size);
        currentAttack.speed = operation(currentAttack.speed, newAttack.speed);
    }

    private void ApplyRangedStats(Func<float, float, float> operation, CharacterStats newModifier)
    {
        RangedAttackData currentRangedAttacks = (RangedAttackData)CurrentStates.attackSo;

        if(!(newModifier.attackSo is RangedAttackData))
        {
            return;
        }

        RangedAttackData rangedAttackModifier = (RangedAttackData)newModifier.attackSo;
        currentRangedAttacks.multipleProjectilesAngel =
            operation(currentRangedAttacks.multipleProjectilesAngel, rangedAttackModifier.multipleProjectilesAngel);
        currentRangedAttacks.spread = operation(currentRangedAttacks.spread, rangedAttackModifier.spread);
        currentRangedAttacks.duration = operation(currentRangedAttacks.duration, rangedAttackModifier.duration);
        currentRangedAttacks.numberofProjectilesPerShot = Mathf.CeilToInt(operation(currentRangedAttacks.numberofProjectilesPerShot,
            rangedAttackModifier.numberofProjectilesPerShot));
        currentRangedAttacks.projectileColor = UpdateColor(operation, currentRangedAttacks.projectileColor, rangedAttackModifier.projectileColor);
    }

    private Color UpdateColor(Func<float, float, float> operation, Color currentColor, Color newColor)
    {
        return new Color(
            operation(currentColor.r, newColor.r),
            operation(currentColor.g, newColor.g),
            operation(currentColor.b, newColor.b),
            operation(currentColor.a, newColor.a));
    }

    private void LimitStats(ref float stat, float minVal)
    {
        stat = Mathf.Max(stat, minVal);
    }

    private void LimitAllStats()
    {
        if(CurrentStates == null || CurrentStates.attackSo == null)
        {
            return;
        }

        LimitStats(ref CurrentStates.attackSo.delay, MinAttackDelay);
        LimitStats(ref CurrentStates.attackSo.power, MinAttackPower);
        LimitStats(ref CurrentStates.attackSo.size, MinAttackSize);
        LimitStats(ref CurrentStates.attackSo.speed, MinAttackSpeed);
        LimitStats(ref CurrentStates.speed, MinSpeed);
        CurrentStates.maxHealth = Mathf.Max(CurrentStates.maxHealth, MinMaxHealth);
    }
}
