using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    //This script is to be placed on every obstacle that the player may collide with.
    //A collider set to "trigger" is required on each obstacle.

    [Tooltip("True if the obstacle is destroyed on hit.")]
    public bool destroyedOnHit;
    [Tooltip("True if the obstacle can be hit infinitely.")]
    public bool infiniteHits;
    [Tooltip("The amount of health to INCREMENT to the player (negative values allowed).")]
    public int health;
    [Tooltip("The amount of score to INCREMENT to the player (negative values allowed).")]
    public int score;

    bool done;
    GameFlowFramework_ScriptReferencer reference;
    GameFlowFramework_PlayerCharacter player;

    void Start()
    {
        done = false;
        reference = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameFlowFramework_ScriptReferencer>();
        player = reference.GameFlowFramework_PlayerCharacter;
    }

    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !done)
        {
            if (!infiniteHits) { done = true; }
            if (health != 0)
            {
                player.Collided(health, 0);
            }
            if (score != 0)
            {
                player.Collided(score, 1);
            }

            if (destroyedOnHit)
            {
                reference.destroyEffect1.transform.position = transform.position;
                reference.destroyEffect1.Play();
                reference.destroyEffect2.transform.position = transform.position;
                reference.destroyEffect2.Play();
                reference.effect_8bitExplode.Play();
                Destroy(gameObject);
            }
        }
    }
}
