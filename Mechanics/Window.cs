using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Windowing.Desktop;

namespace Game
{
    class Window : GameWindow
    {

        public Window() : base(ApplicationSettings.MakeGWS(), ApplicationSettings.MakeNWS())
        {
            
        }
    }
}
