using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Route : MonoBehaviour
{
    public Transform[] controlPoints;
    public GameObject soccerBall;
    public GameObject trajectoryBallPrefab;
    public GameObject ballPool;
    private Vector3[] positions = new Vector3[10];
    private Vector3 tempVector2;
    private Vector3 tempVector3;
    private Vector3 tempVector4;
    private Vector3 directionVector;
    private GameObject[] trajectoryBalls;
    public GameObject player;


    private int numPoints = 10;
    private int posCount;

    private bool isTouch;
    private bool onLimit;
    private bool isUp;
    public bool isShooting;
    private bool isTrajectoryMove;
    private bool isBallMoveOnce;

    private float point2XPos;
    private float point3XPos;
    private float point4XPos;
    private float distance;

  

    private bool instantiateBalls;

    // TRAJECTORY MADE WİTH A CUBİC BEZİER CURVE
    // 4 CONTROL POİNTS FOR CREATİNG A CURVE-> controlPoints[]
    // ACCORDİNG TO THE HAND MOVE, CONTROL POİNTS VALUE EXPAND
    private void Start()
    {
        isTrajectoryMove = false;
        trajectoryBalls = new GameObject[10];
       
        DrawCubicCurve();

        for (int i = 0; i < controlPoints.Length; i++) // control points position set
        {
            if (i == 0)
                controlPoints[i].position = new Vector3(soccerBall.transform.position.x, soccerBall.transform.position.y, soccerBall.transform.position.z);
            else
                controlPoints[i].position = new Vector3(soccerBall.transform.position.x, soccerBall.transform.position.y, controlPoints[i - 1].position.z + 7);

        }

    }

    private void Update()
    {
        
        DrawCubicCurve();

        if (isShooting) // after trajectory ball moves to the goal area
        {
            soccerBall.GetComponent<Rigidbody>().isKinematic = false;
            soccerBall.GetComponent<Rigidbody>().velocity = directionVector * 15;
            player.GetComponent<Player>().isThrowBall = true;
            isShooting = false;
        }
        
        if (Input.GetMouseButtonUp(0))
        {
            isTouch = false;
            isUp = true;
            isTrajectoryMove = true;
            foreach (GameObject item in trajectoryBalls)
            {
                item.SetActive(false);
            }
        }
        if (Input.GetMouseButton(0))
        {
            isTouch = true;
            
        }
    }
    private void DrawCubicCurve()
    {
        if (isTouch && !isTrajectoryMove) // if I m pressing to secreen and checking other touches
        {
            if (!instantiateBalls)
            {
                // trajectory ball instantiate
                InstantiateTrajectoryBalls();
            }
            distance = (0.5f - (Input.mousePosition.x / Screen.width));


            if (controlPoints[1].position.x < -5 ) // left limit check
            {
                controlPoints[1].position = new Vector3(-5, controlPoints[1].position.y ,controlPoints[1].position.z);
                controlPoints[2].position = new Vector3(-5, controlPoints[2].position.y ,controlPoints[2].position.z);
                onLimit = true;
            }
            else if(controlPoints[1].position.x > 9) // right limit check
            {
                controlPoints[1].position = new Vector3(9, controlPoints[1].position.y, controlPoints[1].position.z);
                controlPoints[2].position = new Vector3(9, controlPoints[2].position.y, controlPoints[2].position.z);
                onLimit = true;
            }
            if (controlPoints[3].position.x < -2.2f ) // left limit after turning from onLimit position
            {
                controlPoints[3].position = new Vector3(-2.2f, controlPoints[3].position.y, controlPoints[3].position.z);
                onLimit = false;
            }
            else if(controlPoints[3].position.x > 7) // right limit after turning from onLimit position
            {
                controlPoints[3].position = new Vector3(7, controlPoints[3].position.y, controlPoints[3].position.z);
                onLimit = false;
            }
            TrajectoryMove();
            
        }
        if (isUp && !isShooting && !isBallMoveOnce) // if I m not hold pressing to the screen
        {
            //in player script, make true of animation bool and start shooting a ball
            player.GetComponent<Player>().isShooting = true;
            player.GetComponent<Player>().animator.SetBool("isRunning", true);
            directionVector = positions[positions.Length - 1] - positions[positions.Length - 2];

            throwingBallThroughLine();
        }
    }
    private Vector3 CalculateCubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;
        Vector3 p = uuu * p0;
        p += 3 * uu * t * p1;
        p += 3 * u * tt * p2;
        p += ttt * p3;
        return p;
    }
    private void TrajectoryMove()
    {
        if (!onLimit)
        {
            point2XPos = distance * 20;
            point4XPos = distance * 11;

            tempVector2 = new Vector3(controlPoints[0].position.x + point2XPos, controlPoints[1].position.y, controlPoints[1].position.z);
            tempVector3 = new Vector3(controlPoints[0].position.x + point2XPos, controlPoints[2].position.y, controlPoints[2].position.z);
            tempVector4 = new Vector3(controlPoints[0].position.x + point4XPos, controlPoints[3].position.y, controlPoints[3].position.z);
        }
        if (onLimit) // limit noktasındaysan
        {
            point4XPos = 1 / (distance);
            tempVector4 = new Vector3(controlPoints[0].position.x + point4XPos, controlPoints[3].position.y, controlPoints[3].position.z);

        }
        for (int i = 1; i < numPoints + 1; i++)
        {
            float t = i / (float)numPoints;
            positions[i - 1] = CalculateCubicBezierPoint(t, controlPoints[0].position, controlPoints[1].position, controlPoints[2].position, controlPoints[3].position);
            trajectoryBalls[i - 1].transform.position = positions[i - 1];
        }

        controlPoints[1].position = tempVector2;
        controlPoints[2].position = tempVector3;
        controlPoints[3].position = tempVector4;
    }

    private void InstantiateTrajectoryBalls()
    {
        for (int i = 0; i < numPoints; i++)
        {
            GameObject trajectoryBall = Object.Instantiate(trajectoryBallPrefab, soccerBall.transform.position, Quaternion.identity, ballPool.transform); // su topu instantiate elildi
            trajectoryBalls[i] = trajectoryBall;
        }
        instantiateBalls = true;
    }

    private void throwingBallThroughLine() // after player thorw ball animation, soccerball moves on trajectory curve.
    {
        if (player.GetComponent<Player>().isAnimation == true)//throwing soccerball
        {
            soccerBall.transform.position = Vector3.MoveTowards(soccerBall.transform.position, positions[posCount], 20f * Time.deltaTime);

            if (soccerBall.transform.position == positions[posCount])
                posCount++;

          
            if (posCount == positions.Length)
            {
                isUp = false;
                isShooting = true;
                posCount = 0;
                isBallMoveOnce = true;

            }
        }
    }

}
