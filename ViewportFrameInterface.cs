using Godot;
using System;

namespace Satyrs_Greenwood
{
    public class ViewportFrameInterface : Viewport
    {
        public ViewportFrameInterface(Vector2 viewExtent)
        {
            Size = viewExtent;
            Disable3d = false;
            Keep3dLinear = true;
            Usage = UsageEnum.Usage2d;
            RenderDirectToScreen = false;
        }

        public void SetLinkedNode()
        {
            //World2d = new World2D
        }

        public static void HookupExistingNode(Viewport linkView, Node2D canvasDraw)
        {
            if ( (linkView != null) && (canvasDraw != null) ) {
                linkView.Disable3d = false;
                linkView.Keep3dLinear = true;
                linkView.Usage = UsageEnum.Usage2d;
                linkView.World2d = canvasDraw.GetWorld2d();
            }
        }
    }
}
