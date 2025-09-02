using UnityEngine.AI;
using UnityEngine;

public class SimpleChase : MonoBehaviour
{
    NavMeshAgent agent;
    GameObject player;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (player != null)
        {
            agent.SetDestination(player.transform.position);
        }
    }
}
