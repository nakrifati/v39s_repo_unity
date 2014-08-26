using UnityEngine;
using System.Collections;

/** AI controller specifically made for the spider robot.
 * The spider robot (or mine-bot) which is got from the Unity Example Project
 * can have this script attached to be able to pathfind around with animations working properly.\n
 * This script should be attached to a parent GameObject however since the original bot has Z+ as up.
 * This component requires Z+ to be forward and Y+ to be up.\n
 * 
 * It overrides the AIPath class, see that class's documentation for more information on most variables.\n
 * Animation is handled by this component. The Animation component refered to in #anim should have animations named "awake" and "forward".
 * The forward animation will have it's speed modified by the velocity and scaled by #animationSpeed to adjust it to look good.
 * The awake animation will only be sampled at the end frame and will not play.\n
 * When the end of path is reached, if the #endOfPathEffect is not null, it will be instantiated at the current position. However a check will be
 * done so that it won't spawn effects too close to the previous spawn-point.
 * \shadowimage{mine-bot.png}
 */
[RequireComponent(typeof(Seeker))]
public class MineBotAIMouse : AIPath
{
    /** Minimum velocity for moving */
    public float sleepVelocity = 0.4F;

    /** Speed relative to velocity with which to play animations */
    public float animationSpeed = 0.2F;

    /** Effect which will be instantiated when end of path is reached.
     * \see OnTargetReached */
    public GameObject endOfPathEffect;

    Animator animator;

    public GameObject player;

    public new void Start()
    {
        animator = tr.FindChild("Rat_1").GetComponent<Animator>();
        base.Start();
    }

    /** Point for the last spawn of #endOfPathEffect */
    protected Vector3 lastTarget;

    public override void OnTargetReached()
    {
        if (endOfPathEffect != null && Vector3.Distance(tr.position, lastTarget) > 1)
        {
            GameObject.Instantiate(endOfPathEffect, tr.position, tr.rotation);
            lastTarget = transform.position;
        }
    }

    public override Vector3 GetFeetPosition()
    {
        return transform.position;
    }

    Vector3 velocity;
    protected new void Update()
    {
        if (Time.timeScale > 0)
        {
            if (canMove)
            {
                velocity = CalculateVelocity(transform.position);
                float angle = Mathf.Atan2(targetDirection.x, targetDirection.z) * 180.0f / 3.14159f;

                if (targetDirection != Vector3.zero)
                    RotateTowards(targetDirection);

                animator.SetFloat("Speed", rigidbody.velocity.magnitude, 0.05f, Time.deltaTime);
            }
            else
            {
                velocity = Vector3.zero;
                animator.SetFloat("Speed", 0, 0.05f, Time.deltaTime);
			
            }
        }
    }

    void FixedUpdate()
    {
        if (Time.timeScale > 0 && canMove)
        {
            rigidbody.AddForce(transform.forward * velocity.magnitude * 5);
        }
    }

    void OnCollisionStay()
    {
        canMove = true;
    }

    void OnCollisionExit()
    {
        canMove = false;
    }
}
