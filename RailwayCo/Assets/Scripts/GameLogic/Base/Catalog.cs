using System;

public abstract class Catalog
{
    public Guid Guid { get; protected set; }
    public abstract Enum Type { get; protected set; }
}
