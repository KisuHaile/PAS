using System;
using Domain.Requisition;

namespace Domain.Common
{
    public class DocumentAttachment : BaseEntity
    {
        public string FileName { get; private set; }

        public string FilePath { get; private set; }

        public string ContentType { get; private set; }

        public Guid RelatedEntityId { get; private set; }

        public string RelatedEntityName { get; private set; }

        public Guid? ServiceRequestId { get; private set; }

        public ServiceRequest ServiceRequest { get; private set; }

        public DocumentAttachment(string fileName, string filePath, string contentType, Guid relatedEntityId, string relatedEntityName, Guid? serviceRequestId = null)
        {
            FileName = fileName;
            FilePath = filePath;
            ContentType = contentType;
            RelatedEntityId = relatedEntityId;
            RelatedEntityName = relatedEntityName;
            ServiceRequestId = serviceRequestId;
        }
    }
}