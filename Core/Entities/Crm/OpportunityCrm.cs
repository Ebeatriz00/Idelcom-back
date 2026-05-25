using Core.Attributes;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Crm
{
    public class OpportunityCrm : BaseAuditableEntity
    {
        [AuditField("Id")]
        public long OpporId { get; set; }

        [AuditField("Año")]
        public int Exercise { get; set; }

        [AuditField("Empresa")]
        public long BusinessId { get; set; }

        [AuditField("Oportunidad")]
        public string? OpporDesc { get; set; }

        [AuditField("N° Oportunidad")]
        public string? OpporNum { get; set; }

        [AuditField("Cliente")]
        public long ClientsId { get; set; }

        [AuditField("Contacto")]
        public long ContactsId { get; set; }

        [AuditField("Línea de negocio")]
        public long BusinessLineId { get; set; }

        [AuditField("Comercial")]
        public long CommercialId { get; set; }

        [AuditField("Ing Preventa")]
        public long EngPreSlesId { get; set; }

        [AuditField("Motivo de rechazo")]
        public long? RejectionReasonId { get; set; }

        [AuditField("Razón de rechazo")]
        public string? RejectionReasonDesc { get; set; }

        [AuditField("Moneda")]
        public long CurrencyId { get; set; }

        [AuditField("Fecha de registro")]
        public DateTime RegDate { get; set; }

        [AuditField("Fin de fecha")]
        public DateTime? FinishDate { get; set; }

        [AuditField("Fecha de consulta")]
        public DateTime? ConsultDate { get; set; }

        [AuditField("Fecha de cotizacion")]
        public DateTime? QuoteDate { get; set; }

        [AuditField("Monto de oportunidad")]
        public decimal? OpporAmount { get; set; }

        [AuditField("Porcentate de proceso")]
        public int PorcentProgress { get; set; }

        [AuditField("Estado de Oportunidad")]
        public long StateOpporId { get; set; }

        [AuditField("Estado de preventa")]
        public long StatePreSalesId { get; set; }

        [AuditField("Estado general de Oportunidad")]
        public long StateOpporGenId { get; set; }

        [AuditField("Es una pre Oportunidad")]
        public bool IsPreOppor { get; set; }

        [AuditField("Esta Aprobada")] 
        public bool IsApproved { get; set; }

        [AuditField("Raz de rechazo")]
        public string? RejectionReason { get; set; }

        [AuditField("Decidido por")]
        public long? DecisionBy { get; set; }

        [AuditField("Comentario de propuesta")]
        public string? ProposalComment { get; set; }

        [AuditField("Comentario de ganado")]
        public string? WonComment { get; set; }

        [AuditField("Ultimo cambio de estado")]
        public DateTime? LastStateChangeAt { get; set; }

        [AuditField("Recordatorio activo")]
        public bool? FollowupEnabled { get; set; }

        [AuditField("Recordar cada dias")]
        public int? FollowupEveryDays { get; set; }

        [AuditField("Siguiente recordatorio")]
        public DateTime? FollowupNextAt { get; set; }

        [AuditField("Propuesta presentada")]
        public bool? ProposalPresentation { get; set; }

        [AuditField("Resultado de negociacion")]
        public long? NegotiationOutcomeId { get; set; }

        [AuditField("Tipo de observacion de cliente")]
        public int? TypeObsClients { get; set; }

        [AuditField("Tipo de observacion economica")]
        public int? TypeObsEconomic { get; set; }

        [AuditField("Razon de negociacion")]
        public string? NegotationReason { get; set; }

        [AuditField("Recordatorio suspendido")]
        public bool? FollowupSuspended { get; set; }

        [AuditField("Etapa de negociacion")]
        public long? NegStagesId { get; set; }

        [AuditField("Es contratacion")]
        public bool? IsHiring { get; set; }

        [AuditField("Es reevaluacion")]
        public bool? IsReEvaluation { get; set; }

        [AuditField("Categoria de proyecto")]
        public int? ProjectCategory { get; set; }

        [AuditField("Condicion de pago")]
        public long? PmConditionId { get; set; }

        [AuditField("Tipo de atencion")]
        public long? FlowTypeId { get; set; }

        [AuditField("Fecha de convocatoria")]
        public DateTime? CallDate { get; set; }

        [AuditField("Tipo de oportunidad")]
        public int? TypeOppor { get; set; }

        [AuditField("Oportunidad padre")]
        public long? ParentOpportunityId { get; set; }

        [AuditField("Secuencia adicional")]
        public int? AdditionalSequence { get; set; }

    }
}
