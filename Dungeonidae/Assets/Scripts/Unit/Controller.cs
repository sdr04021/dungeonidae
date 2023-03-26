using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Controller : MonoBehaviour
{
    Player player;

    Vector3 lastMousePostion = new();
    bool mouseDragged = false;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.GetKey(KeyCode.W))
        {
            player.Move(Directions.N);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            player.Move(Directions.NE);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            player.Move(Directions.E);
        }
        else if (Input.GetKey(KeyCode.C))
        {
            player.Move(Directions.SE);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            player.Move(Directions.S);
        }
        else if (Input.GetKey(KeyCode.Z))
        {
            player.Move(Directions.SW);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            player.Move(Directions.W);
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            player.Move(Directions.NW);
        }
        else if (Input.GetKeyDown(KeyCode.F))
            player.SkipTurn();
        */
        if (Input.GetMouseButton(0) && (Vector3.Magnitude(lastMousePostion - Input.mousePosition) > 6))
        {
            mouseDragged = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (mouseDragged)
            {
                mouseDragged = false;
                return;
            }

            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, 0f, LayerMask.GetMask("Tile"));
            if (hit.collider != null)
            {
                if (EventSystem.current.IsPointerOverGameObject()) return;
                Coordinate coord = hit.collider.gameObject.GetComponent<Tile>().Coord;
                player.TileClicked(coord);
            }
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            if (player.Controllable)
            {
                player.SkipTurn();
            }
        }
        else if (Input.GetKeyUp(KeyCode.Z))
        {
            if (player.Controllable)
            {
                player.LootItem();
            }
        }
        lastMousePostion.Set(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);
    }
}
