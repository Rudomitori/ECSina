using AutoMapper;
using ECSina.App.ApiModel.Auth;
using ECSina.App.ApiModel.Forums;
using ECSina.Db.Entities;
using ECSina.Db.Entities.Auth;
using ECSina.Db.Entities.Forums;

namespace ECSina.App.ApiModel;

public sealed class ApiModelMapperProfile : Profile
{
    public ApiModelMapperProfile()
    {
        CreateMap<DataEntity, ApiDataEntity>();
        CreateMap<DataComponent, ApiDataComponent>();

        CreateMap<HierarchyComponent, ApiHierarchyComponent>()
            .IncludeBase<DataComponent, ApiDataComponent>();

        CreateMap<UserComponent, ApiUserComponent>().IncludeBase<DataComponent, ApiDataComponent>();
        CreateMap<PasswordComponent, ApiPasswordComponent>()
            .IncludeBase<DataComponent, ApiDataComponent>();
        CreateMap<RolesComponent, ApiRolesComponent>()
            .IncludeBase<DataComponent, ApiDataComponent>();

        CreateMap<ForumComponent, ApiForumComponent>()
            .IncludeBase<DataComponent, ApiDataComponent>();

        CreateMap<TopicComponent, ApiTopicComponent>()
            .IncludeBase<DataComponent, ApiDataComponent>();

        CreateMap<Message, ApiMessage>();
    }
}
