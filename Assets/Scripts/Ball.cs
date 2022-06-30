using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public GameObject player;
    private float time;
    private bool isGoal;

    private void Update()
    {
        if(isGoal)
        {
            time += Time.deltaTime;
            if(time > 0.5f)
            {
                player.GetComponent<Player>().isGoal = true;
                isGoal = false;

            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("goalArea"))
        {
            isGoal = true;
        }
        if(collision.gameObject.CompareTag("outArea"))
        {
            player.GetComponent<Player>().isFail = true;
        }
        if (collision.gameObject.CompareTag("leftFailArea"))
        {
            player.GetComponent<Player>().isFail = true;
        }
        if (collision.gameObject.CompareTag("rightFailArea"))
        {
            player.GetComponent<Player>().isFail = true;
        }
    }
}
