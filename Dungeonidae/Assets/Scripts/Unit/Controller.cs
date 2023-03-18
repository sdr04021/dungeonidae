using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    Player player;

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
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, 0f, LayerMask.GetMask("Tile"));
            if (hit.collider != null)
            {
                Coordinate coord = hit.collider.gameObject.GetComponent<Tile>().Coord;
                player.TileClicked(coord);
            }
        }
        else if (Input.GetKeyDown(KeyCode.F))
            player.SkipTurn();
    }
}