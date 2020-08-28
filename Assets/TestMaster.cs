// GENERATED AUTOMATICALLY FROM 'Assets/TestMaster.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @TestMaster : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @TestMaster()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""TestMaster"",
    ""maps"": [
        {
            ""name"": ""AbortUI"",
            ""id"": ""e9bdcf8b-a11d-4a5a-9ef7-c78137ddb16d"",
            ""actions"": [
                {
                    ""name"": ""Abort"",
                    ""type"": ""Button"",
                    ""id"": ""91df5648-245f-43c5-be1d-d66d6065735f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""4e729dde-1bb0-41f7-acb5-d8b9d11b5bf6"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Abort"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c9a895f9-befa-4e1e-a81e-839ef6101ee8"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Abort"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // AbortUI
        m_AbortUI = asset.FindActionMap("AbortUI", throwIfNotFound: true);
        m_AbortUI_Abort = m_AbortUI.FindAction("Abort", throwIfNotFound: true);
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

    // AbortUI
    private readonly InputActionMap m_AbortUI;
    private IAbortUIActions m_AbortUIActionsCallbackInterface;
    private readonly InputAction m_AbortUI_Abort;
    public struct AbortUIActions
    {
        private @TestMaster m_Wrapper;
        public AbortUIActions(@TestMaster wrapper) { m_Wrapper = wrapper; }
        public InputAction @Abort => m_Wrapper.m_AbortUI_Abort;
        public InputActionMap Get() { return m_Wrapper.m_AbortUI; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(AbortUIActions set) { return set.Get(); }
        public void SetCallbacks(IAbortUIActions instance)
        {
            if (m_Wrapper.m_AbortUIActionsCallbackInterface != null)
            {
                @Abort.started -= m_Wrapper.m_AbortUIActionsCallbackInterface.OnAbort;
                @Abort.performed -= m_Wrapper.m_AbortUIActionsCallbackInterface.OnAbort;
                @Abort.canceled -= m_Wrapper.m_AbortUIActionsCallbackInterface.OnAbort;
            }
            m_Wrapper.m_AbortUIActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Abort.started += instance.OnAbort;
                @Abort.performed += instance.OnAbort;
                @Abort.canceled += instance.OnAbort;
            }
        }
    }
    public AbortUIActions @AbortUI => new AbortUIActions(this);
    public interface IAbortUIActions
    {
        void OnAbort(InputAction.CallbackContext context);
    }
}
