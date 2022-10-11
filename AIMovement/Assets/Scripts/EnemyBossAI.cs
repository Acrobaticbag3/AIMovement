using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBossAI : MonoBehaviour {
    //Transform that NPC has to follow
    public Transform transformToFollow;
 
    //NavMesh Agent variable
    NavMeshAgent agent;

    public float enemyDistance = 5.0f;

    // Start is called before the first frame update
    void Start() {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = enemyDistance;
    }
 
    // Update is called once per frame
    void Update() {
        float distance = Vector3.Distance(a: transform.position, b: agent.transform.position);
        agent.SetDestination(new Vector3(0,0,0));
        if (distance < enemyDistance) {
            //Follow the player
            Vector3.Distance(a: transform.position, b: transformToFollow.transform.position);
        }
    }
}
