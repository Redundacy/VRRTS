using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seed : MonoBehaviour
{

    public GameObject Treefab;
    public int timeToGrow;
    public int scaleSize;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(scaleOverTime(gameObject.transform, new Vector3(scaleSize, scaleSize, scaleSize), timeToGrow));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    bool isScaling = false;

    IEnumerator scaleOverTime(Transform objectToScale, Vector3 toScale, float duration)
    {
        //Make sure there is only one instance of this function running
        if (isScaling)
        {
            yield break; ///exit if this is still running
        }
        isScaling = true;

        float counter = 0;

        //Get the current scale of the object to be moved
        Vector3 startScaleSize = objectToScale.localScale;

        while (counter < duration)
        {
            counter += Time.deltaTime;
            objectToScale.localScale = Vector3.Lerp(startScaleSize, toScale, counter / duration);
            yield return null;
        }

        isScaling = false;

        TransformIntoTree();
    }

    void TransformIntoTree()
    {
        Instantiate(Treefab, gameObject.transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
