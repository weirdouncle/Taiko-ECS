// GENERATED AUTOMATICALLY FROM 'Assets/WASD.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using static GameSetting;

public class WASDInput : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public WASDInput()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""WASD"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""6b6968b9-f202-4311-9397-985b975b0bee"",
            ""actions"": [
                {
                    ""name"": ""Up"",
                    ""type"": ""Button"",
                    ""id"": ""b61c2d39-23f4-4de9-9526-bcbcd2f77f17"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Down"",
                    ""type"": ""Button"",
                    ""id"": ""6427d61a-b6bd-4a0b-8bb9-038cd1abc8a8"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Left"",
                    ""type"": ""Button"",
                    ""id"": ""c2f9c38b-1550-4878-b592-07137fa101fb"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Right"",
                    ""type"": ""Button"",
                    ""id"": ""4e91d8bd-a2b9-4a4c-b44f-48881aca0ce8"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""F1"",
                    ""type"": ""Button"",
                    ""id"": ""28a7d2d1-9601-4e5a-a602-09c2a9cba7b0"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Enter"",
                    ""type"": ""Button"",
                    ""id"": ""84723355-8efd-4939-975a-96e1b1b78c8f"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Esc"",
                    ""type"": ""Button"",
                    ""id"": ""0fef48a1-7e5e-4139-b479-3b2f96af7465"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""LeftRelease"",
                    ""type"": ""Button"",
                    ""id"": ""2ed526bf-0da3-425a-a21b-dce328866c97"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""RightRelease"",
                    ""type"": ""Button"",
                    ""id"": ""f2969acb-9e7d-4180-92a3-4f8b677cb580"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ScreenShot"",
                    ""type"": ""Button"",
                    ""id"": ""e794d641-058c-427e-b715-d2478eeb7fdb"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Random"",
                    ""type"": ""PassThrough"",
                    ""id"": ""e55a3de9-d971-49a4-81e4-df7b7aa55c17"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""PadAny"",
                    ""type"": ""Button"",
                    ""id"": ""6c61a20b-f3fa-4c31-be87-c60074314d91"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""97de8b46-1e61-4ed6-bb72-e8d0c395117b"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Up"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""54bc50a0-9865-4933-ae0d-999ed53abd99"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Down"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""17726a09-6b3a-4d7a-8d01-6a136aa7106a"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2574ae21-e24a-4167-b025-934165bf7c52"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Right"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""652b9f18-ef62-4e31-a569-7fc9edaa502e"",
                    ""path"": ""<Keyboard>/f1"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""F1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2e47b792-d44a-47b6-b5a7-bd121e9a4a4d"",
                    ""path"": ""<Keyboard>/enter"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Enter"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6a9105d2-36ef-40f0-aeab-0800ea4b36e0"",
                    ""path"": ""<Keyboard>/numpadEnter"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Enter"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""aef02f69-3d5c-41de-91ad-71b941c1cfe9"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": ""Press(behavior=1)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Esc"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4a3be7d7-df27-4672-8bb5-47b6bf8b4c1b"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": ""Press(behavior=1)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftRelease"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ddef9795-e2a3-4fe3-b238-0d026d8b33d6"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": ""Press(behavior=1)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RightRelease"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e7c6f336-409a-4f3f-b291-62101958529b"",
                    ""path"": ""<Keyboard>/printScreen"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ScreenShot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""72dc52df-41f9-488f-b770-6b982eaa4970"",
                    ""path"": ""<Keyboard>/f2"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Random"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2c2e7304-bdd4-4545-9209-967f0fc26390"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": ""Press(behavior=1)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PadAny"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_Up = m_Player.FindAction("Up", throwIfNotFound: true);
        m_Player_Down = m_Player.FindAction("Down", throwIfNotFound: true);
        m_Player_Left = m_Player.FindAction("Left", throwIfNotFound: true);
        m_Player_Right = m_Player.FindAction("Right", throwIfNotFound: true);
        m_Player_F1 = m_Player.FindAction("F1", throwIfNotFound: true);
        m_Player_Enter = m_Player.FindAction("Enter", throwIfNotFound: true);
        m_Player_Esc = m_Player.FindAction("Esc", throwIfNotFound: true);
        m_Player_LeftRelease = m_Player.FindAction("LeftRelease", throwIfNotFound: true);
        m_Player_RightRelease = m_Player.FindAction("RightRelease", throwIfNotFound: true);
        //m_Player_ScreenShot = m_Player.FindAction("ScreenShot", throwIfNotFound: true);
        //m_Player_ScreenShot.PerformInteractiveRebinding().WithControlsExcluding("<Keyboard>/printScreen");
        m_Player_Random = m_Player.FindAction("Random", throwIfNotFound: true);

        //left
        m_Player_LeftKa = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/D", interactions: "Press(pressPoint=1)");
        m_Player_LeftKaRelease = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/D", interactions: "Press(behavior=1)");

        //right
        m_Player_RightKa = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/K", interactions: "Press(pressPoint=1)");
        m_Player_RightKaRelease = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/K", interactions: "Press(behavior=1)");

        //confirm
        m_Player_LeftDon = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/F", interactions: "Press(behavior=1)");
        m_Player_RightDon = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/J", interactions: "Press(behavior=1)");
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
        m_Player_LeftDon.Enable();
        m_Player_RightDon.Enable();
        m_Player_RightKa.Enable();
        m_Player_RightKaRelease.Enable();
        m_Player_LeftKa.Enable();
        m_Player_LeftKaRelease.Enable();

        asset.Enable();
    }

    public void Disable()
    {
        m_Player_LeftDon.Disable();
        m_Player_RightDon.Disable();
        m_Player_RightKa.Disable();
        m_Player_RightKaRelease.Disable();
        m_Player_LeftKa.Disable();
        m_Player_LeftKaRelease.Disable();

        asset.Disable();
    }

    // Player
    private readonly InputActionMap m_Player;
    private readonly InputAction m_Player_Up;
    private readonly InputAction m_Player_Down;
    private readonly InputAction m_Player_Left;
    private readonly InputAction m_Player_Right;
    private readonly InputAction m_Player_F1;
    private readonly InputAction m_Player_Enter;
    private readonly InputAction m_Player_Esc;
    private InputAction m_Player_LeftRelease;
    private InputAction m_Player_RightRelease;
    private InputAction m_Player_Cancel;
    private InputAction m_Player_Option;
    private readonly InputAction m_Player_ScreenShot;
    private readonly InputAction m_Player_Random;
    private InputAction m_Player_Left2P;
    private InputAction m_Player_Right2P;
    private InputAction m_Player_Confirm2P;
    private readonly InputAction m_Player_LeftKa;
    private readonly InputAction m_Player_RightKa;
    private readonly InputAction m_Player_LeftKaRelease;
    private readonly InputAction m_Player_RightKaRelease;
    private InputAction m_Player_LeftDon;
    private InputAction m_Player_RightDon;

    public struct PlayerActions
    {
        private @WASDInput m_Wrapper;
        public PlayerActions(@WASDInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @Up => m_Wrapper.m_Player_Up;
        public InputAction @Down => m_Wrapper.m_Player_Down;
        public InputAction @Left => m_Wrapper.m_Player_Left;
        public InputAction @Right => m_Wrapper.m_Player_Right;
        public InputAction @LeftKa => m_Wrapper.m_Player_LeftKa;
        public InputAction @RightKa => m_Wrapper.m_Player_RightKa;
        public InputAction @F1 => m_Wrapper.m_Player_F1;
        public InputAction @Enter => m_Wrapper.m_Player_Enter;
        public InputAction @Esc => m_Wrapper.m_Player_Esc;
        public InputAction @LeftRelease => m_Wrapper.m_Player_LeftRelease;
        public InputAction @RightRelease => m_Wrapper.m_Player_RightRelease;
        public InputAction @LeftKaRelease => m_Wrapper.m_Player_LeftKaRelease;
        public InputAction @RightKaRelease => m_Wrapper.m_Player_RightKaRelease;
        public InputAction @LeftDon => m_Wrapper.m_Player_LeftDon;
        public InputAction @RightDon => m_Wrapper.m_Player_RightDon;
        public InputAction @Option => m_Wrapper.m_Player_Option;
        public InputAction @Cancel => m_Wrapper.m_Player_Cancel;
        public InputAction @ScreenShot => m_Wrapper.m_Player_ScreenShot;
        public InputAction @Random => m_Wrapper.m_Player_Random;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        
    }
    public PlayerActions @Player => new PlayerActions(this);
    
}
