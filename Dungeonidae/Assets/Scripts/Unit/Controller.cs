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

        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, 0f, LayerMask.GetMask("Tile"));
        if (hit.collider != null)
        {
            if (!EventSystem.current.IsPointerOverGameObject())
                player.TileTargeted(hit.collider.gameObject.GetComponent<Tile>().Coord);
        }


        if (Input.GetMouseButtonUp(0))
        {
            if (mouseDragged)
            {
                mouseDragged = false;
            }
            else if (hit.collider != null)
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    Coordinate coord = hit.collider.gameObject.GetComponent<Tile>().Coord;
                    player.TileClicked(coord);
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            if (player.Controllable)
            {
                player.PrepareBasicAttack();
            }
            else if (player.IsBasicAttackMode)
            {
                player.AutoBasicAttack();
            }
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            if (player.IsBasicAttackMode && (player.BasicAttack.AvailableTilesInRange.Count == 0))
            {
                player.CancelBasicAttack();
            }
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            if (player.Controllable)
            {
                player.SkipTurn();
            }
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            if (player.Controllable)
            {
                player.LootItem();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            if(player.Controllable)
            {
                player.PrepareSkill(0);
            }
            else if (player.IsSkillMode)
            {
                player.AutoSkill();
            }
        }
        else if (Input.GetKeyUp(KeyCode.Q))
        {
            if (player.IsSkillMode && (player.skill.AvailableTilesInRange.Count==0))
            {
                player.CancelSkill();
            }
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            if (player.Controllable)
            {
                player.PrepareSkill(1);
            }
            else if (player.IsSkillMode)
            {
                player.AutoSkill();
            }
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            if (player.IsSkillMode && (player.skill.AvailableTilesInRange.Count == 0))
            {
                player.CancelSkill();
            }
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            if (player.Controllable)
            {
                player.PrepareSkill(2);
            }
            else if (player.IsSkillMode)
            {
                player.AutoSkill();
            }
        }
        else if (Input.GetKeyUp(KeyCode.E))
        {
            if (player.IsSkillMode && (player.skill.AvailableTilesInRange.Count == 0))
            {
                player.CancelSkill();
            }
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            if (player.Controllable)
            {
                player.PrepareSkill(3);
            }
            else if (player.IsSkillMode)
            {
                player.AutoSkill();
            }
        }
        else if (Input.GetKeyUp(KeyCode.R))
        {
            if (player.IsSkillMode && (player.skill.AvailableTilesInRange.Count == 0))
            {
                player.CancelSkill();
            }
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            if (player.Controllable)
            {
                player.PrepareSkill(4);
            }
            else if (player.IsSkillMode)
            {
                player.AutoSkill();
            }
        }
        else if (Input.GetKeyUp(KeyCode.T))
        {
            if (player.IsSkillMode && (player.skill.AvailableTilesInRange.Count == 0))
            {
                player.CancelSkill();
            }
        }

        lastMousePostion.Set(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);
    }
}
