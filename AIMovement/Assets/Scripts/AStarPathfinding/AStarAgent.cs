using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarAgent : MonoBehaviour
{
    [SerializeField] private Transform target;
    public float searchRadius = 10f;
    float speed = 10;
    Vector3[] path;
    int targetIndex;
    int rotationSpeed = 3;
    private Vector3 targetOldPosition;

    void Awake()
    {
        targetOldPosition = target.position;
    }

    void Update() {
        if (Vector3.Distance(targetOldPosition, target.position) > 3) {
        Vector3 randomOffset = Random.insideUnitSphere * searchRadius;
        Vector3 randomTargetPosition = target.position + randomOffset;
        PathRequestManager.RequestPath(transform.position, randomTargetPosition, (newPath) => OnPathFound(newPath, true));
        targetOldPosition = target.position;
    }
    }

    public void OnPathFound(Vector3[] newPath, bool pathIsSuccessful)
    {
        if (pathIsSuccessful)
        {
            path = newPath;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator FollowPath()
    {
        if (targetIndex >= path.Length)
        {
            targetIndex = 0;
            path = new Vector3[0];
            yield break;
        }

        Vector3 currentWaypoint = path[targetIndex];

        while (true)
        {
            if (transform.position == currentWaypoint)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }

            Vector3 targetDir = currentWaypoint - transform.position;
            float step = rotationSpeed * Time.deltaTime;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDir);

            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
            yield return null;
        }
    }

    public void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one);

                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }
}