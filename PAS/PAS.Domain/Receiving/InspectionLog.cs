using System;
using Domain.Common;
namespace Domain.Receiving
{
    public class InspectionLog : BaseEntity
    {
        public Guid ReceivingNoteId { get; private set; }

        public Guid InspectorId { get; private set; }

        public bool IsPassed { get; private set; }

        public string DeviationNotes { get; private set; }

        public DateTime InspectionDate { get; private set; }

        public ReceivingNote? ReceivingNote { get; private set; }

        private InspectionLog()
        {
        }

        public InspectionLog(Guid receivingId, Guid inspector, bool passed, string notes)
        {
            ReceivingNoteId = receivingId;
            InspectorId = inspector;
            IsPassed = passed;
            DeviationNotes = notes;
            InspectionDate = DateTime.UtcNow;
        }
    }
}