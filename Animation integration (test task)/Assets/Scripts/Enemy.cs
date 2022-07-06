using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float destroyTime;

    private Rigidbody[] rigidBodies;
    private Animator anim;
    private Collider collider;

    public bool isKilled;

    public Action OnKill;

    private void Awake()
    {
        rigidBodies = GetComponentsInChildren<Rigidbody>();
        ChangeCondition(true);
        anim = GetComponentInChildren<Animator>();
        collider = GetComponent<Collider>();

        GetComponent<Rigidbody>().isKinematic = true;
    }

    private void ChangeCondition(bool enabled)
    {
        foreach (Rigidbody rb in rigidBodies)
            if (rb.gameObject != gameObject)
                rb.isKinematic = enabled;
            else
                rb.isKinematic = !enabled;
    }

    public void Kill()
    {
        isKilled = true;
        anim.enabled = false;
        ChangeCondition(false);
        collider.isTrigger = true;
        StartCoroutine(Destroy());
    }

    private IEnumerator Destroy()
    {
        yield return new WaitForSeconds(destroyTime);
        OnKill?.Invoke();
        yield return new WaitForEndOfFrame();
        Destroy(gameObject);
    }
}
