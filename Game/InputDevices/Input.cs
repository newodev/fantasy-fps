using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Input;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Game.InputDevices;

static class Input
{
    private static Keys[] KeyList = (Keys[]) Enum.GetValues(typeof(Keys));
    private static MouseButton[] MBList = (MouseButton[]) Enum.GetValues(typeof(MouseButton));
    private static InputAction[] ActionList = (InputAction[]) Enum.GetValues(typeof(InputAction));

    public static List<KeyBinding> Keybindings = new List<KeyBinding>();

    // How long an input has been held down for, in seconds
    private static Dictionary<InputAction, double> KeyHeld = new Dictionary<InputAction, double>();
    // Whether an input was first clicked in this frame
    private static Dictionary<InputAction, bool> KeyPressed = new Dictionary<InputAction, bool>();
    // Whether an input was released in this frame
    private static Dictionary<InputAction, bool> KeyReleased = new Dictionary<InputAction, bool>();

    public static Vector2 MouseDelta { get; private set; }
    private static Vector2 LastPos = Vector2.Zero;

    public static void Init()
    {
        KeyBinding k = new(InputAction.Forward);
        k.Add(Keys.W);
        Keybindings.Add(k);

        k = new(InputAction.Backward);
        k.Add(Keys.S);
        Keybindings.Add(k);

        k = new(InputAction.Left);
        k.Add(Keys.A);
        Keybindings.Add(k);

        k = new(InputAction.Right);
        k.Add(Keys.D);
        Keybindings.Add(k);

        k = new(InputAction.Secondary);
        k.Add(MouseButton.Right);
        Keybindings.Add(k);

        PopulateDictionaries();
    }

    private static void PopulateDictionaries()
    {
        for (int i = 0; i < ActionList.Length; i++)
        {
            KeyHeld.Add(ActionList[i], 0);
            KeyPressed.Add(ActionList[i], false);
            KeyReleased.Add(ActionList[i], false);
        }
    }

    public static void Update(double deltaTime, KeyboardState kb, MouseState mouse)
    {
        UpdateKeyboard(deltaTime, kb);
        UpdateMouse(deltaTime, mouse);
    }

    private static void UpdateKeyboard(double deltaTime, KeyboardState kb)
    {
        for (int i = 0; i < KeyList.Length; i++)
        {
            KeyBinding bind = GetKeyBinding(KeyList[i]);
            if (bind == null)
                continue;

            if (kb.IsKeyDown(KeyList[i]))
            {
                KeyHeld[bind.action] += deltaTime;
            }
            else
            {
                KeyHeld[bind.action] = 0;
            }

            KeyPressed[bind.action] = kb.IsKeyPressed(KeyList[i]);
            KeyReleased[bind.action] = kb.IsKeyReleased(KeyList[i]);
        }
    }

    private static void UpdateMouse(double deltaTime, MouseState mouse)
    {
        for (int i = 0; i < MBList.Length; i++)
        {
            KeyBinding bind = GetKeyBinding(MBList[i]);
            if (bind == null)
                continue;

            if (mouse.IsButtonDown(MBList[i]))
            {
                KeyHeld[bind.action] += deltaTime;
            }
            else
            {
                KeyHeld[bind.action] = 0;
            }

            KeyPressed[bind.action] = mouse.IsButtonDown(MBList[i]) && !mouse.WasButtonDown(MBList[i]);
            KeyReleased[bind.action] = !mouse.IsButtonDown(MBList[i]) && mouse.WasButtonDown(MBList[i]);
        }

        MouseDelta = mouse.Position - LastPos;
        LastPos = mouse.Position;
    }

    private static KeyBinding? GetKeyBinding(Keys key)
    {
        for (int i = 0; i < Keybindings.Count; i++)
        {
            if (Keybindings[i].Keys.Contains(key))
                return Keybindings[i];
        }

        return null;
    }

    private static KeyBinding? GetKeyBinding(MouseButton mb)
    {
        for (int i = 0; i < Keybindings.Count; i++)
        {
            if (Keybindings[i].MouseButtons.Contains(mb))
                return Keybindings[i];
        }

        return null;
    }


    public static double GetKeyHeld(InputAction i) => KeyHeld[i];
    public static bool GetKeyPressed(InputAction i) => KeyPressed[i];
    public static bool GetKeyReleased(InputAction i) => KeyReleased[i];
}

public class KeyBinding
{
    // The mouse buttons and keys used by this binding
    public List<MouseButton> MouseButtons = new List<MouseButton>();
    public List<Keys> Keys = new List<Keys>();

    public readonly InputAction action;

    public KeyBinding(InputAction ia)
    {
        action = ia;
    }

    public void Add(Keys key) => Keys.Add(key);
    public void Add(MouseButton mb) => MouseButtons.Add(mb);
    public void Remove(Keys key) => Keys.Remove(key);
    public void Remove(MouseButton mb) => MouseButtons.Remove(mb);
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
