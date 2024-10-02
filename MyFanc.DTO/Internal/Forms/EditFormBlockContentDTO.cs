using MyFanc.Core;

namespace MyFanc.DTO.Internal.Forms
{
    public class EditFormBlockContentDTO
    {
        public Guid Id { get; set; }
        public Guid FormId { get; set; }
        public Guid NodeId { get; set; }
        public string FieldType { get; set; } = Enums.FormNodeFieldType.Text.ToString();
        public string Property { get; set; } = String.Empty;
        public string Type { get; set; } =  Enums.FormNodeType.FormField.ToString();
        //public PropertyValueBase? Value { get; set; }
    }
}
