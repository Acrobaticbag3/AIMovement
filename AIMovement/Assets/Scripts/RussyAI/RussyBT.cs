/*  
    The general BT architecture was written by Kevin Johansson.
    Info was gathered from: https://medium.com/geekculture/how-to-create-a-simple-behaviour-tree-in-unity-c-3964c84c060e

    This script helps to implement the 
    generic Behavior Tree architecture 
    (aka BT architecture).

    Additional info.
        BTW this is our BT for RussyAI (Ranged AI)
        This means that the Node root is quite 
        important. Thats because it's what 
        keeps that behavior tasks priorities.

*/

using System.Collections.Generic;
using BehaviorTree;
public class RussyBT : Tree {
    public UnityEngine.Transform[] waypoints;
   
    public static float speed = 5f;
    public static float fovRange = 6f; 
    public static float attackRange = 1f;

    protected override Node SetupTree() {

        // The root of our tree
        Node root = new Selector(new List<Node> {

            new Sequence(new List<Node> {
                new CheckEnemyInAttackRange(transform),
                new TaskAttack(transform),
            }),
            new Sequence(new List<Node> {
                new CheckEnemyInFOVRange(transform),
                new TaskGoToTarget(transform),
            }),
            
            new TaskPatrol(transform, waypoints),
        });

        return root;
    }
}
