namespace mem0.Core.Model;

public class History
{
    public Guid Id { get; set; }

    public string MemoryId { get; set; }

    public string PrevValue { get; set; }

    public string NewValue { get; set; }

    public string Event { get; set; }

    public DateTime DateTime { get; set; }

    public bool IsDeleted { get; set; }
}