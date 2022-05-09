using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using CSharp_ECS;
using Game.InputDevices;

using Game.Rendering;

namespace Game;
/*
 * Game:
 *  - ECSClient
 *      - Server Connections (updates ECSWorld based on server states)
 *      - ECSWorld
 *          - Regions of interest to player
 *          - Local prediction systems
 *  - Window
 *  - Input
 *  - Audio
 *  - Renderer
 *  
 * Server Architecture:
 *  - ECSServer
 *  - ECSServer
 *  - ECSServer
 *  - ECSServer
 *      - Client Connections (gathers client input, sends relevant updates to clients)
 *      - ECSWorld
 *          - Regions managed by server
 *          - Game systems
 *  - ECSUniverse
 *      - Holds entire game state
 *      - used to mediate connections between neighbouring servers
*/
class GameClient
{
    // The class that runs the ECS game world
    private ECSClient client;
    // The window is the OpenTK object used for rendering, and getting input
    private Window window = new();
    // The renderer draws to the OpenTK window
    private Renderer renderer = new();

    private TestScene initialiser = new TestScene();

    public GameClient()
    {
        window.GameUpdate += OnGameUpdate;
        window.FrameUpdate += OnFrameUpdate;

        Input.Init();

        renderer.Init();

        initialiser.Rend(renderer);
        client = new(initialiser.InitSystems());
        client.Init(initialiser);

        window.Run();
    }

    public void OnGameUpdate(object? s, double deltaTime)
    {
        Input.Update(deltaTime, window.KeyboardState, window.MouseState);
        renderer.Update();

        // Update all ECS systems
        client.Update();

        Time.UpdateTime((float)deltaTime);
    }

    public void OnFrameUpdate(object? s, double deltaTime)
    {
        client.FrameUpdate();

        renderer.Render();

        window.SwapBuffers();
    }
}
