using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Rendering;
class RenderObject
{
    public int EntityID { get; set; }
    // Transform

    // Previous Transform
}

class Renderer
{
    // Contains the renderableID of each entity
    private Dictionary<int, int> entities = new();
    // RenderObjects sorted by renderable
    private Dictionary<int, List<RenderObject>> renderObjects = new();



    public void AddObject(int renderableID, int entityID /*, Transform t*/)
    {
        if(entities.ContainsKey(entityID))
        {
            // Move current transform to previous, and new to current.
            

            return;
        }
        
        // This entity hasn't been registered to the Renderer recently.
    }

    private void UpdateTransform(int renderableID, int entityID /*, Transform t*/)
    {

    }

    public void Update()
    {
        // Render each renderable

    }
}
