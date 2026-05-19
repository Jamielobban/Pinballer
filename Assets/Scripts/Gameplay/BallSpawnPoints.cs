using UnityEngine;

public class BallSpawnPoints : MonoBehaviour
{
    [SerializeField] private Transform ghostRespawnPoint;

    public Transform GhostRespawnPoint => ghostRespawnPoint;
}