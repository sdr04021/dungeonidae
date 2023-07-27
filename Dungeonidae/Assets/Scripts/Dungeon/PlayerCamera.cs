using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.Experimental.Rendering.Universal;

public class PlayerCamera : MonoBehaviour
{
    public Player target;
    Vector3 cameraPostion;
    Vector3 targetPosition = new();
    Camera cam;

    Vector3 lastMousePostion = new();
    bool found = false;
    bool dragging = false;
    bool follow = true;
    
    Vector3 currentVelocity;

    PixelPerfectCamera ppc;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        ppc = GetComponent<PixelPerfectCamera>();
    }

    void StartFollow()
    {
        follow = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            targetPosition.Set(target.transform.position.x, target.transform.position.y, transform.position.z);

            if (EventSystem.current.IsPointerOverGameObject()) return;

            cameraPostion = transform.position;
            if (Input.GetMouseButton(2))
            {
                Vector3 diff = lastMousePostion - Input.mousePosition;
                if (!dragging && (diff.magnitude > 16f))
                {
                    dragging = true;
                    follow = false;
                }
                if (dragging)
                    cameraPostion += (cam.ScreenToWorldPoint(lastMousePostion) - cam.ScreenToWorldPoint(Input.mousePosition));
                cameraPostion.x = (cameraPostion.x < 0) ? 0 : cameraPostion.x;
                cameraPostion.x = (cameraPostion.x > 100) ? 100 : cameraPostion.x;
                cameraPostion.y = (cameraPostion.y < 0) ? 0 : cameraPostion.y;
                cameraPostion.y = (cameraPostion.y > 100) ? 100 : cameraPostion.y;
                transform.position = cameraPostion;
            }
            if (follow)
            {
                if (Vector2.Distance((Vector2)transform.position, (Vector2)targetPosition) > 0.15f * cam.orthographicSize)
                {
                    transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, Time.deltaTime * 30);
                }
                //else if(target.Controllable) follow = false;
                //transform.position = targetPosition;
                //transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, Time.deltaTime*50);
                //transform.DOMove(targetPosition, Time.deltaTime * 20);
            }

            if (!EventSystem.current.IsPointerOverGameObject())
            {
                float scroll = Input.GetAxis("Mouse ScrollWheel");
                int d = (int)(scroll * Time.deltaTime * 1500f);
                if (scroll != 0)
                {
                    ppc.refResolutionX -= (d * 240);
                    ppc.refResolutionY -= (d * 135);
                    ppc.refResolutionX = Mathf.Max(ppc.refResolutionX, 240);
                    ppc.refResolutionY = Mathf.Max(ppc.refResolutionY, 135);
                    ppc.refResolutionX = Mathf.Min(ppc.refResolutionX, 1200);
                    ppc.refResolutionY = Mathf.Min(ppc.refResolutionY, 675);
                    /*
                    cam.orthographicSize -= scroll * Time.deltaTime * 1500;
                    if (cam.orthographicSize < 1)
                        cam.orthographicSize = 1;
                    else if (cam.orthographicSize > 40)
                        cam.orthographicSize = 40;
                    */
                }
            }

            if (Input.GetMouseButtonUp(2))
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
                target.MoveCamera += new Player.EventHandler(StartFollow);
            }
        }

        lastMousePostion = Input.mousePosition;
    }
}
