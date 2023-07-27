using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Controller : MonoBehaviour
{
    Player player;

    Vector3 lastMousePostion = new();
    bool mouseDragged = false;
    float SPressedTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
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
                player.StartBasicAttack();
            }
            else if (player.IsSkillMode && player.CurrentSkill == player.BasicAttack)
            {
                player.SkillOnCurrentTargeting();
            }
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            if (player.IsSkillMode && player.CurrentSkill == player.BasicAttack && (player.AvailableRange.Count == 0))
            {
                player.CancelSkill();
            }
        }
        else if (Input.GetKey(KeyCode.S))
        {
            if (player.Controllable)
            {
                if ((SPressedTime > 2) && (!player.FoundSomething))
                {
                    player.SkipTurn();
                }
                else
                {
                    if (Input.GetKeyDown(KeyCode.S))
                        player.SkipTurn();
                    SPressedTime += Time.deltaTime;
                }

            }
        }
        else if (Input.GetKeyUp(KeyCode.S))
            SPressedTime = 0;
        else if (Input.GetKeyUp(KeyCode.D))
        {
            if (player.Controllable)
            {
                player.Interact();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            SkillKeyDown(0);
        }
        else if (Input.GetKeyUp(KeyCode.Q))
        {
            SkillKeyUp(0);
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            SkillKeyDown(1);
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            SkillKeyUp(1);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            SkillKeyDown(2);
        }
        else if (Input.GetKeyUp(KeyCode.E))
        {
            SkillKeyUp(2);
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            SkillKeyDown(3);
        }
        else if (Input.GetKeyUp(KeyCode.R))
        {
            SkillKeyUp(3);
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            SkillKeyDown(4);
        }
        else if (Input.GetKeyUp(KeyCode.T))
        {
            SkillKeyUp(4);
        }
        else if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.Keypad8))
        {
            if (player.Controllable)
            {
                if (Input.GetKey(KeyCode.RightArrow)) player.Step(Directions.NE);
                else if (Input.GetKey(KeyCode.LeftArrow)) player.Step(Directions.NW);
                else player.Step(Directions.N);
            }
        }
        else if (Input.GetKey(KeyCode.Keypad9))
        {
            if(player.Controllable) player.Step(Directions.NE);
        }
        else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.Keypad2))
        {
            if (player.Controllable)
            {
                if (Input.GetKey(KeyCode.RightArrow)) player.Step(Directions.SE);
                else if (Input.GetKey(KeyCode.LeftArrow)) player.Step(Directions.SW);
                else player.Step(Directions.S);
            }
        }
        else if (Input.GetKey(KeyCode.Keypad3))
        {
            if (player.Controllable) player.Step(Directions.SE);
        }
        else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.Keypad4))
        {
            if (player.Controllable)
            {
                if (Input.GetKey(KeyCode.UpArrow)) player.Step(Directions.NW);
                else if (Input.GetKey(KeyCode.DownArrow)) player.Step(Directions.SW);
                else player.Step(Directions.W);
            }
        }
        else if (Input.GetKey(KeyCode.Keypad1))
        {
            if (player.Controllable) player.Step(Directions.SW);
        }
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.Keypad6))
        {
            if (player.Controllable)
            {
                if (Input.GetKey(KeyCode.UpArrow)) player.Step(Directions.NE);
                else if (Input.GetKey(KeyCode.DownArrow)) player.Step(Directions.SE);
                else player.Step(Directions.E);
            }

        }
        else if (Input.GetKey(KeyCode.Keypad7))
        {
            if (player.Controllable) player.Step(Directions.NW);
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(player.IsSkillMode) player.CancelSkill();
        }

        if (player.IsSkillMode || player.IsThrowingMode)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Keypad8))
            {
                player.MoveTargeting(Directions.N);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.Keypad2))
            {
                player.MoveTargeting(Directions.S);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.Keypad4))
            {
                player.MoveTargeting(Directions.W);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.Keypad6))
            {
                player.MoveTargeting(Directions.E);
            }
        }

        lastMousePostion.Set(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);
    }

    void SkillKeyDown(int index)
    {
        if (player.Controllable)
        {
            player.StartSkill(index);
        }
        else if (player.IsSkillMode && player.CurrentSkill.Key == player.UnitData.currentSkills[index])
        {
            player.SkillOnCurrentTargeting();
        }
    }
    void SkillKeyUp(int index)
    {
        if (player.IsSkillMode && player.CurrentSkill.Key == player.UnitData.currentSkills[index] && (player.AvailableRange.Count == 0))
        {
            player.CancelSkill();
        }
    }
}
