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
                .ForMember(d => d.DocumentAnalysisId, o => o.MapFrom(s => s.DocumentAnalysisID))
                .ForMember(d => d.KeyConcepts, o => o.MapFrom(s => s.KeyConcepts))
                .ForMember(d => d.SampleQuestions, o => o.MapFrom(s => s.SampleQuestions));

            // KEY CONCEPTS
            CreateMap<KeyConcept, KeyConceptDto>()
                .ForMember(d => d.KeyConceptId, o => o.MapFrom(s => s.KeyConceptID));

            // SAMPLE QUESTIONS
            CreateMap<SampleQuestion, SampleQuestionDto>()
                .ForMember(d => d.SampleQuestionId, o => o.MapFrom(s => s.SampleQuestionID))
                .ForMember(d => d.Type, o => o.MapFrom(s => s.Type.ToString()))
                .ForMember(d => d.Difficulty, o => o.MapFrom(s => s.Difficulty.ToString()))
                .ForMember(d => d.Choices, o => o.MapFrom(s => s.Choices));

            // QUESTION CHOICES
            CreateMap<QuestionChoice, QuestionChoiceDto>()
                .ForMember(d => d.QuestionChoiceId, o => o.MapFrom(s => s.QuestionChoiceID));
        }
    }
}