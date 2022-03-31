using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using CSharp_ECS.ECSCore;

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
 *      - used to mediate connections between servers that interact
*/
class Game
{
    // The class that runs the ECS game world
    private ECSClient client = new();
    // The window is the OpenTK object used for rendering, and getting input
    private Window window = new();
    // The renderer draws to the OpenTK window
    private Renderer renderer = new();
    // Wraps OpenTK input into a more usable system
    private Input input = new();

    public Game()
    {
        window.GameUpdate += OnGameUpdate;
        window.FrameUpdate += OnFrameUpdate;
    }

    public void OnGameUpdate(object? s, double deltaTime)
    {
        input.Update(deltaTime, window.KeyboardState, window.MouseState);
        renderer.Update();
        // Update all gameplay ECS systems
    }

    public void OnFrameUpdate(object? s, double deltaTime)
    {
        // Update ECS RenderSystem.

        renderer.Render();
        window.SwapBuffers();
    }
}
