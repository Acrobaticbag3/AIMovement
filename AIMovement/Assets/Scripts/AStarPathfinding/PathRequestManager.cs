using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using System.Threading.Tasks;

public class PathRequestManager : MonoBehaviour {
    private void Start() {
        
    }

    private void Update() {
        Thread t = new Thread(start: Start);
        
    }
    
}
