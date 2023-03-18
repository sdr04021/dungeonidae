using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public GameObject target;
    Vector3 targetPosition = new();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            targetPosition.Set(target.transform.position.x, target.transform.position.y, transform.position.z);
            transform.position = targetPosition;

            float scroll = Input.GetAxis("Mouse ScrollWheel");

            Camera.main.orthographicSize -= scroll * Time.deltaTime * 1500;
            if(Camera.main.orthographicSize < 1 )
                Camera.main.orthographicSize = 1;
            else if(Camera.main.orthographicSize > 40 )
                Camera.main.orthographicSize = 40;
        }
        else
        {
            target = GameObject.Find("Warrior(Clone)");
        }
    }
}
