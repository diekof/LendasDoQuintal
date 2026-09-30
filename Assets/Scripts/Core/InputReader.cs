using UnityEngine;

namespace LendasDoQuintal.Core
{
    public static class InputReader
    {
        public static float MoveX()
        {
            return Input.GetAxisRaw("Horizontal");
        }

        public static bool RunHeld()
        {
            return Input.GetKey(KeyCode.LeftShift) ||
                Input.GetKey(KeyCode.RightShift) ||
                Input.GetKey(KeyCode.JoystickButton4) ||
                Input.GetKey(KeyCode.JoystickButton5);
        }

        public static bool JumpPressed()
        {
            return Input.GetButtonDown("Jump") ||
                Input.GetKeyDown(KeyCode.Space) ||
                Input.GetKeyDown(KeyCode.JoystickButton0);
        }

        public static bool AttackPressed()
        {
            return Input.GetKeyDown(KeyCode.J) ||
                Input.GetKeyDown(KeyCode.JoystickButton2);
        }

        public static bool InteractPressed()
        {
            return Input.GetKeyDown(KeyCode.E) ||
                Input.GetKeyDown(KeyCode.JoystickButton3);
        }

        public static bool RollPressed()
        {
            return Input.GetKeyDown(KeyCode.K) ||
                Input.GetKeyDown(KeyCode.JoystickButton1);
        }

        public static bool RestartPressed()
        {
            return Input.GetKeyDown(KeyCode.R) ||
                Input.GetKeyDown(KeyCode.JoystickButton7);
        }
    }
}
