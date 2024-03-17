
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using UnityEngine;
public class Model : MonoBehaviour
{
        
    // Activity Constants
    public HashSet<string> ActivityIds { get; private set; } = new HashSet<string>();
    public Dictionary<string, Vector2> ActivityLocations { get; private set; } = new Dictionary<string, Vector2>();
    public Dictionary<string, string> ActivityDescriptions { get; private set; } = new Dictionary<string, string>();
    public Dictionary<string, string> ActivityPurposes { get; private set; } = new Dictionary<string, string>();
    public Dictionary<string, string> ActivityLabels { get; private set; } = new Dictionary<string, string>();

    // Activity Variables
    public HashSet<string> Executed { get; private set; } = new HashSet<string>();
    public HashSet<string> Included { get; private set; } = new HashSet<string>();
    public HashSet<string> Pending { get; private set; } = new HashSet<string>();

    // Constraints
    public Dictionary<string, HashSet<string>> Conditions { get; private set; } = new Dictionary<string, HashSet<string>>();
    public Dictionary<string, HashSet<string>> Responses { get; private set; } = new Dictionary<string, HashSet<string>>();
    public Dictionary<string, HashSet<string>> Excludes { get; private set; } = new Dictionary<string, HashSet<string>>();
    public Dictionary<string, HashSet<string>> Includes { get; private set; } = new Dictionary<string, HashSet<string>>();
    public Dictionary<string, HashSet<string>> Milestones { get; private set; } = new Dictionary<string, HashSet<string>>();

    private List<ModelState> _history = new List<ModelState>();
    
    public string ParseXmlFile(string fileName)
    {
        // Read the XML file into a string
        TextAsset xmlFile = Resources.Load<TextAsset>($"GameData/{fileName}");
        Debug.Log(xmlFile.text);

        // Load the XML content into an XmlDocument
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xmlFile.text);

        // Convert the XML to JSON
        string jsonText = JsonConvert.SerializeXmlNode(doc);

        #if UNITY_EDITOR
        // Save to the Assets/Resources folder (for development use)
        string resourcesFolderPath = Path.Combine(Application.dataPath, "GameData");
        if (!Directory.Exists(resourcesFolderPath))
        {
            Directory.CreateDirectory(resourcesFolderPath);
        }
        string resourcesPath = Path.Combine(resourcesFolderPath, $"{fileName}.json");
        File.WriteAllText(resourcesPath, jsonText);
        #endif

        return jsonText;
    }
    
    public void ProcessJsonFile(string jsonText)
    {
        JObject jsonObject = new JObject();
        JToken resources = new JObject();
        JToken constraints = new JObject();
        JToken marking = new JObject();

        try
        {
            // Create JSON objects
            jsonObject = JObject.Parse(jsonText);
        }
        catch (Exception e)
        {
            Debug.Log($"Error: {e.Message}");
        }
        try
        {
            resources = jsonObject["dcrgraph"]["specification"]["resources"];
        }
        catch (Exception e)
        {
            Debug.Log($"Error: {e.Message}");
        }
        try
        {
            constraints = jsonObject["dcrgraph"]["specification"]["constraints"];
        }
        catch (Exception e)
        {
            
            Debug.Log($"Error: {e.Message}");
        }
        try
        {
            marking = jsonObject["dcrgraph"]["runtime"]["marking"];
        }
        catch (Exception e)
        {
            
            Debug.Log($"Error: {e.Message}");
        }

        // Process events
        try
        {
            JToken events = resources["events"]["event"];
            foreach (var evt in events)
            {
                string id = evt["@id"].ToString();
                int x = (int) evt["custom"]["visualization"]["location"]["@xLoc"];
                int y = (int) evt["custom"]["visualization"]["location"]["@yLoc"];
                string eventDescription = (string) evt["custom"]["eventDescription"];
                string purpose = (string) evt["custom"]["purpose"];

                ActivityIds.Add(id);
                
                if (!ActivityLocations.TryGetValue(id, out _)){
                    ActivityLocations.Add(id, new Vector2(x, y));
                };
                
                if (!ActivityDescriptions.TryGetValue(id, out _)){
                    if(eventDescription != null)
                    {
                        if (eventDescription.Length > 0)
                        {
                            ActivityDescriptions.Add(id, RemoveHtmlTags(eventDescription));
                        }
                        else
                        {
                            ActivityDescriptions.Add(id, "");
                        }
                    }
                    else
                    {
                        ActivityDescriptions.Add(id, "");
                    }
                };
                
                if (!ActivityPurposes.TryGetValue(id, out _)){
                    ActivityPurposes.Add(id, purpose);
                };
            }
        }
        catch (Exception e)
        {
            Debug.Log($"Error: {e.Message}");
        }
    
        // Get label Mappings
        try
        {
            JToken labelMappings = resources["labelMappings"]["labelMapping"];

            foreach (var labelMapping in labelMappings)
            {
                string eventId = labelMapping["@eventId"].ToString();
                string label = labelMapping["@labelId"].ToString();

                if (!ActivityLabels.TryGetValue(eventId, out _)){
                    ActivityLabels.Add(eventId, label);
                }
            }
        }
        catch (Exception e)
        {
            
            Debug.Log($"Error: {e.Message}");
        }
        // Map conditions
        try
        {
            JToken conditions = constraints["conditions"]["condition"];
            CreateConstraints(Conditions, conditions);

        }
        catch (Exception e)
        {
            Debug.Log($"Error: {e.Message}");
        }

        // Create responses
        try
        {   
            JToken responses = constraints["responses"]["response"];
            CreateConstraints(Responses, responses);

        }
        catch (Exception e)
        {
            Debug.Log($"Error: {e.Message}");
        }

        // Create excludes
        try
        {
            JToken excludes = constraints["excludes"]["exclude"];
            CreateConstraints(Excludes, excludes);
        }
        catch (Exception e)
        {
            Debug.Log($"Error: {e.Message}");
        }

        // Create includes
        try
        {
            JToken includes = constraints["includes"]["include"];
            CreateConstraints(Includes, includes);
        }
        catch (Exception e)
        {
            Debug.Log($"Error: {e.Message}");
        }

        // Create milestones
        try
        {
            JToken milestones = constraints["milestones"]["milestone"];
            CreateConstraints(Milestones, milestones);
        }
        catch (Exception e)
        {
            Debug.Log($"Error: {e.Message}");
        }

        // Create initial executed markings
        try
        {
            JToken executedEvents = marking["executed"]["event"];
            CreateMarkings(Executed, executedEvents);
        }
        catch (Exception e)
        {
            Debug.Log($"Error: {e.Message}");
        }

        // Create initial included markings
        try
        {
            JToken includedEvents = marking["included"]["event"];
            CreateMarkings(Included, includedEvents);
        }
        catch (Exception e)
        {
            Debug.Log($"Error: {e.Message}");
        }

        // Create initial pending markings
        try
        {
            JToken pendingResponses = marking["pendingResponses"]["event"];
            CreateMarkings(Pending, pendingResponses);
        }
        catch (Exception e)
        {
            Debug.Log($"Error: {e.Message}");
        }
        
        _history.Add(new ModelState(GetExecuted(), GetIncluded(), GetPending()));
    }
    public Dictionary<string, string> GetActivityLabels()
    {
        return CloneDictionary(ActivityLabels);
    }
    public HashSet<string> GetActivityIds()
    {
        return CloneHashSet(ActivityIds);
    }

    public Dictionary<string, Vector2> GetActivityLocations()
    {
        return CloneDictionary(ActivityLocations);
    }

    public Dictionary<string, string> GetActivityDescriptions()
    {
        return CloneDictionary(ActivityDescriptions);
    }

    public Dictionary<string, string> GetActivityPurposes()
    {
        return CloneDictionary(ActivityPurposes);
    }

    public HashSet<string> GetExecuted()
    {
        return CloneHashSet(Executed);
    }

    public HashSet<string> GetIncluded()
    {
        return CloneHashSet(Included);
    }

    public HashSet<string> GetPending()
    {
        return CloneHashSet(Pending);
    }

    public Dictionary<string, HashSet<string>> GetMilestones()
    {
        return CloneDictionary(Milestones);
    }

    public Dictionary<string, HashSet<string>> GetConditions()
    {
        return CloneDictionary(Conditions);
    }

    public Dictionary<string, HashSet<string>> GetResponses()
    {
        return CloneDictionary(Responses);
    }

    public Dictionary<string, HashSet<string>> GetExcludes()
    {
        return CloneDictionary(Excludes);
    }

    public Dictionary<string, HashSet<string>> GetIncludes()
    {
        return CloneDictionary(Includes);
    }

    // Method overload to enable history update on refused executions.
    public void ExecuteActivity()
    {
        _history.Add(new ModelState(GetExecuted(), GetIncluded(), GetPending()));
    }
    
    public void ExecuteActivity(string clickedActivityId)
    {
        // Mark Activity as Executed and remove any Pending markings
        Executed.Add(clickedActivityId);
        Pending.Remove(clickedActivityId);

        // Add Pending markings to any Activities affected by Response constraints.
        if (Responses.TryGetValue(clickedActivityId, out HashSet<string> responseActivities))
        {
            foreach (string activity in responseActivities)
            {
                Pending.Add(activity);            
            }
        }

        // Add Excluded markings to any Activities affected by Excludes constraints.
        if (Excludes.TryGetValue(clickedActivityId, out HashSet<string> excludeActivities))
        {
            foreach (string activity in excludeActivities)
            {
                Included.Remove(activity);            
            }
        }

        // Add Included markings to any Activities affected by Includes constraints.
        if (Includes.TryGetValue(clickedActivityId, out HashSet<string> includeActivities))
        {
            foreach (string activity in includeActivities)
            {
                Included.Add(activity);
            }
        }
        _history.Add(new ModelState(GetExecuted(), GetIncluded(), GetPending()));

    }

    // Get specific state in history
    public ModelState GetStateAt(int index)
    {
        return _history[index];
    }
    public int GetHistoryLength()
    {
        return _history.Count;
    }
    public void RevertHistoryBackTo(int revertPoint)
    {
        _history.RemoveRange(revertPoint + 1, _history.Count - revertPoint - 1);

        var stateAtRevertPoint = _history[revertPoint];

        Executed = new HashSet<string>(stateAtRevertPoint.Executed);
        Included = new HashSet<string>(stateAtRevertPoint.Included);
        Pending = new HashSet<string>(stateAtRevertPoint.Pending);
    }

    // Helper methods
    internal void CreateConstraints(Dictionary<string, HashSet<string>> constraintsDictionary, JToken constraints)
    {
        if (constraints != null)
        {
            if (constraints.Type == JTokenType.Array)
            {
                foreach (var pair in constraints)
                {
                    string sourceId = pair["@sourceId"].ToString();
                    string targetId = pair["@targetId"].ToString();

                    InsertValuesIntoDictionary(constraintsDictionary, sourceId, targetId);
                }
            }
            else if (constraints.Type == JTokenType.Object)
            {
                string sourceId = constraints["@sourceId"].ToString();
                string targetId = constraints["@targetId"].ToString();

                InsertValuesIntoDictionary(constraintsDictionary, sourceId, targetId);
            }
        }
    }
    internal void CreateMarkings(HashSet<string> markingsHashSet, JToken markings)
    {
        if (markings.Type == JTokenType.Array)
        {
            foreach (var marking in markings)
            {
                InsertValueIntoHashSet(markingsHashSet, marking["@id"].ToString());
            }
        }
        else if(markings.Type == JTokenType.Object)
        {
            InsertValueIntoHashSet(markingsHashSet, markings["@id"].ToString());
        }
    }
    
    internal void InsertValuesIntoDictionary(Dictionary<string, HashSet<string>> constraintsDictionary, string sourceId, string targetId)
    {
        if (!ActivityIds.TryGetValue(sourceId, out string key))
        {
            Debug.Log($"The key '{sourceId}' was not found in the Events dictionary.\n");
            return;
        }

        if (!ActivityIds.TryGetValue(targetId, out string value))
        {
            Debug.Log($"The key '{targetId}' was not found in the Events dictionary.\n");
            return;
        }

        if (constraintsDictionary.TryGetValue(key, out HashSet<string> existingValues))
        {
            if (existingValues.Contains(value))
            {
                Debug.Log("The constraint already exists.\n");
                return;
            }
            existingValues.Add(value);
        }
        else
        {
            constraintsDictionary[key] = new HashSet<string> { value };
        }
    }

    internal void InsertValueIntoHashSet(HashSet<string> markingsHashSet, string eventId)
    {
        if (!ActivityIds.TryGetValue(eventId, out string evt))
        {
            Debug.Log($"The key '{eventId}' was not found in the Events dictionary.\n");
            return;
        }

        markingsHashSet.Add(evt);
    }
    internal HashSet<T> CloneHashSet<T>(HashSet<T> originalHashSet)
    {
        return new HashSet<T>(originalHashSet);
    }

    internal Dictionary<TKey, TValue> CloneDictionary<TKey, TValue>(Dictionary<TKey, TValue> originalDictionary)
    {
        return new Dictionary<TKey, TValue>(originalDictionary);
    }

    internal string RemoveHtmlTags(string input)
    {
        // Regular expression to match any HTML tag
        var regex = new Regex("<.*?>");

        // Replace all HTML tags with an empty string
        return regex.Replace(input, "");
    }
}