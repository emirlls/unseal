using System;
using Unseal.Enums;
using Volo.Abp.DependencyInjection;

namespace Unseal.Etos;

public class UserElasticEto : ITransientDependency
{
    public Guid UserId { get; set; }
    public string? UserName { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public Guid? BlockedUserId {get; set;}
    public int UserElasticQueryTpe { get; set; }
}