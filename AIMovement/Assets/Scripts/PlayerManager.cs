using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {
    private int _healthpoints;

    public GameObject Bullet;
    public float BulletSpeed = 100f;
    private bool _isShooting;

    private void Awake() {
        _healthpoints = 30;
    }

    void Update() {
        _isShooting |= Input.GetMouseButtonDown(button: 0);
    }

    public bool TakeHit() {
        _healthpoints -= 10;
        bool isDead = _healthpoints <= 0;
        if (isDead) _Die();
        return isDead;
    }

    private void _Die() {
        Destroy(obj: gameObject);
    }

    void FixedUpdate() {
        if (_isShooting) {
            GameObject newBullet = Instantiate(original: Bullet, position: this.transform.position + new Vector3(x: 1, y: 0, z: 0), rotation: this.transform.rotation);

            Rigidbody BulletRB = newBullet.GetComponent<Rigidbody>();

            BulletRB.velocity = this.transform.forward *BulletSpeed;
        }

        _isShooting = false;
    }
}
