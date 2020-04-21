using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class Main : MonoBehaviour {
    public Material cubeColor0;
    
    public Material cubeColor1;

    public Material cubeColor2;

    public GameObject cursor;
    public GameObject roundCornerCube;

    private Board board;

    private List<GameObject> cubes = new List<GameObject>();
    // Start is called before the first frame update
    void Start() {
        for (var i = 17; i >= 0; i--) {
            GameObject cube = Instantiate(roundCornerCube); //GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = new Vector3(2 - (i % 3), i / 3, 0);
            cube.GetComponent<Renderer>().material = cubeColor0;//GetMaterial();
            cube.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
            cube.transform.localScale = Vector3.one * .9f;
            
            cubes.Add(cube);
        }

        board = new Board(cursor);
        fsm = new MainFSM(board);
    }

    private Material GetMaterial() {
        return Random.value < 0.5f ? cubeColor1 : cubeColor2;
    }

    private StateMachine fsm;

    void Update() {
        fsm.Update();
    }

    private int[,] numbers = new int[4, 18] {
        {
            1, 1, 1, 
            1, 0, 1, 
            1, 0, 1, 
            1, 0, 1, 
            1, 0, 1, 
            1, 1, 1
        },
        {
            1, 1, 0, 
            0, 1, 0, 
            0, 1, 0, 
            0, 1, 0, 
            0, 1, 0, 
            1, 1, 1
        },
        {
            1, 1, 1, 
            0, 0, 1, 
            0, 1, 1, 
            1, 0, 0, 
            1, 0, 0, 
            1, 1, 1
        },
        {
            1, 1, 1, 
            0, 0, 1, 
            1, 1, 1, 
            0, 0, 1, 
            0, 0, 1, 
            1, 1, 1
        },
    };

    private int currentCount = 3;
    private void CountDownAnimTick() {
        if (currentCount == -1) {
            GetComponent<Animator>().enabled = false;
            
            for (int i = 0; i < 18; i++) {
                cubes[i].transform.SetParent(null);
                cubes[i].GetComponent<Renderer>().material = GetMaterial();
            }

            return;
        }

        for (int i = 0; i < 18; i++) {
            if (numbers[currentCount,i] == 1) {
                cubes[i].transform.SetParent(transform);
                cubes[i].GetComponent<Renderer>().material = cubeColor2;
            }
            else {
                cubes[i].transform.SetParent(null);
                cubes[i].GetComponent<Renderer>().material = cubeColor1;
            }
        }

        currentCount--;
    }
}