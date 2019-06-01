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
    public class HierarchyInput : RenderCanvas
    {
        private SceneContainer _scene;
        private SceneRenderer _sceneRenderer;
        private float _camAngle = 0;
        private float _camVelocity = 0;
        private TransformComponent _baseTransform;
        private TransformComponent _bodyTransform;
        private TransformComponent _upperArmJointTransform;
        private TransformComponent _upperArmTransform;
        private TransformComponent _foreArmJointTransform;
        private TransformComponent _foreArmTransform;
        private TransformComponent _prehensileRailTransform;
        private TransformComponent _prehensileClawLeftTransform;
        private TransformComponent _prehensileClawRightTransform;
        private float _clawsState = 0;
        private float _clawsVelocity = 0;
        private bool _hasEndState = true;

        SceneContainer CreateScene()
        {
            // Initialize Transform Components that need to be changed inside "RenderAFrame"
            _baseTransform = new TransformComponent
            {
                Rotation = new float3(0, M.Pi / 2, 0),
                Scale = new float3(1, 1, 1),
                Translation = new float3(0, 0, 0)
            };
            _bodyTransform = new TransformComponent
            {
                Rotation = new float3(0, 0, 0),
                Scale = new float3(1, 1, 1),
                Translation = new float3(0, 6, 0)
            };
            _upperArmJointTransform = new TransformComponent
            {
                Rotation = new float3(M.Pi / 4, 0, 0),
                Scale = new float3(1, 1, 1),
                Translation = new float3(2, 4, 0)
            };
            _upperArmTransform = new TransformComponent
            {
                Rotation = new float3(0, 0, 0),
                Scale = new float3(1, 1, 1),
                Translation = new float3(0, 4, 0)
            };
            _foreArmJointTransform = new TransformComponent
            {
                Rotation = new float3(M.Pi / 4, 0, 0),
                Scale = new float3(1, 1, 1),
                Translation = new float3(-2, 4, 0)
            };
            _foreArmTransform = new TransformComponent
            {
                Rotation = new float3(0, 0, 0),
                Scale = new float3(1, 1, 1),
                Translation = new float3(0, 4, 0)
            };
            _prehensileRailTransform = new TransformComponent
            {
                Rotation = new float3(0, 0, 0),
                Scale = new float3(1, 1, 1),
                Translation = new float3(0, 5.5f, 0)
            };
            _prehensileClawLeftTransform = new TransformComponent
            {
                Rotation = new float3(0, 0, 0),
                Scale = new float3(1, 1, 1),
                Translation = new float3(-2.5f, 3, 0)
            };
            _prehensileClawRightTransform = new TransformComponent
            {
                Rotation = new float3(0, 0, 0),
                Scale = new float3(1, 1, 1),
                Translation = new float3(2.5f, 3, 0)
            };

            // Shader Effect Components
            var baseShader = new ShaderEffectComponent
            {
                Effect = SimpleMeshes.MakeShaderEffect(new float3(0.7f, 0.7f, 0.7f), new float3(0.7f, 0.7f, 0.7f), 5)
            };
            var bodyShader = new ShaderEffectComponent
            {
                Effect = SimpleMeshes.MakeShaderEffect(new float3(0.8f, 0.2f, 0.2f), new float3(0.7f, 0.7f, 0.7f), 5)
            };
            var upperArmShader = new ShaderEffectComponent
            {
                Effect = SimpleMeshes.MakeShaderEffect(new float3(0.2f, 0.8f, 0.2f), new float3(0.7f, 0.7f, 0.7f), 5)
            };
            var foreArmShader = new ShaderEffectComponent
            {
                Effect = SimpleMeshes.MakeShaderEffect(new float3(0.2f, 0.2f, 0.8f), new float3(0.7f, 0.7f, 0.7f), 5)
            };
            var prehensileRailShader = new ShaderEffectComponent
            {
                Effect = SimpleMeshes.MakeShaderEffect(new float3(0.8f, 0.8f, 0.8f), new float3(0.7f, 0.7f, 0.7f), 5)
            };
            var prehensileClawShader = new ShaderEffectComponent
            {
                Effect = SimpleMeshes.MakeShaderEffect(new float3(0.6f, 0.6f, 0.6f), new float3(0.7f, 0.7f, 0.7f), 5)
            };

            // Mesh components
            var baseMesh = SimpleMeshes.CreateCuboid(new float3(10, 2, 10));
            var armMesh = SimpleMeshes.CreateCuboid(new float3(2, 10, 2));
            var railMesh = SimpleMeshes.CreateCuboid(new float3(6, 1, 2));
            var clawMesh = SimpleMeshes.CreateCuboid(new float3(1, 5, 2));

            // Setup the scene graph
            return new SceneContainer
            {
                Children = new List<SceneNodeContainer>
                {
                    new SceneNodeContainer
                    {
                        Components = new List<SceneComponentContainer>
                        {
                            _baseTransform,
                            baseShader,
                            baseMesh
                        },
                        Children = new List<SceneNodeContainer>
                        {
                            new SceneNodeContainer
                            {
                                Components = new List<SceneComponentContainer>
                                {
                                    _bodyTransform,
                                    bodyShader,
                                    armMesh
                                },
                                Children = new List<SceneNodeContainer>
                                {
                                    new SceneNodeContainer
                                    {
                                        Components = new List<SceneComponentContainer>
                                        {
                                            _upperArmJointTransform
                                        },
                                        Children = new List<SceneNodeContainer>
                                        {
                                            new SceneNodeContainer
                                            {
                                                Components = new List<SceneComponentContainer>
                                                {
                                                    _upperArmTransform,
                                                    upperArmShader,
                                                    armMesh
                                                },
                                                Children = new List<SceneNodeContainer>
                                                {
                                                    new SceneNodeContainer
                                                    {
                                                        Components = new List<SceneComponentContainer>
                                                        {
                                                            _foreArmJointTransform
                                                        },
                                                        Children = new List<SceneNodeContainer>
                                                        {
                                                            new SceneNodeContainer
                                                            {
                                                                Components = new List<SceneComponentContainer>
                                                                {
                                                                    _foreArmTransform,
                                                                    foreArmShader,
                                                                    armMesh
                                                                },
                                                                Children = new List<SceneNodeContainer>
                                                                {
                                                                    new SceneNodeContainer
                                                                    {
                                                                        Components = new List<SceneComponentContainer>
                                                                        {
                                                                            _prehensileRailTransform,
                                                                            prehensileRailShader,
                                                                            railMesh
                                                                        },
                                                                        Children = new List<SceneNodeContainer>
                                                                        {
                                                                            new SceneNodeContainer
                                                                            {
                                                                                Components = new List<SceneComponentContainer>
                                                                                {
                                                                                    _prehensileClawLeftTransform,
                                                                                    prehensileClawShader,
                                                                                    clawMesh
                                                                                }
                                                                            },
                                                                            new SceneNodeContainer
                                                                            {
                                                                                Components = new List<SceneComponentContainer>
                                                                                {
                                                                                    _prehensileClawRightTransform,
                                                                                    prehensileClawShader,
                                                                                    clawMesh
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }

        // Init is called on startup. 
        public override void Init()
        {
            // Set the clear color for the backbuffer to white (100% intensity in all color channels R, G, B, A).
            RC.ClearColor = new float4(0.8f, 0.9f, 0.7f, 1);

            _scene = CreateScene();

            // Create a scene renderer holding the scene above
            _sceneRenderer = new SceneRenderer(_scene);
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            // Keyboard Controls
            float upperArmRot = _upperArmJointTransform.Rotation.x;
            float foreArmRot = _foreArmJointTransform.Rotation.x;
            float maxRotAngle = M.Pi / 2;

            _bodyTransform.Rotation.y += DeltaTime * Keyboard.ADAxis;

            if ((upperArmRot < maxRotAngle || Keyboard.WSAxis < 0) && (upperArmRot > -maxRotAngle || Keyboard.WSAxis > 0))
            {
                _upperArmJointTransform.Rotation.x += DeltaTime * Keyboard.WSAxis;
            }
            if ((foreArmRot < maxRotAngle || Keyboard.UpDownAxis < 0) && (foreArmRot > -maxRotAngle || Keyboard.UpDownAxis > 0))
            {
                _foreArmJointTransform.Rotation.x += DeltaTime * Keyboard.UpDownAxis;
            }

            _prehensileRailTransform.Rotation.y += DeltaTime * Keyboard.LeftRightAxis;


            // Mouse Controls
            if (Mouse.LeftButton)
            {
                _camVelocity = DeltaTime * Mouse.Velocity.x * 0.005f;
            }
            else
            {
                _camVelocity -= DeltaTime * _camVelocity * 2.5f;
            }

            // Keyboard Controls
            if (Keyboard.GetKey(KeyCodes.E))
            {
                if (_hasEndState)
                {
                    if (_clawsState == 0)
                    {
                        _clawsVelocity = 1;
                    }
                    else if (_clawsState == 1)
                    {
                        _clawsVelocity = -1;
                    }
                    _hasEndState = false;
                }
            }

            _clawsState += DeltaTime * _clawsVelocity;

            if (_clawsState < 0 || _clawsState > 1)
            {
                _clawsState = M.Max(0, M.Min(1, _clawsState));
                _clawsVelocity = 0;
                _hasEndState = true;
            }

            _prehensileClawLeftTransform.Translation.x = _clawsState * (-2.0f) + 2.5f;
            _prehensileClawRightTransform.Translation.x = _clawsState * 2.0f - 2.5f;

            _camAngle += _camVelocity;

            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            // Setup the camera 
            RC.View = float4x4.CreateTranslation(0, -10, 50) * float4x4.CreateRotationY(_camAngle);

            // Render the scene on the current render context
            _sceneRenderer.Render(RC);

            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered farame) on the front buffer.
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