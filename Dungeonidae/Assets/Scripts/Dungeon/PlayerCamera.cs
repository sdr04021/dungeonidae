using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class PlayerCamera : MonoBehaviour
{
    public Player target;
    Vector3 cameraPostion;
    Vector3 targetPosition = new();
    Camera cam;

    Vector3 lastMousePostion = new();
    bool found = false;
    bool dragging = false;

    Vector3 currentVelocity;

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
            targetPosition.Set(target.transform.position.x, target.transform.position.y, transform.position.z);
            if (found && target.Controllable)
            {
                if (EventSystem.current.IsPointerOverGameObject()) return;

                cameraPostion = transform.position;
                if (Input.GetMouseButton(0))
                {
                    Vector3 diff = lastMousePostion - Input.mousePosition;
                    if (diff.magnitude > 16f)
                        dragging = true;
                    if (dragging)
                        cameraPostion += (cam.ScreenToWorldPoint(lastMousePostion) - cam.ScreenToWorldPoint(Input.mousePosition));
                    cameraPostion.x = (cameraPostion.x < 0) ? 0 : cameraPostion.x;
                    cameraPostion.x = (cameraPostion.x > 100) ? 100 : cameraPostion.x;
                    cameraPostion.y = (cameraPostion.y < 0) ? 0 : cameraPostion.y;
                    cameraPostion.y = (cameraPostion.y > 100) ? 100 : cameraPostion.y;
                }
                transform.position = cameraPostion;
            }
            else
            {
                transform.position = targetPosition;
                //transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, Time.deltaTime*50);
                //transform.DOMove(targetPosition, Time.deltaTime * 20);
            }

            if (!EventSystem.current.IsPointerOverGameObject())
            {
                float scroll = Input.GetAxis("Mouse ScrollWheel");
                cam.orthographicSize -= scroll * Time.deltaTime * 1500;
                if (cam.orthographicSize < 1)
                    cam.orthographicSize = 1;
                else if (cam.orthographicSize > 40)
                    cam.orthographicSize = 40;
            }

            if (Input.GetMouseButtonUp(0))
                dragging = false;

            if (!found)
            {
                transform.position = targetPosition;
                found = true;
            }
        }
        else
        {
            GameObject temp = GameObject.Find("Warrior(Clone)");
            if((temp!=null)&&temp.TryGetComponent(out Player player))
            {
                target = player;
            }
        }

        lastMousePostion = Input.mousePosition;
    }
}
