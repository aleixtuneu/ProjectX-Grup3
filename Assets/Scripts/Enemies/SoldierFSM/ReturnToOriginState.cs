using UnityEngine;

public class ReturnToOriginState : EnemyState
{
    public ReturnToOriginState(EnemyController3D enemy) : base(enemy) { }

    public override void Enter()
    {
        Enemy.SetRunAnimation(true);
        Enemy.ClearTarget();
    }

    public override void Update()
    {
        // OnTriggerEnter will fire if the player re-enters during return,
        // which calls TransitionTo(ChaseState) directly on the controller —
        // no polling needed here.

        Vector3 toOrigin = Enemy.OriginPosition - Enemy.transform.position;

        if (toOrigin.magnitude > 0.05f)
        {
            Enemy.MoveTowards(Enemy.OriginPosition);

            Vector3 flatDir = new Vector3(toOrigin.x, 0f, toOrigin.z);
            if (flatDir.sqrMagnitude > 0.001f)
                Enemy.RotateTowards(Quaternion.LookRotation(flatDir));
        }
        else
        {
            // Snap position, smoothly restore original facing
            Enemy.transform.position = Enemy.OriginPosition;
            Enemy.RotateTowards(Enemy.OriginRotation);

            if (Quaternion.Angle(Enemy.transform.rotation, Enemy.OriginRotation) < 1f)
            {
                Enemy.transform.rotation = Enemy.OriginRotation;
                Enemy.TransitionTo(Enemy.IdleState);
            }
        }
    }

    public override void Exit()
    {
        Enemy.SetRunAnimation(false);
    }
}
