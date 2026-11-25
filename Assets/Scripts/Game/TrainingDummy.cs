using UnityEngine;

public class TrainingDummy : MonoBehaviour
{
    [SerializeField] float arenaHalfSize = 12f;

    public void Hit()
    {
        // Jump to a new spot so the player keeps moving.
        float x = Random.Range(-arenaHalfSize, arenaHalfSize);
        float z = Random.Range(-arenaHalfSize, arenaHalfSize);
        transform.position = new Vector3(x, transform.position.y, z);
    }
}