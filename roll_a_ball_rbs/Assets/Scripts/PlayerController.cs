using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    public float speed;
    public Text playerScoreText;

    int pickUps;

    private Rigidbody rb;
    private EnemyController enemy;
    private int playerScore;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerScore = 0;
        SetPlayerScoreText();
        pickUps = GameObject.FindGameObjectsWithTag("Pick Up").Length;
    }

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        rb.AddForce(movement * speed);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pick Up"))
        {
            Object.Destroy(other.gameObject);
            playerScore = playerScore + 2;
            SetPlayerScoreText();
        }

        if (other.gameObject.CompareTag("Danger"))
        {
            Object.Destroy(other.gameObject);
            playerScore = playerScore - 1;
            SetPlayerScoreText();
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
            }
            else
            {
                playerScore--;
            }
            SetPlayerScoreText();
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
}