using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Missile : MonoBehaviour
{
    Rigidbody _rb;
    [SerializeField]
    private float _speed = 10f;
    [SerializeField]
    private float _trailRate = 200;
    [SerializeField]
    private GameObject _smokeTrail;
    [SerializeField]
    private GameObject _explosion;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        PlayerController.OnFired += OnMissileFired;
        Physics.IgnoreLayerCollision(6, 7, true);
        Physics.IgnoreLayerCollision(7, 7, true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Destructable"))
        {
            Instantiate(_explosion, collision.transform.position, Quaternion.identity);
            Destroy(collision.collider.gameObject);
        }

        PlayerController.OnFired -= OnMissileFired;
        Destroy(gameObject);
    }

    private void OnMissileFired()
    {
        if (isActiveAndEnabled)
        {
            _smokeTrail.SetActive(true);
            Vector3 forward = _rb.transform.forward;
            _rb.AddForce(forward * _speed, ForceMode.Impulse);
        }
    }
}
