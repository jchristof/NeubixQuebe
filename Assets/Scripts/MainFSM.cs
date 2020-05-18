using System.ComponentModel;
using System.Numerics;
using UnityEngine;
//using UnityEngine.InputSystem;
using Vector3 = UnityEngine.Vector3;

public class MainFSM : FSM<MainFSM> {
    public MainFSM(Board board) {
        Board = board;
        SetState(typeof(InputIdle));
    }

    public Board Board { get; }
}

public enum InputDir {
    Up,
    Down,
    Left,
    Right,
    None
}

class InputIdle : State<MainFSM> {
    public InputIdle(MainFSM fsm) : base(fsm) { }

    // private bool LockPressed() => Gamepad.current?.aButton?.isPressed == true ||
    //                               Keyboard.current.spaceKey.isPressed;

    private InputDir GetDirectionInput() {
        // if ((Gamepad.current?.dpad?.up?.wasPressedThisFrame == true ||
        //      Keyboard.current.wKey.wasPressedThisFrame))
        //     return InputDir.Up;
        // if ((Gamepad.current?.dpad?.down?.wasPressedThisFrame == true ||
        //      Keyboard.current.sKey.wasPressedThisFrame))
        //     return InputDir.Down;
        // if ((Gamepad.current?.dpad?.left?.wasPressedThisFrame == true ||
        //      Keyboard.current.aKey.wasPressedThisFrame))
        //     return InputDir.Left;
        // if ((Gamepad.current?.dpad?.right?.wasPressedThisFrame == true ||
        //      Keyboard.current.dKey.wasPressedThisFrame))
        //     return InputDir.Right;

        return InputDir.None;
    }

    public override void Pre(object args = null) { }

    public override void Update() {
        InputDir inputDir = GetDirectionInput();

        if (inputDir == InputDir.None)
            return;

        // if (LockPressed())
        //     fsm.SetState(typeof(InputMoveBoard), inputDir);

        else
            fsm.SetState(typeof(InputMoveCursor), inputDir);
    }
}

class InputMoveCursor : State<MainFSM> {
    public InputMoveCursor(MainFSM fsm) : base(fsm) { }

    private float moveStartTime;
    private Vector3 moveStartPosition;
    private Vector3 moveEndPosition;

    public override void Pre(object args) {
        var inputDir = (InputDir)args;
        moveStartTime = Time.time;
        moveStartPosition = fsm.Board.Cursor.transform.position;
        moveEndPosition = moveStartPosition.ToTargetFromDirection(inputDir);
    }

    public override void Update() {
        var cursor = fsm.Board.Cursor;
        cursor.transform.position =
            Vector3.Slerp(moveStartPosition, moveEndPosition, (Time.time - moveStartTime) / .1f);
        if ((cursor.transform.position - moveEndPosition).magnitude < .1f) {
            cursor.transform.position = moveEndPosition;
            fsm.SetState(typeof(InputIdle));
        }
    }
}

class InputMoveBoard : State<MainFSM> {
    protected InputMoveBoard(MainFSM fsm) : base(fsm) { }
}

public static class Extensions {
    public static Vector3 Up(this Vector3 v) => v + Vector3.up;
    public static Vector3 Down(this Vector3 v) => v + Vector3.down;
    public static Vector3 Left(this Vector3 v) => v + Vector3.left;
    public static Vector3 Right(this Vector3 v) => v + Vector3.right;

    public static Vector3 ToTargetFromDirection(this Vector3 start, InputDir inputDir) {
        switch (inputDir) {
            case InputDir.Up:
                return start.Up();
            case InputDir.Down:
                return start.Down();
            case InputDir.Left:
                return start.Left();
            case InputDir.Right:
                return start.Right();
        }
        
        throw new InvalidEnumArgumentException("Input direction not valid");
    }
}