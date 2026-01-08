using WorkTrace.Data.Common.Generics;

namespace WorkTrace.Data.Models;

public class FormTemplate : BaseModel
{
    public string Name { get; set; }
    public string Description { get; set; }
    public List<FormQuestion> Questions { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt {  get; set; }
}