using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemyScript : MonoBehaviour
{
    public NavMeshAgent enemy;
    public Transform player;
    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        enemy = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        enemy.SetDestination(player.position);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            anim.SetTrigger("canAttack");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        anim.SetTrigger("walk");
    }

}