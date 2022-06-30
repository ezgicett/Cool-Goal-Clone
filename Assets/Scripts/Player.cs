using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Animator animator;
    public GameObject winPanel;
    public GameObject failPanel;
    public Camera mainCamera;
    public GameObject[] camPos;
    public GameObject ball;
    public GameObject[] balls;
    public GameObject[] parts;
    public Image[] ProgressBarParts;
    private Vector3 directionVector;
    private Vector3 startPos;

    //ball move check
    private float timer;
    public bool isShooting;
    public bool isTouch;
    private bool onLimit;
    public bool isAnimation;

    // win-fail check
    static int partCount;
    public bool isGoal;
    public bool isFail;
    private bool isWin;
    public bool isThrowBall;

    private float timer2;
    public GameObject ps;
    private GameObject particles;
    public Transform rightParticlePos;
    public Transform leftParticlePos;
    private int count;


    void Start()
    {
        animator = GetComponent<Animator>();
    }
    

    void Update()
    {
      
        if (isShooting)
        {
            if (!isAnimation)
            {
                timer += Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, ball.transform.position - new Vector3(0, 0, 1), 3.5f * Time.deltaTime);
                if (timer >= 1f)
                {
                    animator.SetBool("isPass", true);
                    isAnimation = true;
                }
            }
        }
        if (isThrowBall)// soccer ball after force
        {
            if (isGoal) // if it is a goal
            {
                timer2 += Time.deltaTime;
                if(timer2 <2)
                {
                    if(count <6)
                    {
                        particles = Object.Instantiate(ps, rightParticlePos.position, Quaternion.identity);
                        particles = Object.Instantiate(ps, leftParticlePos.position, Quaternion.identity);
                        count++;
                    }
                  
                }
                else
                {
                    
                    ProgressBarParts[partCount].color = Color.blue;

                    if (partCount == 2)
                    {
                        winPanel.SetActive(true);
                        Destroy(ball);
                        isWin = true;
                        isGoal = false;
                    }
                    if (partCount < 2)
                    {
                        StartCoroutine(Move(mainCamera.transform.position, camPos[partCount + 1].transform.position));
                        if (mainCamera.transform.position == camPos[partCount + 1].transform.position)
                        {
                            for (int i = 0; i < balls.Length; i++)
                            {
                                balls[i].SetActive(false);
                            }
                            parts[partCount].SetActive(false);

                            parts[partCount + 1].SetActive(true);

                            partCount++;

                            isGoal = false;
                        }
                    }
                }
                
               

            }
            if (isFail)
            {
                failPanel.SetActive(true);
            }
            if (isWin)
            {
                partCount = 0;
            }
        }
    }



    public void loadCurrentLevel() // EİTHER BOTH WİN OR LOSE STATUS , GAME STARTS OVER.
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        partCount = 0;

    }

    IEnumerator Move(Vector3 startPos, Vector3 endPos) // camera moves between the parts.
    {
        float time = 0;
        while (time < 2f)
        {
            time += Time.deltaTime;
            mainCamera.transform.position = Vector3.Lerp(startPos, endPos, time / 2f);
            yield return null;
        }

        mainCamera.transform.position = endPos;
    }
}
