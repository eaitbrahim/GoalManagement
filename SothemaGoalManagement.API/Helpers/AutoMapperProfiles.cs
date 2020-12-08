using System;
using System.Linq;
using AutoMapper;
using SothemaGoalManagement.API.Dtos;
using SothemaGoalManagement.API.Models;

namespace SothemaGoalManagement.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForListDto>().ForMember(dest => dest.PhotoUrl, opt =>
            {
                opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
            });

            CreateMap<User, UserForSearchResultDto>().ForMember(dest => dest.LastEvaluationFile, opt =>
            {
                opt.MapFrom(src => src.EvaluationFileInstances.OrderByDescending(efi => efi.Year).FirstOrDefault().Title);
            });

            CreateMap<User, UserForDetailDto>().ForMember(dest => dest.PhotoUrl, opt =>
            {
                opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
            });

            CreateMap<Photo, PhotosForDetailDto>();

            CreateMap<ProfileForUpdateDto, User>();

            CreateMap<UserForUpdateDto, User>().ForMember(dest => dest.UserName, opt =>
            {
                opt.MapFrom(src => src.Email);
            }).ForMember(dest => dest.NormalizedUserName, opt =>
            {
                opt.MapFrom(src => src.Email.ToUpper());
            }).ForMember(dest => dest.NormalizedEmail, opt =>
            {
                opt.MapFrom(src => src.Email.ToUpper());
            });

            CreateMap<Photo, PhotoForReturnDto>();

            CreateMap<PhotoForCreationDto, Photo>();

            CreateMap<UserForRegisterDto, User>();

            CreateMap<MessageForCreationDto, Message>().ReverseMap();

            CreateMap<Message, MessageToReturnDto>()
                .ForMember(m => m.SenderPhotoUrl, opt => opt.MapFrom(u => u.Sender.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(m => m.RecipientPhotoUrl, opt => opt.MapFrom(u => u.Recipient.Photos.FirstOrDefault(p => p.IsMain).Url));

            CreateMap<Department, DepartmentToReturnDto>();

            CreateMap<UserStatus, UserStatusToReturnDto>();

            CreateMap<Strategy, StrategyForListDto>().ForMember(dest => dest.OwnerName, opt =>
            {
                opt.ResolveUsing(u => u.Owner.FirstName.FullName(u.Owner.LastName));
            });

            CreateMap<StrategyForCreationDto, Strategy>();

            CreateMap<StrategyForUpdateDto, Strategy>();

            CreateMap<Strategy, StrategyToReturnDto>().ForMember(dest => dest.OwnerName, opt =>
            {
                opt.ResolveUsing(u => u.Owner.FirstName.FullName(u.Owner.LastName));
            });

            CreateMap<AxisForCreationDto, Axis>();

            CreateMap<Axis, AxisToReturnDto>();

            CreateMap<AxisForUpdateDto, Axis>();

            CreateMap<AxisPole, AxisPoleToReturnDto>();

            CreateMap<BehavioralSkillForCreationDto, BehavioralSkill>();

            CreateMap<BehavioralSkillForUpdateDto, BehavioralSkill>();

            CreateMap<BehavioralSkill, BehavioralSkillToReturnDto>().ForMember(dest => dest.CreatedByName, opt =>
            {
                opt.ResolveUsing(u => u.CreatedBy.FirstName.FullName(u.CreatedBy.LastName));
            });

            CreateMap<BehavioralSkillInstance, BehavioralSkillToReturnDto>();

            CreateMap<EvaluationFileForCreationDto, EvaluationFile>();

            CreateMap<EvaluationFileForUpdateDto, EvaluationFile>().ForMember(x => x.BehavioralSkills, opt => opt.Ignore()); ;

            CreateMap<ParametersForCreationDto, Parameters>();

            CreateMap<Parameters, ParametersToReturnDto>();

            CreateMap<AxisInstance, AxisInstanceToReturnDto>();

            CreateMap<EvaluationFileInstance, EvaluationFileInstanceToReturnDto>().ForMember(dest => dest.AxisInstances, opt =>
            {
                opt.ResolveUsing(u => u.AxisInstances);
            }).ForMember(dest => dest.OwnerName, opt =>
            {
                opt.ResolveUsing(u => u.Owner.FirstName.FullName(u.Owner.LastName));
            }).ForMember(dest => dest.OwnerTitle, opt =>
            {
                opt.ResolveUsing(u => u.Owner.Title);
            }).ForMember(dest => dest.OwnerId, opt =>
            {
                opt.ResolveUsing(u => u.Owner.Id);
            }).ForMember(dest => dest.EmployeeNumber, opt =>
            {
                opt.ResolveUsing(u => u.Owner.EmployeeNumber);
            }).ForMember(dest => dest.PhotoUrl, opt =>
            {
                opt.MapFrom(src => src.Owner.Photos.FirstOrDefault(p => p.IsMain).Url);
            }).ForMember(dest => dest.Parameters, opt =>
            {
                opt.MapFrom(src => src.EvaluationFile.Parameters);
            });

            CreateMap<EvaluationFileInstance, EvaluationSheetToReturnDto>().ForMember(dest => dest.PoleName, opt =>
            {
                opt.ResolveUsing(u => u.AxisInstances.First().PoleName);
            }).ForMember(dest => dest.FullName, opt =>
            {
                opt.ResolveUsing(u => u.Owner.FirstName + " " + u.Owner.LastName);
            }).ForMember(dest => dest.GoalsStatus, opt =>
            {
                opt.ResolveUsing(u =>
                {
                    var firstAxisInstance = u.AxisInstances.FirstOrDefault();
                    var firstGoal = firstAxisInstance.Goals.FirstOrDefault();
                    return firstGoal != null ? firstGoal.Status : null;
                });
            }).ForMember(dest => dest.ValidatorValidationDateTime, opt =>
            {
                opt.ResolveUsing(src =>
                {
                    var firstAxisInstance = src.AxisInstances.FirstOrDefault();
                    var firstGoal = firstAxisInstance.Goals.FirstOrDefault();
                    DateTime? latestGoalEvalDateTime = DateTime.MinValue;
                    if (firstGoal != null)
                    {
                        var latestGoalEval = firstGoal.GoalEvaluations.LastOrDefault();
                        if (latestGoalEval != null) latestGoalEvalDateTime = latestGoalEval.Created;
                    }
                    return latestGoalEvalDateTime;
                });
            });

            CreateMap<GoalType, GoalTypeToReturnDto>();
            CreateMap<Project, ProjectToReturnDto>();

            CreateMap<GoalForCreationDto, Goal>();

            CreateMap<GoalForUpdateDto, Goal>();

            CreateMap<Goal, GoalToReturnDto>().ForMember(dest => dest.CascaderFullName, opt =>
            {
                opt.MapFrom(src => src.AxisInstance.EvaluationFileInstance != null ? src.AxisInstance.EvaluationFileInstance.Owner.FirstName + " " + src.AxisInstance.EvaluationFileInstance.Owner.LastName : "");
            }).ForMember(dest => dest.CascaderPhotoUrl, opt =>
            {
                opt.MapFrom(src => src.AxisInstance.EvaluationFileInstance != null ? (src.AxisInstance.EvaluationFileInstance.Owner.Photos.FirstOrDefault(p => p.IsMain) != null ? src.AxisInstance.EvaluationFileInstance.Owner.Photos.FirstOrDefault(p => p.IsMain).Url : "") : "");
            });

            CreateMap<Goal, GoalWithChildrenToReturnDto>().ForMember(dest => dest.AxisInstanceTitle, opt =>
            {
                opt.MapFrom(src => src.AxisInstance.Title);
            }).ForMember(dest => dest.GoalTypeName, opt =>
            {
                opt.MapFrom(src => src.GoalType.Name);
            }).ForMember(dest => dest.OwnerFullName, opt =>
            {
                opt.MapFrom(src => src.AxisInstance.EvaluationFileInstance != null ? src.AxisInstance.EvaluationFileInstance.Owner.FirstName + " " + src.AxisInstance.EvaluationFileInstance.Owner.LastName : "");
            }).ForMember(dest => dest.OwnerPhotoUrl, opt =>
            {
                opt.MapFrom(src => src.AxisInstance.EvaluationFileInstance != null ? (src.AxisInstance.EvaluationFileInstance.Owner.Photos.FirstOrDefault(p => p.IsMain) != null ? src.AxisInstance.EvaluationFileInstance.Owner.Photos.FirstOrDefault(p => p.IsMain).Url : "") : "");
            });

            CreateMap<GoalEvaluationForCreationDto, GoalEvaluation>();

            CreateMap<GoalEvaluation, GoalEvaluationToReturnDto>().ForMember(dest => dest.EvaluatorName, opt =>
            {
                opt.ResolveUsing(u => u.Evaluator.FirstName.FullName(u.Evaluator.LastName));
            });

            CreateMap<BehavioralSkillEvaluationForCreationDto, BehavioralSkillEvaluation>();

            CreateMap<BehavioralSkillEvaluation, BehavioralSkillEvaluationToReturnDto>().ForMember(dest => dest.EvaluatorName, opt =>
            {
                opt.ResolveUsing(u => u.Evaluator.FirstName.FullName(u.Evaluator.LastName));
            });

            CreateMap<FinalEvaluationDto, EvaluationFileInstance>();
        }
    }
}