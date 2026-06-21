using Application.Features.Documents.DTOs;
using Application.Features.Users.DTOs;
using AutoMapper;
using Domain.Entities;

namespace Application.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // USERS
            CreateMap<User, UserDto>()
                .ForMember(d => d.UserId, o => o.MapFrom(s => s.UserID))
                .ForMember(d => d.UserRole, o => o.MapFrom(s => s.UserPermissions.ToString()));

            // DOCUMENTS
            CreateMap<Document, DocumentDto>()
                .ForMember(d => d.DocumentId, o => o.MapFrom(s => s.DocumentID))
                .ForMember(d => d.ContentType, o => o.MapFrom(s => s.ContentType.ToString()))
                .ForMember(d => d.ProcessingStatus, o => o.MapFrom(s => s.ProcessingStatus.ToString()))
                .ForMember(d => d.Analysis, o => o.MapFrom(s => s.DocumentAnalysis));

            // DOCUMENT ANALYSIS
            CreateMap<DocumentAnalysis, DocumentAnalysisDto>()
                .ForMember(d => d.DocumentAnalysisId, o => o.MapFrom(s => s.DocumentAnalysisID));
        }
    }
}