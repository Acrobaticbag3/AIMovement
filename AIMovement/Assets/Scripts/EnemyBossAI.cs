using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBossAI : MonoBehaviour {
    public Transform transformToFollow; //Transform that NPC has to follow
    NavMeshAgent agent; //NavMesh Agent variable
    private float enemyStoppingDistance = 10.0f;

    // Start is called before the first frame update
    void Start() {
        agent = GetComponent<NavMeshAgent>();
    }
 
    // Update is called once per frame
    void Update() {
        float distance = Vector3.Distance(a: transform.position, b: agent.transform.position);
        agent.destination = transformToFollow.position;

        if (distance < enemyStoppingDistance) {
           Vector3.Distance(a: transform.position, b: transformToFollow.transform.position); //Follow the player
        }
        agent.stoppingDistance = enemyStoppingDistance;
    }
}
