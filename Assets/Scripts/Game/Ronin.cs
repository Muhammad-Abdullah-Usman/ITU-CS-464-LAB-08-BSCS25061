using UnityEngine;

public class Ronin : MonoBehaviour
{
    [SerializeField] Health player;
    [SerializeField] float speed = 2.2f;
    [SerializeField] float attackRange = 1.2f;
    [SerializeField] float pushBack = 3f;

    void Update()
    {
        if (MissionGame.Instance == null || !MissionGame.Instance.IsPlaying) return;

        Vector3 target = player.transform.position;
        target.y = transform.position.y;
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < attackRange)
            player.TakeDamage(1);
    }

    public void Stagger()
    {
        Vector3 away = transform.position - player.transform.position;
        away.y = 0f;

        Vector3 next = transform.position + away.normalized * pushBack;
        next.x = Mathf.Clamp(next.x, -13f, 13f);
        next.z = Mathf.Clamp(next.z, -13f, 13f);
        transform.position = next;
    }
}