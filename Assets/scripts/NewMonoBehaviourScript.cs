using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public float speed = 5f;
    public float jump = 2f;
    public float gravity = -9.81f;
    [Tooltip("Seconds after leaving the ground in which a jump still counts (helps on bumpy terrain)")]
    public float coyoteTime = 0.15f;

    private CharacterController cc;
    private Vector3 velocity;
    private float lastGroundedTime = -999f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        // isGrounded is the result of the LAST cc.Move() call, so read it before moving this frame.
        bool grounded = cc.isGrounded;

        if (grounded)
        {
            lastGroundedTime = Time.time;
            if (velocity.y < 0)
            {
                // Press into the ground hard enough to follow the steepest walkable slope when walking downhill.
                velocity.y = -Mathf.Max(2f, speed * Mathf.Tan(cc.slopeLimit * Mathf.Deg2Rad));
            }
        }

        if (Input.GetButtonDown("Jump") && Time.time - lastGroundedTime <= coyoteTime)
        {
            velocity.y = Mathf.Sqrt(jump * -2f * gravity);
            lastGroundedTime = -999f; // no second jump inside the grace period
        }

        velocity.y += gravity * Time.deltaTime;

        Vector3 move = transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical");
        move = Vector3.ClampMagnitude(move, 1f); // W+A / W+D is no faster than W alone

        // One Move() per frame (walking + gravity together) so isGrounded stays reliable on flat ground.
        cc.Move((move * speed + velocity) * Time.deltaTime);
    }
}
