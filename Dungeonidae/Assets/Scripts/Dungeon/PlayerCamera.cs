using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerCamera : MonoBehaviour
{
    public Player target;
    Vector3 cameraPostion;
    Vector3 targetPosition = new();
    Camera cam;

    Vector3 lastMousePostion = new();
    bool found = false;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            if(found && target.Controllable)
            {
                if (EventSystem.current.IsPointerOverGameObject()) return;
                float scroll = Input.GetAxis("Mouse ScrollWheel");
                cameraPostion = transform.position;

                cam.orthographicSize -= scroll * Time.deltaTime * 1500;
                if (cam.orthographicSize < 1)
                    cam.orthographicSize = 1;
                else if (cam.orthographicSize > 40)
                    cam.orthographicSize = 40;

                if (Input.GetMouseButton(0))
                {
                    Vector3 diff = lastMousePostion - cam.ScreenToWorldPoint(Input.mousePosition);
                    if (diff.magnitude > 0.075f)
                        cameraPostion += diff;
                    cameraPostion.x = (cameraPostion.x < 0) ? 0 : cameraPostion.x;
                    cameraPostion.x = (cameraPostion.x > 100) ? 100 : cameraPostion.x;
                    cameraPostion.y = (cameraPostion.y < 0) ? 0 : cameraPostion.y;
                    cameraPostion.y = (cameraPostion.y > 100) ? 100 : cameraPostion.y;
                }
                transform.position = cameraPostion;
            }
            else
            {
                targetPosition.Set(target.transform.position.x, target.transform.position.y, transform.position.z);
                //transform.position = targetPosition;
                transform.position = Vector3.Lerp(transform.position, targetPosition, 0.1f);
            }

            if (!found)
            {
                transform.position = targetPosition;
                found = true;
            }
        }
        else
        {
            target = GameObject.Find("Warrior(Clone)").GetComponent<Player>();
        }

        lastMousePostion = cam.ScreenToWorldPoint(Input.mousePosition);
    }
}
