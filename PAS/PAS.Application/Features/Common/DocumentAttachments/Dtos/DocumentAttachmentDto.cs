using Application.Common.Mappings;
using AutoMapper;

namespace Application.Features.Common.DocumentAttachments.Dtos;

public class DocumentAttachmentDto : IMapFrom<DocumentAttachment>
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public Guid RelatedEntityId { get; set; }
    public string RelatedEntityName { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
    public string UploadedBy { get; set; } = string.Empty;
    public string DownloadUrl { get; set; } = string.Empty;

    public void Mapping(Profile profile)
    {
        profile.CreateMap<DocumentAttachment, DocumentAttachmentDto>()
            .ForMember(d => d.FileSize, opt => opt.Ignore())
            .ForMember(d => d.UploadedBy, opt => opt.Ignore())
            .ForMember(d => d.DownloadUrl, opt => opt.Ignore());
    }
}