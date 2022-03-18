using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Input;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Game;
class TestWindow : GameWindow
{
    public TestWindow() : base(ApplicationSettings.MakeGWS(), ApplicationSettings.MakeNWS())
    {

    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
            
    }
}

