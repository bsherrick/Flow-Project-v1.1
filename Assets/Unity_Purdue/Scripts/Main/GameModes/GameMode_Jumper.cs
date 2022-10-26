using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMode_Jumper : MonoBehaviour
{

    GameFlowFramework_Environment env;

    void Start()
    {
        env = GetComponentInParent<GameFlowFramework_Environment>();
    }

    public void Init()
    {
        env.jumperModeParent.SetActive(true); //turn on the parent gameobject

        int spawnCooldown = env.minDistance; //obstacles must remain a minimum distance from each other based on minDistance
        float randomNumber; //the chance of spawning an obstacle

        ObstacleArray obstacleArrayScript = env.jumperModeParent.GetComponentInChildren<ObstacleArray>(); //get the script that has information on the obstacles

        if (env.enableDebugLog) { Debug.Log("Total number of obstacles: " + obstacleArrayScript.obstacles.Length); }

        //for every obstacle available game
        for (int i = 0; i < obstacleArrayScript.obstacles.Length; i++)
        {
            spawnCooldown--;
            if (spawnCooldown == 0) //if minDistance reached (if the current obstacle is far enough)
            {
                randomNumber = Random.Range(0f, 100f); //generate random percentage
                if (randomNumber <= env.spawnChance) //if the random number is within spawnChance
                {
                    obstacleArrayScript.obstacles[i].SetActive(true); //spawn obstacle

                    if (env.enableDebugLog)
                    {
                        Debug.Log("obstacle spawned.");
                        Debug.Log("obstacles[" + i + "] spawned with chance " + randomNumber);
                    }
                }
                spawnCooldown = env.minDistance; //reset minDistance
            }
        }
    }
}
