using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToPoint : MonoBehaviour
{
    bool done;
    Vector3 middle;
    GameFlowFramework_ScriptReferencer referencer;
    GameFlowFramework_PlayerCharacter player;

    void Start()
    {
        done = false;
        referencer = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameFlowFramework_ScriptReferencer>();
        player = referencer.GameFlowFramework_PlayerCharacter;
        middle = transform.GetChild(0).transform.position;
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !done)
        {
            done = true;

            player.playerObject.transform.position = new Vector3(middle.x
                , player.playerObject.transform.position.y
                , middle.z);
            referencer.effect_laserShoot.Play();

            /*
            player.controlsEnabled = false;
            player.playerObject.GetComponent<Rigidbody>().isKinematic = true;
            StartCoroutine(MoveOverSeconds(player.playerObject, middle, 1));
            StartCoroutine(AllowControls());
            */
        }
    }

    IEnumerator MoveOverSeconds(GameObject objectToMove, Vector3 end, float seconds)
    {
        float elapsedTime = 0;
        Vector3 startingPos = objectToMove.transform.position;
        while (elapsedTime < seconds)
        {
            objectToMove.transform.position = Vector3.Lerp(startingPos, end, (elapsedTime / seconds));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        objectToMove.transform.position = end;
    }

    IEnumerator AllowControls()
    {
        yield return new WaitForSeconds(2);
        player.controlsEnabled = true;
        player.playerObject.GetComponent<Rigidbody>().isKinematic = false;
    }
}
