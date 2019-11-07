using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public Text enemyScoreText;

    NavMeshAgent agent;
    Transform destinationPosition;

    private int enemyScore;
    private PlayerController player;

    void Start()
    {
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        player = playerObj.GetComponent<PlayerController>();
        agent = GetComponent<NavMeshAgent>();
        enemyScore = 0;
        SetEnemyScoreText();
    }

    private void Update()
    {
        if (agent.enabled == true)
        {
            
            float distance = 100f;
            var pickUps = GameObject.FindGameObjectsWithTag("Pick Up");
            for (int i = 0; i < pickUps.Length; i++)
            {
                var pickUp = pickUps[i];
                float newDistance = Vector3.Distance(pickUp.transform.position, transform.position);
                if (newDistance < distance)
                {
                    destinationPosition = pickUp.transform;
                    distance = newDistance;
                }
            }

            var playerDistance = Vector3.Distance(player.transform.position, transform.position);
            if (playerDistance < distance)
            {
                if (player.getScore() > getScore())
                {
                    destinationPosition = player.transform;
                }
            }
            if (destinationPosition != null)
            {
                agent.SetDestination(destinationPosition.position);
            }
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pick Up"))
        {
            Object.Destroy(other.gameObject);
            enemyScore = enemyScore + 2;
            SetEnemyScoreText();
        }

        if (other.gameObject.CompareTag("Danger"))
        {
            Object.Destroy(other.gameObject);
            enemyScore = enemyScore - 1;
            SetEnemyScoreText();
        }
    }

    void OnCollisionEnter(Collision obj)
    {
        if (obj.gameObject.CompareTag("Player"))
        {
            player = obj.gameObject.GetComponent<PlayerController>();
            var forceVec = -obj.gameObject.GetComponent<Rigidbody>().velocity.normalized * 250f;
            if (player.getScore() > getScore())
            {
                obj.gameObject.GetComponent<Rigidbody>().AddForce(-forceVec, ForceMode.Force);
                enemyScore++;
            }
            else
            {
                agent.enabled = false;
                var rigidbody = GetComponent<Rigidbody>();
                rigidbody.isKinematic = false;
                rigidbody.AddForce(forceVec, ForceMode.Force);
                Invoke("EnableNavMesh", 1F);
                enemyScore--;
            }
            SetEnemyScoreText();
            
        }
    }

    void SetEnemyScoreText()
    {
        enemyScoreText.text = enemyScore.ToString();
    }

    public int getScore()
    {
        return enemyScore;
    }

    void EnableNavMesh()
    {
        agent.enabled = true;
        var rigidbody = GetComponent<Rigidbody>();
        rigidbody.isKinematic = true;
    }
}