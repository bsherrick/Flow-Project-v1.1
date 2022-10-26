using UnityEngine;
using System.Collections;

// Makes objects float up & down while gently spinning.
public class FloatAndRotate : MonoBehaviour
{
    // User Inputs
    public float degreesPerSecond_original;
    public float amplitude_original;
    public float frequency_original;

    // User Inputs
    public float degreesPerSecond = 15.0f;
    public float amplitude = 0.5f;
    public float frequency = 1f;

    //Mangkorn
    public bool random;
    public bool local;

    // Position Storage Variables
    Vector3 posOffset = new Vector3();
    Vector3 tempPos = new Vector3();

    //Mangkorn
    Vector3 posOffset2 = new Vector3();
    Vector3 tempPos2 = new Vector3();

    // Use this for initialization
    void Start()
    {
        degreesPerSecond_original = degreesPerSecond;
        amplitude_original = amplitude;
        frequency_original = frequency;

        // Store the starting position & rotation of the object
        posOffset = transform.position;
        posOffset2 = transform.localPosition;

        float scale = 1f;
        if (local) { scale = 0.75f; }


        degreesPerSecond *= scale;
        amplitude *= scale;
        //frequency *= scale;

        //Mangkorn
        if (random)
        {
            degreesPerSecond += Random.Range(-4f, 4f);
            amplitude += Random.Range(-0.5f * scale, 0.5f * scale);
            frequency += Random.Range(-0.25f, 0.25f);
        }
    }

    // Update is called once per frame
    void Update()
    {

        // Spin object around Y-Axis
        if (local)
        {
            transform.Rotate(new Vector3(0f, Time.deltaTime * degreesPerSecond, 0f), Space.Self);
        }
        else
        {
            transform.Rotate(new Vector3(0f, Time.deltaTime * degreesPerSecond, 0f), Space.World);
        }

        // Float up/down with a Sin()
        tempPos = posOffset;
        tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;

        tempPos2 = posOffset2;
        tempPos2.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;

        //transform.position = tempPos;
        //transform.localPosition = tempPos;

        if (local)
        {
            transform.localPosition = tempPos2;
        }
        else
        {
            transform.position = tempPos;
        }

    }

    public void Modify(float deg, float amp, float freq, float time)
    {
        if (deg > -1) { degreesPerSecond = deg; }
        if (amp > -1) { amplitude = amp; }
        if (freq > -1) { frequency = freq; }
        StartCoroutine(StopModify(time));
    }

    IEnumerator StopModify(float time)
    {
        yield return new WaitForSeconds(time);
        degreesPerSecond = degreesPerSecond_original;
        amplitude = amplitude_original;
        frequency = frequency_original;
    }

    public void ForceReset()
    {
        degreesPerSecond = degreesPerSecond_original;
        amplitude = amplitude_original;
        frequency = frequency_original;
    }
}