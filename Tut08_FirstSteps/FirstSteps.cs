using System;
using System.Collections.Generic;
using System.Linq;
using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;
using static System.Math;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;

namespace Fusee.Tutorial.Core
{
    public class FirstSteps : RenderCanvas
    {
        private SceneContainer _scene;
        private SceneRenderer _sceneRenderer;
        private float _camAngle = 0;
        private const int _count = 9; //amount of cubes
        private TransformComponent[] _cubeTransform = new TransformComponent[_count];
        private ShaderEffectComponent[] _cubeShader = new ShaderEffectComponent[_count];

        // Init is called on startup. 
        public override void Init()
        {
            // Set the clear color for the backbuffer to light green (intensities in R, G, B, A).
            RC.ClearColor = new float4(0.1f, 0.1f, 0.1f, 1);

            float cubeSpace = (float)1 / _count * 50; //cube width with space between
            float cubeSize = cubeSpace * 0.75f; //cube width
            Mesh cubeMesh = SimpleMeshes.CreateCuboid(new float3(cubeSize, cubeSize, cubeSize)); //cube mesh

            // Create the scene containing the cube as the only object
            _scene = new SceneContainer();
            _scene.Children = new List<SceneNodeContainer>();

            // Create a scene with a cube
            // The three components: one XForm, one Shader and the Mesh
            float center = -(cubeSpace * _count) / 2 + cubeSpace / 2; //leftmost x-value to display cubes in center
            Diagnostics.Log("center " + center);
            for (int i = 0; i < _count; i++)
            {
                _cubeTransform[i] = new TransformComponent //Transformation
                {
                    Scale = new float3(1, 1, 1),
                    Translation = new float3(center + i * cubeSpace, 0, 0),
                    Rotation = new float3(0, 0, 0)
                };
                Diagnostics.Log(i + " " + _cubeTransform[i].Translation.x);

                float color = (float)(i + 1) / _count; //color of cubes, gray levels between ]black,white]

                _cubeShader[i] = new ShaderEffectComponent //Shader
                {
                    Effect = SimpleMeshes.MakeShaderEffect(new float3(color, color, color), new float3(1, 1, 1), 4)
                };

                // Assemble the cube node containing the three components
                var cubeNode = new SceneNodeContainer();
                cubeNode.Components = new List<SceneComponentContainer>();
                cubeNode.Components.Add(_cubeTransform[i]);
                cubeNode.Components.Add(_cubeShader[i]);
                cubeNode.Components.Add(cubeMesh);
                _scene.Children.Add(cubeNode);
            }

            // Create a scene renderer holding the scene above
            _sceneRenderer = new SceneRenderer(_scene);
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            // Setup the camera 
            RC.View = float4x4.CreateTranslation(0, 0, 50) * float4x4.CreateRotationY(_camAngle);

            // Animate the cube
            for (int i = 0; i < _count; i++)
            {
                int rotationDirection = -1;
                if (i % 2 == 0) rotationDirection = 1; //if index is even then rotation direction is positive, if it's odd then direction is negative
                float amplitude = 10 * M.Sin((float)i / (_count - 1) * 2 * M.Pi); //makes line of cubes swing as sine
                float scale = (float)1 / 20 * M.Max(_cubeTransform[i].Translation.y, -1 * _cubeTransform[i].Translation.y) + 1; //substitute for M.Abs (which doesn't seem to exist)

                _cubeTransform[i].Scale.y = scale;
                _cubeTransform[i].Scale.z = scale;
                _cubeTransform[i].Translation.y = amplitude * M.Sin(3 * TimeSinceStart);
                _cubeTransform[i].Rotation.x += rotationDirection * 90.0f * M.Pi / 180.0f * DeltaTime;
            }

            // Animate the camera angle (45 deg per second)
            _camAngle += 45.0f * M.Pi / 180.0f * DeltaTime;

            // Render the scene on the current render context
            _sceneRenderer.Render(RC);

            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.
            Present();
        }


        // Is called when the window was resized
        public override void Resize()
        {
            // Set the new rendering area to the entire new windows size
            RC.Viewport(0, 0, Width, Height);

            // Create a new projection matrix generating undistorted images on the new aspect ratio.
            var aspectRatio = Width / (float)Height;

            // 0.25*PI Rad -> 45° Opening angle along the vertical direction. Horizontal opening angle is calculated based on the aspect ratio
            // Front clipping happens at 1 (Objects nearer than 1 world unit get clipped)
            // Back clipping happens at 2000 (Anything further away from the camera than 2000 world units gets clipped, polygons will be cut)
            var projection = float4x4.CreatePerspectiveFieldOfView(M.PiOver4, aspectRatio, 1, 20000);
            RC.Projection = projection;
        }
    }
}