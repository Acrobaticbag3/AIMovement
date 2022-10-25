/*  
    The general BT architecture was written by Kevin Johansson.
    Info was gathered from: https://medium.com/geekculture/how-to-create-a-simple-behaviour-tree-in-unity-c-3964c84c060e

    This script helps to implement the 
    generic Behavior Tree architecture 
    (aka BT architecture).
*/

using BehaviorTree;
public class RussyBT : Tree {
    public UnityEngine.Transform[] waypoints;
   
    public static float speed = 2f;

    protected override Node SetupTree() {
        Node root = new TaskPatrol(transform: transform, waypoints: waypoints);
        return root;
    }
}
