using System;

public abstract class Worker : ICloneable
{
    public Guid Guid { get; protected set; }
    public abstract Enum Type { get; protected set; }

    public abstract object Clone();
}
