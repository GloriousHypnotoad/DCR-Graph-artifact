using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
public class ViewActivity : MonoBehaviour
{
    public string Id { get; private set; }
    public string Label { get; private set; }
    public string Description { get; private set; }
    public Color DisabledColor { get; private set; }
    public bool Disabled { get; private set; }
    private Color _buttonColor;
    
    private ButtonController _buttonController;
    private EffectsController _effectsController;
    private ProximityDetector _proximityDetector;
    private SceneryController _sceneryController;
    private ConstraintsController _constraintsController;
    private ActivityDetectionTrigger _activityDetectionTrigger;
    private event Action<ViewActivity> _activityMouseOver;
    private event Action<ViewActivity> _activityMouseExit;
    private event Action<ViewActivity> _onExecuted;
    private event Action<ViewActivity> _onExecuteRefused;
    private event Action<ViewActivity> _lockSelected;
    private event Action<bool> _simulatedExecution;
    private HashSet<Color> _disabledColors;
    private Color _pendingColor;
    private bool _cursorIsOneActivity;
    private bool _lockSignalRunning;
    private bool _keySignalRunning;
    private bool _isConditionOrMilestone;
    private string _sceneName;

    void Awake()
    {
        // Sub component controllers
        _buttonController = GetComponentInChildren<ButtonController>();
        _effectsController = GetComponentInChildren<EffectsController>();
        _proximityDetector = GetComponentInChildren<ProximityDetector>();
        _sceneryController = GetComponentInChildren<SceneryController>();
        _constraintsController = GetComponentInChildren<ConstraintsController>();
        _activityDetectionTrigger = GetComponentInChildren<ActivityDetectionTrigger>();

        // Subscribe to sub component events
        _buttonController.SubscribeToOnPressed(OnButtonPressed);
        _proximityDetector.SubscribeToIsTargetNearby(OnPlayerNearButton);
        _activityDetectionTrigger.SubscribeToOnMouseOver(OnActivityMouseOver);
        _activityDetectionTrigger.SubscribeToOnMouseExit(OnActivityMouseExit);
        _activityDetectionTrigger.SubscribeToOnMouseDown(OnActivityMouseDown);
        _activityDetectionTrigger.SubscribeToOnSimulatedMouseOver(OnActivitySimulatedMouseOver);
        _activityDetectionTrigger.SubscribeToOnSimulatedMouseDown(OnActivitySimulatedMouseDown);
        _activityDetectionTrigger.SubscribeToOnSimulatedMouseExit(OnActivitySimulatedMouseExit);
        _constraintsController.SubscribeToOnLockMouseDown(OnLockSelected);

        _pendingColor = Color.yellow;
        
        _lockSignalRunning = false;
        _keySignalRunning = false;

        _cursorIsOneActivity = false;
    }

    void Start()
    {
        _sceneName = GameSettings.SceneName;
        _effectsController.ToggleGlitterKey(false);
        _effectsController.ToggleGlitterLock(false);
    }

    private void OnActivitySimulatedMouseOver()
    {
        if (!_cursorIsOneActivity)
        {
            _cursorIsOneActivity = true;
            _simulatedExecution?.Invoke(true);
            OnActivityMouseOver();
            OnActivityMouseDown();
        }
    }

    private void OnActivitySimulatedMouseDown()
    {
    }

    private void OnActivitySimulatedMouseExit()
    {
        if (_cursorIsOneActivity)
        {
            _cursorIsOneActivity = false;
            _simulatedExecution?.Invoke(false);

        }
    }

    // Initialize object with activity data from graph
    internal void Initialize(string id, string label, string description, Color disabledColor)
    {
        Id = id;
        Label = label;
        Description = description;
        DisabledColor = disabledColor;
        _buttonColor = Color.white;
        _isConditionOrMilestone = disabledColor != Color.white;
        _constraintsController.ToggleKey(_isConditionOrMilestone);
        if(_isConditionOrMilestone)
        {
            _constraintsController.SetKeyColor(DisabledColor);
        }
    }

    // Public methods to set the visual state of the Activity
    internal void SetExecuted(bool isExecuted){

        // Toggle Glitter
        if(isExecuted)
        {
            _buttonColor = Color.green;
            _buttonController.SetPushButtonColor(_buttonColor);
            _effectsController.ChangeGlitterColor(Color.white);
            _effectsController.ToggleGlitterKey(false);
            _constraintsController.ToggleKey(false);
        }
        else
        {
            _buttonColor = Color.white;
            _buttonController.SetPushButtonColor(_buttonColor);
        }

        _effectsController.ToggleGlitter(!isExecuted);
        
        // Toggle animated elements on/off
        _sceneryController.ToggleAnimatedElements(isExecuted);
    }

    internal void SetPending(bool isPending)
    {
        _effectsController.ToggleSceneryLight(isPending);
        _effectsController.ToggleGodray(isPending);
        
        if(isPending){
        _effectsController.ToggleGlitter(true);
        _effectsController.ChangeSceneryLightColor(DisabledColor);
        _effectsController.TogglePulseOnSceneryLight(true);

        }   
    }

    internal void SetDisabled(HashSet<Color> colors)
    {
        _disabledColors = colors;

        bool isDisabled = false;
        if (_disabledColors.Count != 0){
            _constraintsController.ToggleLock(true);
            isDisabled = true;
            if (_lockSignalRunning)
            {
                LockSignal();
            }
            if (_disabledColors.Count == 1)
            {
                _constraintsController.StopLockColorCycle();
                _constraintsController.SetLockColor(_disabledColors.First());

            }
            else
            {
                _constraintsController.StartLockColorCycle(_disabledColors);
            }
        }
        else
        {
            _constraintsController.ToggleLock(false);
            _effectsController.ToggleGlitterLock(false);
            _lockSignalRunning = false;
        }

        // Toggle effects
        switch(_sceneName)
        {
            case "Rpg":
            case "Rpg_Abstract":
            case "Rpg_English":
            case "Rpg_English_Abstract":
                _effectsController.ToggleFog(isDisabled);
                _effectsController.ToggleDoorAndWalls(false);
                break;
            case "Office":
            case "Office_Abstract":
            case "Office_English":
            case "Office_English_Abstract":
                _effectsController.ToggleDoorAndWalls(isDisabled);
                _effectsController.ToggleFog(false);
                break;
            default:
                _effectsController.ToggleFog(isDisabled);
                _effectsController.ToggleDoorAndWalls(false);
                break;
        }
        _buttonController.ToggleRotation(!isDisabled);

        // Set class variable
        Disabled = isDisabled;
    }

    internal void SetIncluded(bool isIncluded)
    {
        // Set opacity to reflect inclusion.
        _buttonController.SetOpaque(isIncluded);
        _sceneryController.SetOpaque(isIncluded);

        if(!isIncluded)
        {
            // Disable activity if excluded.
            Disabled = true;

            _buttonController.ToggleRotation(false);
            _constraintsController.ToggleKey(false);
            if (_keySignalRunning)
            {
                _keySignalRunning = false;
                _effectsController.ToggleGlitterKey(false);
            }
        } else
        {
            _constraintsController.ToggleKey(_isConditionOrMilestone);
        }
    }
    // Allow subscribtion to Activity mouse events
    internal void SubscribeToActivityMouseOver(Action<ViewActivity> subscriber)
    {
        _activityMouseOver += subscriber;
    }
    internal void SubscribeToActivityMouseExit(Action<ViewActivity> subscriber)
    {
        _activityMouseExit += subscriber;
    }
    internal void SubscribeToLockSelected(Action<ViewActivity> subscriber)
    {
        _lockSelected += subscriber;
    }
    internal void SubscribeToSimulatedExecution(Action<bool> subscriber)
    {
        _simulatedExecution += subscriber;
    }
    

    // Forwards configuration data to Proximity Detector.
    internal void SetProximityDetectorTarget(int targetLayer)
    {
        _proximityDetector.SetTargetLayer(targetLayer);
    }

    // Allows View to subscribe to activity execution event
    internal void SubscribeToOnExecuted(Action<ViewActivity> subscriber){
        _onExecuted+=subscriber;
    }
    internal void SubscribeToOnExecuteRefused(Action<ViewActivity> subscriber){
        _onExecuteRefused+=subscriber;
    }

    // Forward MouseOver event to View
    internal void OnActivityMouseOver()
    { 
        _activityMouseOver?.Invoke(this);
    }

    // Forward MouseOver event to View
    internal void OnActivityMouseExit()
    {
        _activityMouseExit?.Invoke(this);
    }

    // Forward control to ButtonController On Mouse Down.
    internal void OnActivityMouseDown()
    {
        if(Disabled)
        {
            _buttonController.PressButtonRefuse();
            _onExecuteRefused?.Invoke(this);
        }
        else
        {
            _buttonController.PressButton();
        }
    }

    internal void OnLockSelected()
    {
        _lockSelected?.Invoke(this);
    }

    // Inform Button Controller on event from Proximity Detector
    internal void OnPlayerNearButton(bool playerNearButton)
    {
        if(!Disabled){
            _buttonController.TogglePushButtonAnimation(playerNearButton);

        }
    }
    private void OnButtonPressed(float quickRotationDuration)
    {
        _onExecuted?.Invoke(this);
        _effectsController.GlitterBurst(quickRotationDuration);
    }

    internal void SetStateAdded()
    {
    }

    internal void SetStateRemoved()
    {
    }
    internal void KeySignal()
    {
        _keySignalRunning = true;
        _effectsController.ToggleGlitterKey(true);
        _effectsController.ChangeGlitterKeyColor(DisabledColor);
    }

    internal void LockSignal()
    {
        _lockSignalRunning = true;
        _effectsController.ToggleGlitterLock(true);
        if(_disabledColors.Count > 1)
        {
            _effectsController.StartGlitterLockColorCycle(_disabledColors);
        }
        else
        {
            _effectsController.StopGlitterLockColorCycle();
            _effectsController.ChangeGlitterLockColor(_disabledColors.First());

        }
    }
    internal void DisableKeySignal()
    {
        _effectsController.ToggleGlitterKey(false);
    }

    internal void DisableLockSignal()
    {
        _lockSignalRunning = false;
         _effectsController.ToggleGlitterLock(false);
    }
}