using System;
using System.Collections.Generic;

namespace FluxPay.Models;
public class TransferWebHook
{
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public string DisplayName { get; set; }
    public int TemplateId { get; set; }
    public List<WebhookEvent> Events { get; set; }
    public WebhookConfig Config { get; set; }
}
public class WebhookConfig
{
    public string PayloadURL { get; set; }
    public string ContentType { get; set; }
}

public class WebhookEvent
{
    public string ActionName { get; set; }
    public string EntityName { get; set; }
}
