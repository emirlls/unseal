using System;
using Volo.Abp.Domain.Entities.Events.Distributed;

namespace Unseal.Etos;

public class PasswordResetEto : EtoBase
{
    public Guid UserId { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public string ConfirmationToken { get; set; }
}