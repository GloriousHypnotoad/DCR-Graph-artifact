using System.Collections.Generic;

public class ModelState
{
    public HashSet<string> Executed { get; set; }
    public HashSet<string> Included { get; set; }
    public HashSet<string> Pending { get; set; }

    public ModelState(HashSet<string> executed, HashSet<string> included, HashSet<string> pending)
    {
        Executed = new HashSet<string>(executed);
        Included = new HashSet<string>(included);
        Pending = new HashSet<string>(pending);
    }
    public HashSet<string> GetExecuted()
    {
        return Executed;
    }
    public HashSet<string> GetIncluded()
    {
        return Included;
    }
    public HashSet<string> GetPending()
    {
        return Pending;
    }
}