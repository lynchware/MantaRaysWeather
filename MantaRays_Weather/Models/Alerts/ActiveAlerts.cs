namespace MantaRays_Weather.Models.Alerts
{
    public class ActiveAlerts
    {
        public List<ActiveAlertFeature> Features { get; set; } = [];
    }

    public class ActiveAlertFeature
    {
        public ActiveAlert? Properties { get; set; }
    }

    public class ActiveAlert
    {
        public string? Id { get; set; }
        public string? AreaDesc { get; set; }
        public List<string> AffectedZones { get; set; } = [];
        public DateTimeOffset? Effective { get; set; }
        public DateTimeOffset? Onset { get; set; }
        public DateTimeOffset? Expires { get; set; }
        public DateTimeOffset? Ends { get; set; }
        public string? Status { get; set; }
        public string? MessageType { get; set; }
        public string? Severity { get; set; }
        public string? Certainty { get; set; }
        public string? Urgency { get; set; }
        public string? Event { get; set; }
        public string? SenderName { get; set; }
        public string? Headline { get; set; }
        public string? Description { get; set; }
        public string? Instruction { get; set; }
        public string? Response { get; set; }
    }
}
