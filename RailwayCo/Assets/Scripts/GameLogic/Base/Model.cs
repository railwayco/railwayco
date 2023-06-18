using System;

public abstract class Model
{
    public Guid Guid { get; protected set; }
    public abstract Enum Type { get; protected set; }
}
