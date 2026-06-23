using Fusion;
using UnityEngine;

public class TrapsManager : NetworkBehaviour
{
    public float trapSpawnIntervals;
    [SerializeField] GameObject trapPrefab;
    
    [Networked]
    [field:SerializeField]
    public float nextTrapSpawnTime { get; set; }

    private bool initialized = false;

    [SerializeField] private Vector2 spawnAreaMin = new Vector2(-5, -5);
    [SerializeField] private Vector2 spawnAreaMax = new Vector2(5, 5);
    [SerializeField] private Color gizmoColor = Color.green;

    public override void Spawned()
    {
        base.Spawned();
        initialized = true;
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if (!initialized || !Runner.IsSharedModeMasterClient)
            return;
        
        nextTrapSpawnTime += Runner.DeltaTime;
        if (nextTrapSpawnTime > trapSpawnIntervals)
        {
            nextTrapSpawnTime = 0;

            Vector3 randomPosition = GetRandomPositionInXZRectangle();

            Runner.Spawn(trapPrefab, randomPosition, Quaternion.identity, Object.InputAuthority);
        }
    }

    private Vector3 GetRandomPositionInXZRectangle()
    {
        float randomX = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
        float randomZ = Random.Range(spawnAreaMin.y, spawnAreaMax.y);

        return new Vector3(randomX, transform.position.y, randomZ);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;

        Vector3 bottomLeft = new Vector3(spawnAreaMin.x, transform.position.y, spawnAreaMin.y);
        Vector3 bottomRight = new Vector3(spawnAreaMax.x, transform.position.y, spawnAreaMin.y);
        Vector3 topLeft = new Vector3(spawnAreaMin.x, transform.position.y, spawnAreaMax.y);
        Vector3 topRight = new Vector3(spawnAreaMax.x, transform.position.y, spawnAreaMax.y);

        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bottomLeft);
    }
}