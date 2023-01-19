using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeyLabeler : MonoBehaviour
{
    [SerializeField] private InputActionReference action;
    [SerializeField] private GameObject waitingForInputObject;
    [SerializeField] private int index;
    [SerializeField] private bool changeable = true;
    private TMP_Text _bindingDisplayNameText;
    private InputActionRebindingExtensions.RebindingOperation _rebindingOperation;

    private void Start()
    {
        _bindingDisplayNameText = GetComponentInChildren<TMP_Text>();
        Label();
    }

    private void OnEnable()
    {
        Label();
    }

    private static void Save()
    {
        KeyMapper.SaveRebinds();
    }

    public void StartRebinding()
    {
        if (!changeable)
        {
            return;
        }
        waitingForInputObject.SetActive(true);
        KeyMapper.playerInput.FindAction(action.name).Disable();
        _rebindingOperation = KeyMapper.playerInput.FindAction(action.name).PerformInteractiveRebinding(index)
            .WithControlsExcluding("Mouse")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(_ => RebindComplete())
            .Start();
    }

    private void RebindComplete()
    {
        _bindingDisplayNameText.text = InputControlPath.ToHumanReadableString(
            KeyMapper.playerInput.FindAction(action.name).bindings[index].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);

        _rebindingOperation.Dispose();
        Save();
        KeyMapper.playerInput.FindAction(action.name).Enable();
        waitingForInputObject.SetActive(false);
        NotifyInputManager();
    }

    public void NotifyInputManager()
    {
        var inputManager = transform.root.GetComponent<InputManager>();
        if (inputManager)
        {
            inputManager.RebindKey();
        }
    }

    public void Label()
    {
        print(InputControlPath.ToHumanReadableString(
            KeyMapper.playerInput.FindAction(action.name).bindings[index].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice));
        _bindingDisplayNameText.text = InputControlPath.ToHumanReadableString(
            KeyMapper.playerInput.FindAction(action.name).bindings[index].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);
    }
}