using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Notifications
{
    public abstract record NotificationEvent(string Type);

    public sealed record NewComment : NotificationEvent
    {
        public string OpporId { get; init; }
        public long CreatedBy { get; init; }
        public string CreatedByName { get; init; }
        public long BusinessId { get; init; }
        public string? CommentPreview { get; init; }
        public string CommentId { get; init; }

        public long VisibilityId { get; init; }             
        public List<long>? AudienceAreaIds { get; init; }   
        public NewComment(string opporId,long createdBy, string? createdByName, long businessId, string? commentPreview, string commentId)
            : base("NEW_COMMENT")
        {
            OpporId = opporId;
            CreatedBy = createdBy;
            BusinessId = businessId;
            CommentPreview = commentPreview;
            CommentId = commentId;

        }
    }

    public sealed record OpportunityStateChanged : NotificationEvent
    {
        public string OpporId { get; init; }
        public string NewState { get; init; }
        public long ChangedBy { get; init; }
        public long BusinessId { get; init; }

        public OpportunityStateChanged(string opporId, string newState, long changedBy, long businessId)
            : base("OPPOR_STATE_CHANGED")
        {
            OpporId = opporId;
            NewState = newState;
            ChangedBy = changedBy;
            BusinessId = businessId;
        }
    }

}