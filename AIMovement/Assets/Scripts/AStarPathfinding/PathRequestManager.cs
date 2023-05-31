using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathRequestManager : MonoBehaviour
{
    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    PathRequest currentPathRequest;

    static PathRequestManager instance;
    AStarPathfinding pathfinding;

    bool isProcessingPath;

    void Awake() {
        instance = this;
        pathfinding = GetComponent<AStarPathfinding>();
    }

    public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[]> callback) {
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
        instance.pathRequestQueue.Enqueue(newRequest);
        instance.TryProcessNext();
    }

    void TryProcessNext() {
        if (!isProcessingPath && pathRequestQueue.Count > 0) {
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;
            pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd, OnPathFound);
        }
    }

    void OnPathFound(Vector3[] path) {
		currentPathRequest.callback.Invoke(path);
		isProcessingPath = false;
		TryProcessNext();
	}

	public void FinishedProcessingPath(Vector3[] path, bool success) {
		currentPathRequest.callback.Invoke(path);
		isProcessingPath = false;
		TryProcessNext();
	}

    struct PathRequest {
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<Vector3[]> callback;

        public PathRequest(Vector3 start, Vector3 end, Action<Vector3[]> callback) {
            pathStart = start;
            pathEnd = end;
            this.callback = callback;
        }
    }
}