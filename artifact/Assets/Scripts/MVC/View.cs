
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class View : MonoBehaviour
{
    
    private PlayerObjectController _playerObjectController;    
    private CameraController _firstPersonCamera;
    private CameraController _thirdPersonCamera;
    private CameraController _topDownCamera;
    private CameraController _birdsEyeCamera;
    private RawImage _topDownCamFeed;
    private Image _reticule;
    private event Action<string> _activityExecuted;
    private event Action<string> _activityExecuteRefused;
    private event Action<string> _activityLockSelected;
    private event Action<string> _activityLockExcluded;
    private Dictionary<string, GameObject> _activities = new Dictionary<string, GameObject>();
    private event Action<bool> _activitySimulationIsExecuting;

    void Awake()
    {
        // Controller for moving the player avatar
        _playerObjectController = GetComponentInChildren<PlayerObjectController>();
        
        // Controllers for all four cameras in the game.
        _firstPersonCamera = transform.Find("PlayerObject/FirstPersonCamera").GetComponent<CameraController>();
        _thirdPersonCamera = transform.Find("PlayerObject/ThirdPersonCamera").GetComponent<CameraController>();
        _topDownCamera = transform.Find("PlayerObject/TopDownCamera").GetComponent<CameraController>();
        _birdsEyeCamera = transform.Find("BirdsEyeCamera").GetComponent<CameraController>();

        // UI elements (Top down camera and targeting reticule for first person)
        _topDownCamFeed = transform.GetComponentInChildren<RawImage>();
        _reticule = GetComponentInChildren<Image>();
        
        // Configure Camera controllers for individual purposes
        _firstPersonCamera.SetCameraMode(CameraMode.FirstPerson);
        _thirdPersonCamera.SetCameraMode(CameraMode.ThirdPerson);
        _topDownCamera.SetCameraMode(CameraMode.TopDown);
        _birdsEyeCamera.SetCameraMode(CameraMode.BirdsEye);

        // Enable first person camera
        SetCamerasAreEnabled(new bool[]{true, false, true, false});
    }

    void Start()
    {
        // Remove cursor from initial first person mode
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Configurations for each camera mode
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            SetCamerasAreEnabled(new bool[]{true, false, true, false});
            GameSettings.ActiveCamera = CameraMode.FirstPerson;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            SetCamerasAreEnabled(new bool[]{false, true, true, false});
            GameSettings.ActiveCamera = CameraMode.ThirdPerson;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            SetCamerasAreEnabled(new bool[]{false, false, false, true});
            GameSettings.ActiveCamera = CameraMode.BirdsEye;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            // Toggle topdown feed on/off
            if (!_birdsEyeCamera.gameObject.activeSelf){
                _topDownCamFeed.gameObject.SetActive(!_topDownCamFeed.gameObject.activeSelf);
            }
        }
    }

    // Create individual activities in View
    public void CreateActivities(
        Dictionary<string, string> idsAndLabels, 
        Dictionary<string, string> descriptions, 
        Dictionary<string, Color> colors
    ){
        foreach (KeyValuePair<string, string> kvp in idsAndLabels)
        {
            // Get pre-configured activity object from game space
            GameObject activtyObject = transform.Find("ViewActivityContainer").Find(kvp.Value).gameObject;
            ViewActivity activity = activtyObject.GetComponent<ViewActivity>();

            // Pass ID and label to to game object script
            activity.Initialize(kvp.Key, kvp.Value, descriptions[kvp.Key], colors[kvp.Key]);
            _activities.Add(kvp.Key, activtyObject);

            // Subscribe to user input events handled by the Activity
            activity.SubscribeToActivityMouseOver(OnActivityMouseOver);
            activity.SubscribeToActivityMouseExit(OnActivityMouseExit);
            activity.SubscribeToOnExecuted(OnActivityExecuted);
            activity.SubscribeToOnExecuteRefused(OnActivityExecuteRefused);
            activity.SubscribeToLockSelected(OnLockSelected);
            activity.SubscribeToSimulatedExecution(OnSimulatedExecution);

            // Configure proximity detector for player object.
            activity.SetProximityDetectorTarget(transform.Find("PlayerObject/PlayerBody").gameObject.layer);
        }
    }

    private void OnSimulatedExecution(bool isExecuting)
    {
        _activitySimulationIsExecuting?.Invoke(isExecuting);
    }

    // Methods for passing state data to Activities
    public void SetActivityExecuted(string activityId, bool isExecuted)
    {
        _activities[activityId].GetComponent<ViewActivity>().SetExecuted(isExecuted);
    }
    public void SetActivityPending(string activityId, bool isPending){
        _activities[activityId].GetComponent<ViewActivity>().SetPending(isPending);
    }
    public void SetActivityDisabled(string activityId, HashSet<Color> colors){
        _activities[activityId].GetComponent<ViewActivity>().SetDisabled(colors);
    }
    public void SetActivityIncluded(string activityId, bool isIncluded){
        _activities[activityId].GetComponent<ViewActivity>().SetIncluded(isIncluded);
    }
    public void SetActivityStateAdded(string activityId)
    {
        if (_activities.TryGetValue(activityId, out var activity))
        {
            activity.GetComponent<ViewActivity>().SetStateAdded();
        }
    }
    public void SetActivityStateRemoved(string activityId)
    {
        if (_activities.TryGetValue(activityId, out var activity))
        {
            activity.GetComponent<ViewActivity>().SetStateRemoved();
        }
    }

    // Method for updating the global lighting when activities are pending or milestones are unmet
    public void UpdateGlobalEnvironment(bool hasPendingActivities)
    {
        string activeLight;
        string deactiveLight;

        if (hasPendingActivities)
        {
            activeLight = FileStrings.Moonlight;
            deactiveLight = FileStrings.Sunlight;
        }
        else
        {
            activeLight = FileStrings.Sunlight;
            deactiveLight = FileStrings.Moonlight;
        }
            
        string skybox = hasPendingActivities ? Path.Combine(FileStrings.SkyBoxesPath, FileStrings.SkyBoxDaytime) : Path.Combine(FileStrings.SkyBoxesPath, FileStrings.SkyBoxNighttime);
        string lightPath = Path.Combine(FileStrings.EnvironmentLightsPath, activeLight);
        
        RenderSettings.skybox = Resources.Load<Material>(skybox);
        RenderSettings.sun = Resources.Load<Light>(lightPath);
        RenderSettings.reflectionIntensity = 0;
        
        transform.Find(activeLight).gameObject.SetActive(true);
        transform.Find(deactiveLight).gameObject.SetActive(false);

        DynamicGI.UpdateEnvironment(); 
    }
    
    // Method for subscribing to Activity ID
    public void SubscribeToActivityExecuted(Action<string> subscriber)
    {
        _activityExecuted+=subscriber;
    }
    public void SubscribeToActivityExecuteRefused(Action<string> subscriber)
    {
        _activityExecuteRefused+=subscriber;
    }

    public void SubscribeToActivityLockSelected(Action<string> subscriber)
    {
        _activityLockSelected+=subscriber;
    }

    internal void SubscribeToActivityLockExcluded(Action<string> subscriber)
    {
        _activityLockExcluded+=subscriber;
    }

    public void SubscribeToActivitySimulationIsExecuting(Action<bool> subscriber)
    {
        _activitySimulationIsExecuting += subscriber;
    }

    // Helper method for toggling cameras on/off
    internal void SetCamerasAreEnabled(bool[] isEnabled)
    {
        _firstPersonCamera.gameObject.SetActive(isEnabled[0]);
        _thirdPersonCamera.gameObject.SetActive(isEnabled[1]);
        _topDownCamFeed.gameObject.SetActive(isEnabled[2]);
        _birdsEyeCamera.gameObject.SetActive(isEnabled[3]);
    }

    // Event handlers
    internal void OnActivityMouseOver(ViewActivity activity)
    {
        if (!activity.Disabled)
        {
            _reticule.color = Color.green;
        }
        else
        {
            _reticule.color = Color.red;
        }
        string label = activity.Label;
        string description = activity.Description;

        DisplayActivityText(label, description);
    }

    private void OnLockSelected(ViewActivity activity)
    {
        _activityLockSelected?.Invoke(activity.Id);
    }

    private void OnLockExcluded(ViewActivity activity)
    {
        _activityLockExcluded?.Invoke(activity.Id);
    }

    internal void DisplayActivityText(string label, string description)
    {
        TextMeshProUGUI labelDisplay = GameObject.Find("HUD/LabelDisplay").GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI descriptionDisplay = GameObject.Find("HUD/DescriptionDisplay").GetComponentInChildren<TextMeshProUGUI>();

        labelDisplay.text = label;
        descriptionDisplay.text = description;
    }
    internal void OnActivityMouseExit(ViewActivity activity){
        _reticule.color = Color.white;
        DisplayActivityText("", "");

    }

    internal void OnActivityExecuted(ViewActivity activity){
        _activityExecuted?.Invoke(activity.Id);
    }

    private void OnActivityExecuteRefused(ViewActivity activity)
    {
        _activityExecuteRefused?.Invoke(activity.Id);
    }

    internal void UpdateDeltaLists(List<string> addedToIncluded, List<string> removedFromIncluded, List<string> addedToPending, List<string> removedFromPending, List<string> addedToHaveUnmetMilestones, List<string> removedFromHaveUnmetMilestones, List<string> addedToDisabled, List<string> removedFromDisabled)
    {
        Debug.Log("addedToIncluded");
        foreach (var item in addedToIncluded)
        {
            Debug.Log(item);
        }
        Debug.Log("removedFromIncluded");
        foreach (var item in removedFromIncluded)
        {
            Debug.Log(item);
        }
        Debug.Log("addedToPending");
        foreach (var item in addedToPending)
        {
            Debug.Log(item);
        }
        Debug.Log("removedFromPending");
        foreach (var item in removedFromPending)
        {
            Debug.Log(item);
        }
        Debug.Log("addedToHaveUnmetMilestones");
        foreach (var item in addedToHaveUnmetMilestones)
        {
            Debug.Log(item);
        }
        Debug.Log("removedFromHaveUnmetMilestones");
        foreach (var item in removedFromHaveUnmetMilestones)
        {
            Debug.Log(item);
        }
        Debug.Log("addedToDisabled");
        foreach (var item in addedToDisabled)
        {
            Debug.Log(item);
        }
        Debug.Log("removedFromDisabled");
        foreach (var item in removedFromDisabled)
        {
            Debug.Log(item);
        }
    }

    internal void ActivityKeySignal(string activityId)
    {
        _activities[activityId].GetComponent<ViewActivity>().KeySignal();
    }

    internal void ActivityLockSignal(string activityId)
    {
         _activities[activityId].GetComponent<ViewActivity>().LockSignal();
    }

    internal void ClearKeySignal(string activityId)
    {
        _activities[activityId].GetComponent<ViewActivity>().DisableKeySignal();
    }

    internal void ClearLockSignal(string activityId)
    {
        _activities[activityId].GetComponent<ViewActivity>().DisableLockSignal();
    }
}