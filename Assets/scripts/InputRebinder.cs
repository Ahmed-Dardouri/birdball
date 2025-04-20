using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

using TMPro;

public class InputRebinder : MonoBehaviour
{
    public InputActionReference actionReference; // assign in Inspector
    public int bindingIndex = 0; // Which binding (e.g. 0 = keyboard, 1 = gamepad)
    public TextMeshProUGUI bindingText; // Text component to show current binding

    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    void Start()
    {
        LoadRebinds();
        UpdateBindingDisplay();
    }

    public void StartRebind()
    {
        bindingText.text = "Press any key...";

        actionReference.action.Disable();

        rebindingOperation = actionReference.action.PerformInteractiveRebinding(bindingIndex)
            .WithCancelingThrough("<Keyboard>/escape")
            .OnComplete(operation => {
                actionReference.action.Enable();
                rebindingOperation.Dispose();
                UpdateBindingDisplay();
                SaveRebinds();
                LoadRebinds();
            })
            .Start();
    }

    void UpdateBindingDisplay()
    {
        var binding = actionReference.action.bindings[bindingIndex];
        bindingText.text = InputControlPath.ToHumanReadableString(binding.effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
    }

    void SaveRebinds()
    {
        PlayerPrefs.SetString("rebinds", actionReference.action.actionMap.asset.SaveBindingOverridesAsJson());
    }

    public void LoadRebinds()
    {
        if (PlayerPrefs.HasKey("rebinds"))
        {
            actionReference.action.actionMap.asset.LoadBindingOverridesFromJson(PlayerPrefs.GetString("rebinds"));
            UpdateBindingDisplay();
        }
    }
}
