using Godot;
using System;

namespace Satyrs_Greenwood
{
    public class SceneUtilities : Node
    {
        public PackedScene newScene;

        public void CleanPreviousScenes(Spatial referenceScene)
        {
            Node previousScene = referenceScene.GetTree().Root.GetChild(0);
            /*
            var props = GetProperties(previousScene);
            if (props.Count > 0)
            {
                PrintObjectProperties("previousScene", introTimer);
            }
            */
            referenceScene.GetTree().Root.RemoveChild(previousScene);
            //previousScene.Free();
        }

        public void ChangeScene(Spatial referenceScene, string scenePath)
        {
            newScene = (PackedScene)ResourceLoader.Load(scenePath);
            if (newScene != null)
                referenceScene.GetTree().Root.AddChild(newScene.Instance());
        }

        public static void ExitApplication(Spatial referenceScene)
        {
            referenceScene.GetTree().Quit();
        }

        public static void SetCameraPosition(Camera aCamera, Vector3 newPosition)
        {
            if ((aCamera != null) && (newPosition != null))
            {
                aCamera.Translation = newPosition;
            }
        }

        public static Godot.Vector3 CalculateCirclePosition(Godot.Vector3 circlePosition, float radiusCircle, float angle)
        {
            Godot.Vector3 res = new Vector3();
            if ((circlePosition != null) && (radiusCircle >= 0.1f))
            {

            }
            return res;
        }

        public static void MoveCameraAroundPosition(Camera aCamera, Godot.Vector3 circlePosition, float radiusCircle, int step)
        {
            Godot.Vector3 circleCoord = CalculateCirclePosition(circlePosition, radiusCircle, step / 1.0f);
            aCamera.Translation = circleCoord;
        }
    }
}
