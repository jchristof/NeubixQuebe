using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class Main : MonoBehaviour
{
    public Material cubeColor1;

    public Material cubeColor2;

    public GameObject cursor;
    
    private InputActions inputActions;
    private Vector2 movementInput;
    private bool lockInput;
    private Board board;
    

    private void Awake()
    {
       inputActions = new InputActions();
       inputActions.PlayerControls.Move.performed += ctx =>
       {
           var newMove = ctx.ReadValue<Vector2>();
       };
       inputActions.PlayerControls.Lock.performed += ctx => lockInput = ctx.ReadValue<bool>();
       inputActions.Enable();
    }

    // Start is called before the first frame update
    void Start()
    {
        for (var i = 0; i < 18; i++)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = new Vector3(i % 3, i / 3, 0);
            cube.GetComponent<Renderer>().material = GetMaterial();
            cube.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
            cube.transform.localScale = Vector3.one * .9f;
        }
        
        board = new Board(cursor);
        mainFsm = new MainFSM(board);
    }

    private Material GetMaterial()
    {
        return Random.value < 0.5f ? cubeColor1 : cubeColor2;
    }

    private bool UpPressed() => Gamepad.current?.dpad?.up?.wasPressedThisFrame == true ||
                                Keyboard.current.wKey.wasPressedThisFrame;
    private bool DownPressed() => Gamepad.current?.dpad?.down?.wasPressedThisFrame == true ||
                                Keyboard.current.sKey.wasPressedThisFrame;
    private bool LeftPressed() => Gamepad.current?.dpad?.left?.wasPressedThisFrame == true ||
                                Keyboard.current.aKey.wasPressedThisFrame;
    private bool RightPressed() => Gamepad.current?.dpad?.right?.wasPressedThisFrame == true ||
                                Keyboard.current.dKey.wasPressedThisFrame;
    
    private bool LockPressed() => Gamepad.current?.aButton?.wasPressedThisFrame == true ||
                                  Keyboard.current.spaceKey.wasPressedThisFrame;

    private float moveStartTime;
    private Vector3 moveStartPosition;
    private Vector3 moveEndPosition;
    private bool moving;

    private MainFSM mainFsm;
    void Update()
    {
        mainFsm.Update();
        
//        if (moving)
//        {
//            cursor.transform.position = Vector3.Slerp(moveStartPosition, moveEndPosition, (Time.time - moveStartTime)/.1f);
//            if ((cursor.transform.position - moveEndPosition).magnitude < .1f)
//            {
//                cursor.transform.position = moveEndPosition;
//                moving = false;
//            }
//            return;
//        }
//        Vector3 currentPosition = cursor.transform.position;
//        if (UpPressed())
//        {
//            if (LockPressed())
//            {
//                
//            }
//            else
//            {
//                moveStartTime = Time.time;
//                moveStartPosition = currentPosition;
//                moveEndPosition = new Vector3(currentPosition.x, currentPosition.y + 1, currentPosition.z);
//                moving = true;
//                //transform.position = Vector3.MoveTowards(transform.position, target.position, step);
//            }
//                //cursor.transform.position = new Vector3(currentPosition.x, currentPosition.y + 1, currentPosition.z);
//        }
//        else if (DownPressed())
//        {
//            cursor.transform.position = new Vector3(currentPosition.x, currentPosition.y - 1, currentPosition.z);
//        }
//        else if (LeftPressed())
//        {
//            cursor.transform.position = new Vector3(currentPosition.x - 1, currentPosition.y, currentPosition.z);
//        }
//        else if (RightPressed())
//        {
//            cursor.transform.position = new Vector3(currentPosition.x + 1, currentPosition.y, currentPosition.z);
//        }
//        
        
    }
}
