using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using UnityEngine.UI;

public class RollerAgent : Agent
{
    public float speed = 10;
    public Text playerScoreText;

    Rigidbody rBody;
    Transform Target;
    Transform closestDanger;
    List<GameObject> pickUpList;
    List<GameObject> dangerList;
    int playerScore;
    EnemyController enemy;
    int pickUpsTotal;

    void Start()
    {
        rBody = GetComponent<Rigidbody>();

        pickUpList = new List<GameObject>();
        dangerList = new List<GameObject>();

        var pickUpsArray = GameObject.FindGameObjectsWithTag("Pick Up");
        pickUpsTotal = pickUpsArray.Length;
        foreach ( Object pickUp in pickUpsArray)
        {
            pickUpList.Add((GameObject)pickUp);
        }

        var dangersArray = GameObject.FindGameObjectsWithTag("Danger");
        foreach (Object danger in dangersArray)
        {
            dangerList.Add((GameObject) danger);
        }

        var enemyObj = GameObject.FindGameObjectWithTag("Enemy");
        enemy = enemyObj.GetComponent<EnemyController>();
        enemy.resetScore();
        enemy.SetEnemyScoreText();

        playerScore = 0;
        SetPlayerScoreText();
        findTarget();
        findClosestDanger();
    }

    private void Update()
    {
        findTarget();
        findClosestDanger();
    }

    public override void AgentReset()
    {
        if (boardIsClear() || this.transform.position.y < 0)
        {
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
            this.transform.position = new Vector3(0, 0.5f, 1.5f);
            enemy.transform.position = new Vector3(0, 0.5f, -1.5f);

            enemy.resetScore();
            enemy.SetEnemyScoreText();

            playerScore = 0;
            SetPlayerScoreText();
            repositionGameObjects();

        }
    }

    public override void CollectObservations()
    {
        // Current Target, closest Danger, Agent and Enemy positions
        AddVectorObs(Target.position);
        AddVectorObs(closestDanger.position);
        AddVectorObs(this.transform.position);
        AddVectorObs(enemy.transform.position);

        // Agent velocity
        AddVectorObs(rBody.velocity.x);
        AddVectorObs(rBody.velocity.z);

        // RollerAgent Score and Enemy Score
        AddVectorObs(playerScore);
        AddVectorObs(enemy.getScore());
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.z = vectorAction[1];
        rBody.AddForce(controlSignal * speed);

        // Fell off platform, which doesn't happen in this version of the game, but still, could be useful in a variant.
        if (this.transform.position.y < 0)
        {
            SetReward(-1f);
            Done();
        }

        if (boardIsClear() || enemy.transform.position.y < 0)
        {
            if (getScore() > enemy.getScore())
            {
                AddReward(0.5f);
            }

            Done();
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pick Up"))
        {
            Vector3 pos = other.gameObject.transform.position;
            pos.y = -0.5f;
            other.gameObject.transform.position = pos;
            playerScore = playerScore + 2;
            SetPlayerScoreText();
            float reward = 0.5f / pickUpsTotal;
            AddReward(reward);
        }

        if (other.gameObject.CompareTag("Danger"))
        {
            Vector3 pos = other.gameObject.transform.position;
            pos.y = -0.5f;
            other.gameObject.transform.position = pos;
            playerScore = playerScore - 1;
            SetPlayerScoreText();
            float negativeReward = -0.25f / pickUpsTotal;
            AddReward(negativeReward);
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            enemy = other.gameObject.GetComponent<EnemyController>();
            if (enemy.getScore() > getScore())
            {
                playerScore++;
                float reward = 0.5f / pickUpsTotal;
                AddReward(reward);
            }
            else
            {
                playerScore--;
                float negativeReward = -0.25f / pickUpsTotal;
                AddReward(negativeReward);
            }
            SetPlayerScoreText();
        }
    }

    void findTarget()
    {
        // Calculate closest Pick Up Object
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
                    Target = pickUp.transform;
                    distance = newDistance;
                }
            }
        }

        // Check if closest Pick Up is closer then Player
        var playerDistance = Vector3.Distance(enemy.transform.position, transform.position);
        if (playerDistance < distance)
        {
            // If the enemy has a higher score, try to steal his points
            if (enemy.getScore() > getScore())
            {
                Target = enemy.transform;
            }
        }
    }

    void findClosestDanger()
    {
        float dangerDistance = 100f;
        var dangers = GameObject.FindGameObjectsWithTag("Danger");
        for (int i = 0; i < dangers.Length; i++)
        {
            if (dangers[i].transform.position.y > 0)
            {
                var danger = dangers[i];
                float newDistance = Vector3.Distance(danger.transform.position, transform.position);
                if (newDistance < dangerDistance)
                {
                    dangerDistance = newDistance;
                    closestDanger = dangers[i].transform;
                }
            }
        }

        // Check if closest Pick Up is closer then Player
        var playerDistance = Vector3.Distance(enemy.transform.position, transform.position);
        if (playerDistance < dangerDistance)
        {
            // If the enemy has a higher score, try to steal his points
            if (enemy.getScore() < getScore())
            {
                closestDanger = enemy.transform;
            }
        }
    }

    void SetPlayerScoreText()
    {
        playerScoreText.text = playerScore.ToString();
    }

    public int getScore()
    {
        return playerScore;
    }

    public void repositionGameObjects()
    {
        foreach (GameObject danger in dangerList)
        {
            Vector3 position = new Vector3(0, 0, 0);
            int overlappingColliders = 2;
            while (overlappingColliders > 1)
            {
                float x = Random.Range(-9, 9);
                float z = Random.Range(-8, 8);
                position = new Vector3(x, 0.5f, z);
                Collider[] hitColliders = Physics.OverlapSphere(position, 2f);
                overlappingColliders = hitColliders.Length;

            }
            danger.transform.position = position;
        }

        foreach (GameObject pickUp in pickUpList)
        {
            Vector3 position = new Vector3(0, 0, 0);
            int overlappingColliders = 2;
            while (overlappingColliders > 1)
            {
                float x = Random.Range(-9, 9);
                float z = Random.Range(-8, 8);
                position = new Vector3(x, 0.5f, z);
                Collider[] hitColliders = Physics.OverlapSphere(position, 0.5f);
                overlappingColliders = hitColliders.Length;

            }
            pickUp.transform.position = position;
        }
        
    }

    bool boardIsClear()
    {
        bool allBelow = true;
        var pickUpsArray = GameObject.FindGameObjectsWithTag("Pick Up");
        foreach (GameObject pickUp in pickUpsArray)
        {
            if (pickUp.transform.position.y > 0)
                allBelow = false;
        }
        return allBelow;
    }
}