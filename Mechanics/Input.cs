using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Input;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Game
{
    class Input
    {
        private Keys[] KeyList;

        public Dictionary<InputAction, double> KeyHeld = new Dictionary<InputAction, double>();
        public Dictionary<Keys, InputAction> KeyPressed = new Dictionary<Keys,InputAction>();
        public Input()
        {
            
        }

        public void Update(double deltaTime, KeyboardState kb, MouseState mouse)
        {

        }
    }

    public enum InputAction
    {
        Forward,
        Backward,
        Left,
        Right,
        Primary,
        Secondary,
        Equip,
    };
}
