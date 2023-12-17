namespace ECSina.App.ApiModel;

public class ApiDataEntity
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required Guid? CreatedById { get; set; }

    public List<ApiDataComponent>? Components { get; set; }
}
