using Application.Common.Mappings;
using AutoMapper;

namespace Application.Features.Receiving.Suppliers.Dtos;

public class SupplierDto : IMapFrom<Domain.Receiving.Supplier>
{
    public Guid Id { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public string ContactPerson { get; set; } = string.Empty;
    public string TinNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public int ReceivingNotesCount { get; set; }
    public int TotalItemsReceived { get; set; }
    public decimal TotalPurchaseValue { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Domain.Receiving.Supplier, SupplierDto>()
            .ForMember(d => d.ReceivingNotesCount, opt => opt.Ignore())
            .ForMember(d => d.TotalItemsReceived, opt => opt.Ignore())
            .ForMember(d => d.TotalPurchaseValue, opt => opt.Ignore());
    }
}

public class SupplierDetailDto : SupplierDto
{
    public List<SupplierReceivingNoteDto> ReceivingNotes { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public List<SupplierContactDto> Contacts { get; set; } = new();
}

public class SupplierReceivingNoteDto
{
    public Guid Id { get; set; }
    public string GRNNumber { get; set; } = string.Empty;
    public DateTime ReceivedDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string ReceivedBy { get; set; } = string.Empty;
    public int ItemsCount { get; set; }
    public int TotalQuantity { get; set; }
    public decimal TotalValue { get; set; }
}

public class SupplierContactDto
{
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
}