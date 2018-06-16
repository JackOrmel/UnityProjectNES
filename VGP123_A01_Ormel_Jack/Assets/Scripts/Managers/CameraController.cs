using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [SerializeField] Transform target;

    [SerializeField] Transform cameraBoundMin;
    [SerializeField] Transform cameraBoundMax;

    float xMin, xMax, yMin, yMax;

    // Use this for initialization
    void Start () {
        if (!target)
        {
            Debug.LogWarning("No target found for camera to follow.");
            return;
        }

        if (!cameraBoundMin)
            Debug.LogWarning("CameraMin reference not found on " + name);
        if (!cameraBoundMax)
            Debug.LogWarning("CameraMax reference not found on " + name);

        xMin = cameraBoundMin.position.x;
        yMin = cameraBoundMin.position.y;

        xMax = cameraBoundMax.position.x;
        yMax = cameraBoundMax.position.y;
    }
	
	// Update is called once per frame
	void Update () {
        if (target)
        {
            transform.position = new Vector3(
                Mathf.Clamp(target.position.x, xMin, xMax),
                Mathf.Clamp(target.position.y, yMin, yMax),
                transform.position.z);
        }
    }
}
