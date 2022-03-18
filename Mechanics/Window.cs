using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Input;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Game
{
    class Window : GameWindow
    {
        public event EventHandler<double> GameUpdate;


        public Window() : base(ApplicationSettings.MakeGWS(), ApplicationSettings.MakeNWS())
        {
            
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            // Invoke the game update event, which is used to run the game loop.
            GameUpdate.Invoke(this, args.Time);
        }
    }
}
