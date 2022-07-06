
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillEnemy : MonoBehaviour
{
    [SerializeField] private GameObject textGo;
    [SerializeField] private KeyCode killKey = KeyCode.Space;
    [SerializeField] private Vector3 targetOffset;
    [SerializeField] private CharacterController creator;
    [SerializeField] private GameObject[] weapons;

    private bool canKill;
    private Animator anim;
    public List<Enemy> selectedTargets;

    private void Start()
    {
        creator = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        ShowMessage();

        if(canKill && Input.GetKeyDown(killKey))
        {
            if (CheckEnemy())
            {
                StopAllCoroutines();
                StartCoroutine(GoToTarget(GetTransformToKill()));
            }                
        }
    }

    private IEnumerator GoToTarget(Transform target)
    {
        canKill = false;
        creator.killingEnemy = true;
        
        Vector3 newPos = target.position + targetOffset;
        StartCoroutine(creator.MoveToTarget(target.position + target.rotation*targetOffset, target.rotation));

        while(creator.movingToTarget)
            yield return null;

        ChangeWeapon(1);
        anim.SetTrigger("finish");
        yield return new WaitForSeconds(1.5f);
        ChangeWeapon(0);
        creator.killingEnemy = false;
        Enemy enemy = target.GetComponent<Enemy>();
        enemy.Kill();

        yield return new WaitForSeconds(1);
        canKill = CheckEnemy();
    }

    private Transform GetTransformToKill()
    {
        foreach(Enemy enemy in selectedTargets)
        {
            if (CheckEnemy(enemy))
                return enemy.transform;
        }

        return null;
    }
    private bool CheckEnemy(Enemy enemy)
    {
        if (enemy.isKilled)
            return false;

        Vector3 fromEnemy = creator.transform.position - enemy.transform.position;

        if (Vector3.Dot(enemy.transform.forward, fromEnemy) < 0)
            return true;     
       
        return false;
    }

    private bool CheckEnemy()
    {
        foreach (Enemy enemy in selectedTargets)
        {
            if (CheckEnemy(enemy))
                return true;
        }

        return false;
    }

    private void ShowMessage()
    {
        if (!canKill)
        {
            textGo.SetActive(false);
            return;
        }

        textGo.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Enemy")
        {
            Enemy enemy = other.GetComponent<Enemy>();

            if (enemy == null)
                return;

            if (!selectedTargets.Contains(enemy))
                selectedTargets.Add(enemy);

            canKill = CheckEnemy();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Enemy")
        {
            Enemy enemy = other.GetComponent<Enemy>();

            if (enemy == null)
                return;

            selectedTargets.Remove(enemy);
            ShowMessage();

            canKill = CheckEnemy();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        canKill = CheckEnemy();
    }

    private void ChangeWeapon(int num)
    {
        for(int i = 0; i < weapons.Length; i++)
        {
            if(i == num)
            {
                weapons[i].SetActive(true);
            }
            else
            {
                weapons[i].SetActive(false);
            }
        }
    }
}
