using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class TopDownMovement : MonoBehaviour
{
    private TopDownCharacterController _controller;
    private CharacterStatsHandler _stats;

    private Vector2 _movemnetDirection = Vector2.zero;
    private Rigidbody2D _rigidbody;

    private Vector2 _knockback = Vector2.zero;
    private float knockbackDuration = 0.0f;

    private void Awake()
    {
        _controller = GetComponent<TopDownCharacterController>();
        _stats = GetComponent<CharacterStatsHandler>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _controller.OnMoveEvent += Move;
    }

    private void FixedUpdate()
    {
        ApplyMovement(_movemnetDirection);
        if(knockbackDuration > 0.0f)
        {
            knockbackDuration -= Time.fixedDeltaTime;
        }
    }

    public void ApplyKnockback(Transform other, float power, float duration)
    {
        knockbackDuration = duration;
        _knockback = -(other.position - transform.position).normalized * power;
    }

    private void Move(Vector2 direction)
    {
        _movemnetDirection = direction;
    }

    private void ApplyMovement(Vector2 direction)
    {
        direction = direction * _stats.CurrentStates.speed;
        if (knockbackDuration > 0.0f)
        {
            direction += _knockback;
        }
        _rigidbody.velocity = direction;
    }
}
