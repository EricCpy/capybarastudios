//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.4.3
//     from Assets/Input/Player Input.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @PlayerInput : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerInput()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Player Input"",
    ""maps"": [
        {
            ""name"": ""Walking"",
            ""id"": ""1371f36a-7e82-47b1-869f-8e96fbaeda41"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""60c3b06b-adde-4c18-8fe1-84fc041ac272"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""37390265-7030-48a0-9eb8-1b82fa793227"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""LookAround"",
                    ""type"": ""Value"",
                    ""id"": ""28957072-9f0d-460c-af45-002a62fbed84"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Crouch"",
                    ""type"": ""Button"",
                    ""id"": ""9b9c1c8b-e25a-4a8c-8dd8-244813a1807a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Sprint"",
                    ""type"": ""Button"",
                    ""id"": ""01e95d8a-c6d1-4db8-9032-7bef8d8637db"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""2D WASD"",
                    ""id"": ""1b92611c-2f58-4974-a936-b8a8da7c47de"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""6bc02c51-c202-468d-9fe9-f2d3cc007356"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""2248d8a5-edc8-4d4c-bab2-d0eecb576e24"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""69842ab8-978f-4f90-9831-32dff0d75708"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""c2dbf88b-e8fa-4f3e-8135-e372c4cb3996"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""2D Controller"",
                    ""id"": ""c50f54f4-f00f-468c-8279-df32fabb4b12"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""30f7290f-ce43-47b0-8fc2-454010810681"",
                    ""path"": ""<Gamepad>/leftStick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""dd44f6f1-53f7-4e22-afad-96215456f199"",
                    ""path"": ""<Gamepad>/leftStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""736fed74-1650-497a-bb8e-a3edadc2154d"",
                    ""path"": ""<Gamepad>/leftStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""aa6284aa-8591-4c44-9a80-76eeeed8dec9"",
                    ""path"": ""<Gamepad>/leftStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""0df65c4b-dc22-467f-a3d6-06f322940d5f"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""dc096096-4462-427c-a34e-bd0145613479"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c57f4677-66f3-44ea-8a7d-bd5b9749b5d9"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LookAround"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4fa55ebb-ac8c-4d83-9968-739b813e9191"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LookAround"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f477016f-5ebc-40a7-94d1-849d1a7046bf"",
                    ""path"": ""<Keyboard>/leftCtrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Crouch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""49230136-99bf-4b7f-a326-356206fe4703"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Crouch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""95002667-f7a8-4bef-b4e9-49bc497ff9ec"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": ""Hold"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Sprint"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ebf470db-fcb3-4428-9743-6e257f6a8e00"",
                    ""path"": ""<Gamepad>/leftStickPress"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Sprint"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Shooting"",
            ""id"": ""e8075fd3-5e50-427e-a8be-25b917a1d928"",
            ""actions"": [
                {
                    ""name"": ""Shoot"",
                    ""type"": ""Button"",
                    ""id"": ""19b2281c-4ede-463d-bd55-61e23bfce5c5"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""afb5dc6f-50d8-4ff9-9398-fa83f5573eb7"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1a27c542-bb4e-402a-a171-96d4dee0fc48"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Walking
        m_Walking = asset.FindActionMap("Walking", throwIfNotFound: true);
        m_Walking_Movement = m_Walking.FindAction("Movement", throwIfNotFound: true);
        m_Walking_Jump = m_Walking.FindAction("Jump", throwIfNotFound: true);
        m_Walking_LookAround = m_Walking.FindAction("LookAround", throwIfNotFound: true);
        m_Walking_Crouch = m_Walking.FindAction("Crouch", throwIfNotFound: true);
        m_Walking_Sprint = m_Walking.FindAction("Sprint", throwIfNotFound: true);
        // Shooting
        m_Shooting = asset.FindActionMap("Shooting", throwIfNotFound: true);
        m_Shooting_Shoot = m_Shooting.FindAction("Shoot", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }
    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Walking
    private readonly InputActionMap m_Walking;
    private IWalkingActions m_WalkingActionsCallbackInterface;
    private readonly InputAction m_Walking_Movement;
    private readonly InputAction m_Walking_Jump;
    private readonly InputAction m_Walking_LookAround;
    private readonly InputAction m_Walking_Crouch;
    private readonly InputAction m_Walking_Sprint;
    public struct WalkingActions
    {
        private @PlayerInput m_Wrapper;
        public WalkingActions(@PlayerInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_Walking_Movement;
        public InputAction @Jump => m_Wrapper.m_Walking_Jump;
        public InputAction @LookAround => m_Wrapper.m_Walking_LookAround;
        public InputAction @Crouch => m_Wrapper.m_Walking_Crouch;
        public InputAction @Sprint => m_Wrapper.m_Walking_Sprint;
        public InputActionMap Get() { return m_Wrapper.m_Walking; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(WalkingActions set) { return set.Get(); }
        public void SetCallbacks(IWalkingActions instance)
        {
            if (m_Wrapper.m_WalkingActionsCallbackInterface != null)
            {
                @Movement.started -= m_Wrapper.m_WalkingActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_WalkingActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_WalkingActionsCallbackInterface.OnMovement;
                @Jump.started -= m_Wrapper.m_WalkingActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_WalkingActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_WalkingActionsCallbackInterface.OnJump;
                @LookAround.started -= m_Wrapper.m_WalkingActionsCallbackInterface.OnLookAround;
                @LookAround.performed -= m_Wrapper.m_WalkingActionsCallbackInterface.OnLookAround;
                @LookAround.canceled -= m_Wrapper.m_WalkingActionsCallbackInterface.OnLookAround;
                @Crouch.started -= m_Wrapper.m_WalkingActionsCallbackInterface.OnCrouch;
                @Crouch.performed -= m_Wrapper.m_WalkingActionsCallbackInterface.OnCrouch;
                @Crouch.canceled -= m_Wrapper.m_WalkingActionsCallbackInterface.OnCrouch;
                @Sprint.started -= m_Wrapper.m_WalkingActionsCallbackInterface.OnSprint;
                @Sprint.performed -= m_Wrapper.m_WalkingActionsCallbackInterface.OnSprint;
                @Sprint.canceled -= m_Wrapper.m_WalkingActionsCallbackInterface.OnSprint;
            }
            m_Wrapper.m_WalkingActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @LookAround.started += instance.OnLookAround;
                @LookAround.performed += instance.OnLookAround;
                @LookAround.canceled += instance.OnLookAround;
                @Crouch.started += instance.OnCrouch;
                @Crouch.performed += instance.OnCrouch;
                @Crouch.canceled += instance.OnCrouch;
                @Sprint.started += instance.OnSprint;
                @Sprint.performed += instance.OnSprint;
                @Sprint.canceled += instance.OnSprint;
            }
        }
    }
    public WalkingActions @Walking => new WalkingActions(this);

    // Shooting
    private readonly InputActionMap m_Shooting;
    private IShootingActions m_ShootingActionsCallbackInterface;
    private readonly InputAction m_Shooting_Shoot;
    public struct ShootingActions
    {
        private @PlayerInput m_Wrapper;
        public ShootingActions(@PlayerInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @Shoot => m_Wrapper.m_Shooting_Shoot;
        public InputActionMap Get() { return m_Wrapper.m_Shooting; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(ShootingActions set) { return set.Get(); }
        public void SetCallbacks(IShootingActions instance)
        {
            if (m_Wrapper.m_ShootingActionsCallbackInterface != null)
            {
                @Shoot.started -= m_Wrapper.m_ShootingActionsCallbackInterface.OnShoot;
                @Shoot.performed -= m_Wrapper.m_ShootingActionsCallbackInterface.OnShoot;
                @Shoot.canceled -= m_Wrapper.m_ShootingActionsCallbackInterface.OnShoot;
            }
            m_Wrapper.m_ShootingActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Shoot.started += instance.OnShoot;
                @Shoot.performed += instance.OnShoot;
                @Shoot.canceled += instance.OnShoot;
            }
        }
    }
    public ShootingActions @Shooting => new ShootingActions(this);
    public interface IWalkingActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnLookAround(InputAction.CallbackContext context);
        void OnCrouch(InputAction.CallbackContext context);
        void OnSprint(InputAction.CallbackContext context);
    }
    public interface IShootingActions
    {
        void OnShoot(InputAction.CallbackContext context);
    }
}
