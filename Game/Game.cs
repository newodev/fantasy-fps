using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using CSharp_ECS.ECSCore;
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
    private ECSClient client;
    // The window is the OpenTK object used for rendering, and getting input
    private Window window;
    // The renderer draws to the OpenTK window
    private Renderer renderer;
    // Wraps OpenTK input into a more usable system
    private Input input;

    public Game()
    {
        client = new();
        window = new();
        input = new();
        renderer = new();

        window.GameUpdate += OnGameUpdate;
    }

    public void OnGameUpdate(object? s, double deltaTime)
    {
        input.Update(deltaTime, window.KeyboardState, window.MouseState);
    }
}
