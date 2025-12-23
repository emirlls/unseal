using System;
using Volo.Abp.Domain.Entities;

namespace Unseal.Entities.Lookups;

public abstract class LookupBaseEntity : IEntity<Guid>
{
    public Guid Id { get; }
    public string Name { get; set; }
    public int Code { get; set; }
    
    public object[] GetKeys()
    {
        return new object[] { Id };
    }

    protected LookupBaseEntity(Guid id, string name, int code)
    {
        Id = id;
        Name = name;
        Code = code;
    }
}