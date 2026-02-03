using System;
using Volo.Abp.Content;

namespace Unseal.Dtos.Capsules;

public class CapsuleBaseDto
{
    public Guid? ReceiverId { get; set; }
    public string Name { get;set; }
    public string TextContext { get;set; }
    public string GeoJson { get;set; }
    public DateTime RevealDate { get;set; }
    public IRemoteStreamContent StreamContent { get;set; }
}