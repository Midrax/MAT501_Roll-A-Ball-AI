using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Object pickUpPrefab;
    public Object dangerPrefab;
    public Text gameOverText;
    public int pickUps;

    // Start is called before the first frame update
    void Start()
    {
        //Time.timeScale = 1;
        createGameObjects();
    }

    // Update is called once per frame
    void Update()
    {
        bool allBelow = true;
        var pickUpsArray = GameObject.FindGameObjectsWithTag("Pick Up");
        foreach (GameObject pickUp in pickUpsArray)
        {
            if (pickUp.transform.position.y > 0)
                allBelow = false;
        }

        if (allBelow)
        {
            RollerAgent player = GameObject.FindObjectOfType<RollerAgent>();
            EnemyController enemy = GameObject.FindObjectOfType<EnemyController>();

            if (player.getScore() > enemy.getScore())
            {
                gameOverText.text = "You Win!";
            }
            else if (player.getScore() == enemy.getScore())
            {
                gameOverText.text = "It's a Draw!";
            }
            else
                gameOverText.text = "You lost...";
            //Time.timeScale = 0;
        }
        else
        {
            gameOverText.text = "";
        }
    }

    public void createGameObjects()
    {
        gameOverText.text = "";
        for (int i = 0; i < pickUps; i++)
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
            GameObject.Instantiate(dangerPrefab, position, Quaternion.identity);
        }

        for (int i = 0; i < pickUps; i++)
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
            GameObject.Instantiate(pickUpPrefab, position, Quaternion.identity);
        }
    }
}
