using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float speed = 6f;
    [SerializeField] float arenaHalfSize = 13f;

    Rigidbody body;
    Vector3 input;

    void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (MissionGame.Instance == null || !MissionGame.Instance.IsPlaying)
        {
            input = Vector3.zero;
            return;
        }

        input = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        input = Vector3.ClampMagnitude(input, 1f);

        if (input != Vector3.zero)
            transform.forward = input;
    }

    void FixedUpdate()
    {
        Vector3 next = body.position + input * speed * Time.fixedDeltaTime;
        next.x = Mathf.Clamp(next.x, -arenaHalfSize, arenaHalfSize);
        next.z = Mathf.Clamp(next.z, -arenaHalfSize, arenaHalfSize);
        body.MovePosition(next);
    }
}