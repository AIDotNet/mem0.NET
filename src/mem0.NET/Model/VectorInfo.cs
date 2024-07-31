namespace mem0.Core.Model;

public class VectorInfo
{
    public Dictionary<string,object> PayloadSchema { get; set; }

    public ulong PointsCount { get; set; }

    public ulong SegmentsCount { get; set; }

    public bool HasPointsCount { get; set; }

    public bool HasVectorsCount { get; set; }

    public VectorInfoStatus Status { get; set; }

    public OptimizerStatus OptimizerStatus { get; set; }
}

public class OptimizerStatus
{
    public bool Ok { get; set; }

    public string Error { get; set; }
}