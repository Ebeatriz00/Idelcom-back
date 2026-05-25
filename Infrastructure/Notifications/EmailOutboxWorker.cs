using Core.Entities.Email;
using Core.Interfaces;
using Core.Interfaces.Notifications;
using Infrastructure.Hubs;
using Infrastructure.Notifications;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

public sealed class EmailOutboxWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<EmailOutboxWorker> _log;
    private readonly IHubContext<NotificationsHub> _hub;

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public EmailOutboxWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<EmailOutboxWorker> log,
        IHubContext<NotificationsHub> hub)
    {
        _scopeFactory = scopeFactory;
        _log = log;
        _hub = hub;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        bool printedResources = false;
        await Task.Yield();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
            using var scope = _scopeFactory.CreateScope();

            var repo = scope.ServiceProvider.GetRequiredService<IEmailOutboxRepository>();
            var email = scope.ServiceProvider.GetRequiredService<IEmailSender>();
            var tpl = scope.ServiceProvider.GetRequiredService<IEmailTemplateRenderer>();

            var batch = await repo.GetPendingBatchAsync(20, stoppingToken);

            //var asm = typeof(EmailTemplateRenderer).Assembly;
            //foreach (var resource in asm.GetManifestResourceNames())
            //{
            //    Console.WriteLine($"Resource: {resource}");
            //}

            foreach (var item in batch)
            {
                
                if (!await repo.MarkProcessingAsync(item.OutboxId, stoppingToken))
                    continue;

                try
                {
                    if (string.IsNullOrWhiteSpace(item.ToEmail))
                        throw new Exception("ToEmail vacío en outbox.");

                    string template;
                    object model;

                    Console.WriteLine($"Procesando item={item.EventCode}");

                    switch (item.EventCode)
                    {
                        case "CLIENT_REASSIGNED":
                            template = "ClientReassigned";
                            model = JsonSerializer.Deserialize<ClientReassigned>(item.PayloadJson, JsonOpts)
                                ?? throw new Exception($"Payload inválido para {item.EventCode}. OutboxId={item.OutboxId}");
                            break;

                        case "OPPOR_CREATED":
                            template = "OpporCreated";
                            model = JsonSerializer.Deserialize<OpporCreated>(item.PayloadJson, JsonOpts)
                                ?? throw new Exception($"Payload inválido para {item.EventCode}. OutboxId={item.OutboxId}");
                            break;

                        case "OPPOR_MGMT_CREATED":
                            template = "OpporCreatedManager";
                            model = JsonSerializer.Deserialize<OpporCreated>(item.PayloadJson, JsonOpts)
                                ?? throw new Exception($"Payload inválido para {item.EventCode}. OutboxId={item.OutboxId}");
                            break;

                        case "OPPOR_LOW_VIABILITY":
                            template = "OpporLowViability";
                            model = JsonSerializer.Deserialize<OpporCreated>(item.PayloadJson, JsonOpts)
                                ?? throw new Exception($"Payload inválido para {item.EventCode}. OutboxId={item.OutboxId}");
                            break;
                        case "OPPOR_VIABILITY_APPROVED":
                            template = "DecisionViability";
                            model = JsonSerializer.Deserialize<DecisionViability>(item.PayloadJson, JsonOpts)
                                 ?? throw new Exception($"Payload invalido para {item.EventCode}. OutboxId={item.OutboxId}");
                            break;
                        case "OPPOR_VIABILITY_REJECTED":
                            template = "DecisionViabilityRechazed";
                            model = JsonSerializer.Deserialize<DecisionViabilityRechazed>(item.PayloadJson, JsonOpts)
                                ?? throw new Exception($"Payload invalido para {item.EventCode}. OutboxId={item.OutboxId}");
                            break;
                        case "PROJECT_RESPONSIBLE_ASSIGNED":
                            template = "ProjectResponsibleAssigned";
                            model = JsonSerializer.Deserialize<ProjectResponsibleAssigned>(item.PayloadJson, JsonOpts)
                                ?? throw new Exception($"Payload invalido para {item.EventCode}. OutboxId={item.OutboxId}");
                            break;
                        case "OPPOR_STATE_CHANGED":
                            template = "OpporStateChanged";
                            model = JsonSerializer.Deserialize<OpporStateChanged>(item.PayloadJson, JsonOpts)
                                 ?? throw new Exception($"Payload invalido para {item.EventCode}. OutboxId={item.OutboxId}");
                            break;

                        case "OPPOR_DELIVERABLES_INGENIERIA_INIT":
                            template = "OpporStateChangedPresales";
                            model = JsonSerializer.Deserialize<OpporStateChangedDeliverables>(item.PayloadJson, JsonOpts)
                                 ?? throw new Exception($"Payload invalido para {item.EventCode}. OutboxId={item.OutboxId}");
                            break;

                        case "OPPOR_DELIVERABLES_INGENIERIA_NEW":
                            template = "OpporStateChangedDeliverables";
                            model = JsonSerializer.Deserialize<OpporStateChangedDeliverables>(item.PayloadJson, JsonOpts)
                                 ?? throw new Exception($"Payload invalido para {item.EventCode}. OutboxId={item.OutboxId}");
                            break;

                        case "OPPOR_PROPOSAL_REGISTERED":
                            template = "OpporStateChangedProposal";
                            model = JsonSerializer.Deserialize<OpporStateChangedProposal>(item.PayloadJson, JsonOpts)
                                 ?? throw new Exception($"Payload invalido para {item.EventCode}. OutboxId={item.OutboxId}");
                            break;

                        case "OPPOR_WON_REGISTERED":
                            template = "OpporStateChangedWon";
                            model = JsonSerializer.Deserialize<OpporWon>(item.PayloadJson, JsonOpts)
                                 ?? throw new Exception($"Payload invalido para {item.EventCode}. OutboxId={item.OutboxId}");
                            break;

                        case "OPPOR_LOST_REGISTERED":
                            template = "OpporStateChangedLost";
                            model = JsonSerializer.Deserialize<OpporStateRechazed>(item.PayloadJson, JsonOpts)
                                 ?? throw new Exception($"Payload invalido para {item.EventCode}. OutboxId={item.OutboxId}");
                            break;
                        case "OPPOR_DISCARDED_REGISTERED":
                            template = "OpporStateChangedDiscarded";
                            model = JsonSerializer.Deserialize<OpporStateRechazed>(item.PayloadJson, JsonOpts)
                                 ?? throw new Exception($"Payload invalido para {item.EventCode}. OutboxId={item.OutboxId}");
                            break;
                        case "OPPOR_STANDBY_REGISTERED":
                            template = "OpporStateChangedStandBy";
                            model = JsonSerializer.Deserialize<OpporStateRechazed>(item.PayloadJson, JsonOpts)
                                 ?? throw new Exception($"Payload invalido para {item.EventCode}. OutboxId={item.OutboxId}");
                            break;
                        case "PROJECT_MEMBER_ADDED":
                            template = "ProjectMemberAdded";
                            model = JsonSerializer.Deserialize<ProjectTeamAddDto>(item.PayloadJson, JsonOpts)
                                 ?? throw new Exception($"Payload invalido para {item.EventCode}. OutboxId={item.OutboxId}");
                            break;
                        case "URGENT_OBSERVATION":
                            template = "UrgentObservation";
                            model = JsonSerializer.Deserialize<UrgentObservationDto>(item.PayloadJson, JsonOpts)
                                    ?? throw new Exception($"Payload invalido para {item.EventCode}. OutboxId={item.OutboxId}");
                            break;
                        case "HIRING_CONSULTANCY_COMPLETED":
                            template = "HiringConsultancyCompleted";
                            model = JsonSerializer.Deserialize<HiringCompletedDto>(item.PayloadJson, JsonOpts)
                                    ?? throw new Exception($"payload invalido para{item.EventCode}. OutboxId={item.OutboxId}");
                            break;
                        case "OPPOR_CONSULTING_REQUESTED":
                            template = "OpporHiring";
                            model = JsonSerializer.Deserialize<OpporStateChangedDeliverables>(item.PayloadJson, JsonOpts)
                                    ?? throw new Exception($"payload invalido para{item.EventCode}. OutboxId={item.OutboxId}");
                            break;
                        case "OPPOR_HIRING_DEV":
                            template = "OpporHiringDev";
                            model = JsonSerializer.Deserialize<OpporStateChangedDeliverables>(item.PayloadJson, JsonOpts)
                                    ?? throw new Exception($"payload invalido para{item.EventCode}. OutboxId={item.OutboxId}");
                            break;
                        case "OPPOR_OBS_LICITATIONS":
                            template = "OpporObsLicitations";
                            model = JsonSerializer.Deserialize<OpporStateChanged>(item.PayloadJson, JsonOpts)
                                    ?? throw new Exception($"payload invalido para{item.EventCode}. OutboxId={item.OutboxId}");
                            break;
                        case "OPPOR_OBS_COMMERCIAL":
                            template = "OpporObsCommercial";
                            model = JsonSerializer.Deserialize<OpporStateChanged>(item.PayloadJson, JsonOpts)
                                    ?? throw new Exception($"payload invalido para{item.EventCode}. OutboxId={item.OutboxId}");
                            break;
                        case "OPPOR_OBS_PRESALES":
                            template = "OpporObsPresales";
                            model = JsonSerializer.Deserialize<OpporStateChanged>(item.PayloadJson, JsonOpts)
                                    ?? throw new Exception($"payload invalido para{item.EventCode}. OutboxId={item.OutboxId}");
                            break;
                        case "QUOTATION_INVALIDATED":
                            template = "QuotationInvalidated";
                            model = JsonSerializer.Deserialize<QuotationInvalidated>(item.PayloadJson, JsonOpts)
                                    ?? throw new Exception($"payload invalido para{item.EventCode}. OutboxId={item.OutboxId}");
                            break;
                        case "OPPOR_REEVALUATION_REQUIRED":
                            template = "OpporReEvaluation";
                            model = JsonSerializer.Deserialize<OpporCreated>(item.PayloadJson, JsonOpts)
                                    ?? throw new Exception($"payload invalido para{item.EventCode}. OutboxId={item.OutboxId}");
                            break;
                        case "PROJECT_DELIVERED":
                            template = "ProjectDelivered";
                            model = JsonSerializer.Deserialize<ProjectDelivered>(item.PayloadJson, JsonOpts)
                                    ?? throw new Exception($"payload invalido para{item.EventCode}. OutboxId={item.OutboxId}");
                            break;

                        case "PROJECT_DELIVERED_COPY":
                            template = "ProjectDeliveredCopy";
                            model = JsonSerializer.Deserialize<ProjectDeliveredCopy>(item.PayloadJson, JsonOpts)
                                    ?? throw new Exception($"payload invalido para{item.EventCode}. OutboxId={item.OutboxId}");
                            break;

                        case "TASK_ASSIGNED":
                            template = "TaskAssigned";
                            model = JsonSerializer.Deserialize<TaskAssigned>(item.PayloadJson, JsonOpts)
                                    ?? throw new Exception($"payload invalido para{item.EventCode}. OutboxId={item.OutboxId}");
                            break;
                        case EmailEventCodes.SsomaDocumentExpirationAlert:
                            template = "SsomaDocumentExpirationAlert";
                            var ssomaPayload = DeserializeSsomaDocumentExpirationPayload(item.PayloadJson, item.EventCode, item.OutboxId);
                            model = SsomaDocumentExpirationEmailModelFactory.Create(ssomaPayload);
                            break;
                        case "SUPPORT_EXPIRATION_ALERT":
                            template = "SupportExpirationAlert"; // Nombre de tu plantilla HTML/Razor
                            model = JsonSerializer.Deserialize<SupportExpirationAlert>(item.PayloadJson, JsonOpts)
                                    ?? throw new Exception($"Payload inválido para {item.EventCode}. OutboxId={item.OutboxId}");
                            break;
                        default:
                            throw new Exception($"Evento no soportado: {item.EventCode}");
                    }
                    var html = await tpl.RenderAsync(template, model);

                    var toList = SplitEmails(item.ToEmail);
                    var ccList = SplitEmails(item.CcEmail);

                    await email.SendAsync(new EmailMessage
                    {
                        To = toList,
                        Cc = ccList.Count > 0 ? ccList : null,
                        Subject = item.Subject,
                        HtmlBody = html
                    }, stoppingToken);

                    await repo.MarkSentAsync(item.OutboxId, stoppingToken);

                    if (item.TargetUserId != null)
                    {
                        await _hub.Clients.User(item.TargetUserId.ToString()!)
                            .SendAsync("notify", new
                            {
                                code = "EMAIL_SENT",
                                message = "Correo enviado",
                                outboxId = item.OutboxId,
                                targetUserId = item.TargetUserId,
                                createdAt = DateTime.UtcNow
                            }, stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _log.LogError(ex,
                        "Error enviando correo OUTBOX_ID={OutboxId} EVENT={EventCode} TO={ToEmail}",
                        item.OutboxId, item.EventCode, item.ToEmail);

                    int next = item.Retries switch
                    {
                        0 => 1,
                        1 => 3,
                        2 => 5,
                        3 => 10,
                        _ => 30
                    };

                    await repo.MarkFailedAsync(item.OutboxId, ex.Message, next, stoppingToken);
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error crítico en EmailOutboxWorker. Reintentando en 30s.");
                await DelayOrStopAsync(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }

    private static async Task DelayOrStopAsync(TimeSpan delay, CancellationToken ct)
    {
        try
        {
            await Task.Delay(delay, ct);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
        }
    }

    private static List<string> SplitEmails(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return new List<string>();

        return raw
            .Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(e => e.Trim())
            .Where(e => !string.IsNullOrWhiteSpace(e))
            .ToList();
    }

    private static List<SsomaDocumentExpirationPayloadItem> DeserializeSsomaDocumentExpirationPayload(
        string payloadJson,
        string eventCode,
        long outboxId)
    {
        if (string.IsNullOrWhiteSpace(payloadJson))
            throw new Exception($"Payload vacío para {eventCode}. OutboxId={outboxId}");

        using var doc = JsonDocument.Parse(payloadJson);

        return doc.RootElement.ValueKind switch
        {
            JsonValueKind.Array => JsonSerializer.Deserialize<List<SsomaDocumentExpirationPayloadItem>>(payloadJson, JsonOpts)
                ?? throw new Exception($"Payload inválido para {eventCode}. OutboxId={outboxId}"),
            JsonValueKind.Object => new List<SsomaDocumentExpirationPayloadItem>
            {
                JsonSerializer.Deserialize<SsomaDocumentExpirationPayloadItem>(payloadJson, JsonOpts)
                    ?? throw new Exception($"Payload inválido para {eventCode}. OutboxId={outboxId}")
            },
            _ => throw new Exception($"Payload inválido para {eventCode}. Se esperaba objeto o array. OutboxId={outboxId}")
        };
    }
}


