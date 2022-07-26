using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRound : Enemy, IDamageable {

    public float torque;

    // Override collider size for round version
    protected override void StartCollider()
    {
        m_collider_size = ((CircleCollider2D)m_collider).radius;
    }

    protected override void FixedUpdate()
    {
        // Call Update of parent
        base.FixedUpdate();

        // Add constant torque
        m_rigidbody.AddTorque(torque);

    }
}
