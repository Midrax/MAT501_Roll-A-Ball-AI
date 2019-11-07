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
    private RollerAgent player;

    void Start()
    {
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        player = playerObj.GetComponent<RollerAgent>();
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
                if (pickUps[i].transform.position.y > 0)
                {
                    var pickUp = pickUps[i];
                    float newDistance = Vector3.Distance(pickUp.transform.position, transform.position);
                    if (newDistance < distance)
                    {
                        destinationPosition = pickUp.transform;
                        distance = newDistance;
                    }
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
            Vector3 pos = other.gameObject.transform.position;
            pos.y = -0.5f;
            other.gameObject.transform.position = pos;
            enemyScore = enemyScore + 2;
            SetEnemyScoreText();
        }

        if (other.gameObject.CompareTag("Danger"))
        {
            Vector3 pos = other.gameObject.transform.position;
            pos.y = -0.5f;
            other.gameObject.transform.position = pos;
            enemyScore = enemyScore - 1;
            SetEnemyScoreText();
        }
    }

    void OnCollisionEnter(Collision obj)
    {
        if (obj.gameObject.CompareTag("Player"))
        {
            player = obj.gameObject.GetComponent<RollerAgent>();
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

    public void SetEnemyScoreText()
    {
        enemyScoreText.text = enemyScore.ToString();
    }

    public int getScore()
    {
        return enemyScore;
    }
    public void resetScore()
    {
        enemyScore = 0;
    }


    void EnableNavMesh()
    {
        agent.enabled = true;
        var rigidbody = GetComponent<Rigidbody>();
        rigidbody.isKinematic = true;
    }
}