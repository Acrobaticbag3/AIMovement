using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarAgent : MonoBehaviour
{
    private Transform target;
    public float searchRadius = 10f; // Maximum radius to search for an endpoint
    public float speed = 10f;
    private Vector3[] path;
    private int targetIndex;
    public int rotationSpeed = 3;
    private Vector3 targetOldPosition;
    public float pathUpdateDelay = 0.5f; // Delay between path updates

    private bool isUpdatingPath = false; // Flag to prevent multiple path update requests
    private bool isFollowingPath = false; // Flag to track if the agent is currently following a path

    private void Awake()
    {
        targetOldPosition = transform.position; // Set initial targetOldPosition to agent's position
    }

    private void Update()
    {
        if (target != null && Vector3.Distance(targetOldPosition, target.position) > 3f && !isUpdatingPath && !isFollowingPath)
        {
            StartCoroutine(UpdatePathWithDelay());
        }
    }

    private IEnumerator UpdatePathWithDelay()
    {
        isUpdatingPath = true;

        // Calculate a random endpoint within the search radius
        Vector3 randomOffset = Random.insideUnitSphere * searchRadius;
        Vector3 randomTargetPosition = target.position + randomOffset;

        PathRequestManager.RequestPath(transform.position, randomTargetPosition, OnPathFound);
        targetOldPosition = target.position;

        yield return new WaitForSeconds(pathUpdateDelay);

        isUpdatingPath = false;
    }

    public void OnPathFound(Vector3[] newPath, bool pathIsSuccessful)
    {
        if (pathIsSuccessful)
        {
            path = newPath;
            targetIndex = 0;
            StartCoroutine("FollowPath");
        }
    }

    private IEnumerator FollowPath()
    {
        isFollowingPath = true;

        while (targetIndex < path.Length)
        {
            Vector3 currentWaypoint = path[targetIndex];

            while (transform.position != currentWaypoint)
            {
                Vector3 targetDir = currentWaypoint - transform.position;
                float step = rotationSpeed * Time.deltaTime;
                Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0F);
                transform.rotation = Quaternion.LookRotation(newDir);

                transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);

                yield return null;
            }

            targetIndex++;
        }

        isFollowingPath = false;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        targetOldPosition = transform.position; // Update the targetOldPosition when setting a new target
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