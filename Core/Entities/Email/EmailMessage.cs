using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Email
{
    public sealed record EmailAttachment(
    string Name,
    string Path,
    string MimeType
);

    public sealed class EmailMessage
    {
        public List<string> To { get; init; } = new();
        public List<string> Cc { get; init; } = new();
        public List<string> Bcc { get; init; } = new();
        public string Subject { get; init; } = "";
        public string HtmlBody { get; init; } = "";
        public string? TextBody { get; init; }

        public string? ReplyTo { get; init; }
        public List<EmailAttachment> Attachments { get; init; } = new();

        // Trazabilidad
        public string? EventCode { get; init; }
        public long? OutboxId { get; init; }
        public string? CorrelationId { get; init; }
    }
    public abstract class BaseEmailModel
    {
        public string ReceiverName { get; init; } = "";
        public string OpporName { get; init; } = "";
        public string OpporNumber { get; init; } = "";
        public DateTime SentAt { get; init; } = DateTime.UtcNow;
    }

    public class ClientReassigned : BaseEmailModel
    {
        public string ClientName { get; set; } = "";
        public string AssignerName { get; set; } = "";
    }

    public class OpporCreated
    {
        public string OpporNumber { get; set; } = "";
        public string OpporName { get; set; } = "";
        public string ClientsName { get; set; } = "";
        public string ContactName { get; set; } = "";
        public string SellerName { get; set; } = "";
        public string SectorName { get; set; } = "";
        public int Viability { get; set; }
        public DateTime DateFinish { get; set; }
        public string StatusGeneral { get; set; } = "";
        public string StatusOppor { get; set; } = "";
        public string StatusOpporSales { get; set; } = "";
        public string ReceiverName { get; set; } = "";
    }



    public class OpporStateChanged
    {
        public string OpporNumber { get; set; } = "";
        public string OpporName { get; set; } = "";
        public string ClientsName { get; set; } = "";
        public string SellerName { get; set; } = "";
        public string StatusGeneral { get; set; } = "";
        public string StatusOppor { get; set; } = "";
        public string ReceiverName { get; set; } = "";

        public string ReasonObsClients { get; set; } = "";
        public string LicStatus { get; set; } = "";
        public string NegotationOutcome { get; set; } = "";
        public string StateCommercial { get; set; } = "";
        public string StatesPresales { get; set; } = "";
    }

    public class OpporStateChangedDeliverables
    {
        public string OpporNumber { get; set; } = "";
        public string OpporName { get; set; } = "";
        public string ClientsName { get; set; } = "";
        public string SellerName { get; set; } = "";
        public string StatusGeneral { get; set; } = "";
        public string StatusOppor { get; set; } = "";
        public string StatusOpporSales { get; set; } = "";
        public string ReceiverName { get; set; } = "";
        public string ProcessType { get; set; } = "";
        public List<DeliverablesOpporSales>? DeliverablesSales { get; set; }
    }
    public class DeliverablesOpporSales
    {
        public string? DeliverablesName { get; set; }
        public string? Comment { get; set; }
        public DateTime? DueDate { get; set; }
        public string? State { get; set; }


    }

    public class OpporStateChangedProposal
    {
        public string OpporNumber { get; set; } = "";
        public string OpporName { get; set; } = "";
        public string ClientsName { get; set; } = "";
        public string SellerName { get; set; } = "";
        public string StatusGeneral { get; set; } = "";
        public string StatusOppor { get; set; } = "";
        public string ProposalComment { get; set; } = "";
    }


    public class OpporWon
    {
        public string OpporNumber { get; set; } = "";
        public string OpporName { get; set; } = "";
        public string ClientsName { get; set; } = "";
        public string SellerName { get; set; } = "";
        public string StatusGeneral { get; set; } = "";
        public string StatusOppor { get; set; } = "";

        public string CurrencySymbol { get; set; } = "";
        public string CurrencyName { get; set; } = "";
        public decimal ExchangeRate { get; set; }
        public decimal Price { get; set; }
        public decimal Cost { get; set; }
        public decimal Utility { get; set; }

    }

    public class OpporStateRechazed : BaseEmailModel
    {

        public string ClientsName { get; set; } = "";
        public string SellerName { get; set; } = "";
        public string StatusGeneral { get; set; } = "";
        public string StatusOppor { get; set; } = "";

        public string Rejection { get; set; }
        public string RejectionReason { get; set; }

    }
    public class DecisionViability
    {
        public string OpporNumber { get; set; } = "";
        public string OpporName { get; set; } = "";
        public string ReceiverName { get; set; } = "";
        public string ClientsName { get; set; }
        public string StatusGeneral { get; set; }
        public string StatusOppor { get; set; } = "";

    }

    public class DecisionViabilityRechazed
    {
        public string OpporNumber { get; set; } = "";
        public string OpporName { get; set; } = "";
        public string ReceiverName { get; set; } = "";
        public string ClientsName { get; set; }
        public string StatusGeneral { get; set; }
        public string RejectionReason { get; set; }
        public string StatusOppor { get; set; } = "";

    }


    public class ProjectResponsibleAssigned : BaseEmailModel
    {
        public string ClientsName { get; set; } = "";
        public string AssignerName { get; set; } = "";
        public List<DeliverableItemDto> DeliverablesSales { get; set; } = new();
    }


    public class DeliverableItemDto
    {
        public string DeliverablesName { get; set; }
        public DateTime DueDate { get; set; }
        public string Comment { get; set; }
        public string State { get; set; }
    }

    public class ProjectTeamAddDto : BaseEmailModel
    {

        public string ResponsibleName { get; set; }
        public string Message { get; set; }
    }

    public class UrgentObservationDto : BaseEmailModel
    {
        public string ObservationSeverity { get; set; }
        public string ObservationReason { get; set; }
    }
    public class HiringCompletedDto
    {
        public string WorkerName { get; set; }
        public string ProjectName { get; set; }
        public string ClientName { get; set; }
        public string NewStatus { get; set; }
        public string ProjectCode { get; set; }
        public string MessageDetail { get; set; }
    }


    public class QuotationInvalidated : BaseEmailModel
    {
        public string WorkerName { get; set; }
        public string Message { get; set; }
    }



    public class ProjectDelivered
    {
        public string ProjectNumber { get; set; }
        public string ProjectName { get; set; }
        public string ClientsName { get; set; }
        public string WorkerName { get; set; }
        public string ComercialName { get; set; }
        public string PreventaName { get; set; }
        public string Status { get; set; }

    }


    public class ProjectDeliveredCopy
    {
        public string ProjectNumber { get; set; }
        public string ClientsName { get; set; }
        public string ProjectName { get; set; }
        public string WorkerName { get; set; }
        public string Status { get; set; }

    }

    public class TaskAssigned
    {
        public string TaskTitle { get; set; }
        public string TaskDescription { get; set; }
        public DateTime DueDate { get; set; }
        public string OpporNum { get; set; }
        public string OpporDesc { get; set; }
        public string AssignerName { get; set; }
        public string ReceiverName { get; set; }
    }

    public class SupportExpirationAlert
    {
        public long SupportId { get; set; }        // Mapea a "supportId"
        public string Provider { get; set; }       // Mapea a "provider"
        public string Service { get; set; }        // Mapea a "service"
        public string ExpirationDate { get; set; } // Mapea a "expirationDate"
        public string AlertCode { get; set; }      // Mapea a "alertCode"
    }
}
