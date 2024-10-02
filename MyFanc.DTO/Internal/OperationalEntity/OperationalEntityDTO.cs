using MyFanc.DTO.Internal.Data;
using static MyFanc.Core.Enums;

namespace MyFanc.DTO.Internal.OperationalEntity
{
    public class OperationalEntityDTO
    {
        public string Id { get; set; } = string.Empty;
        public string IntracommunityNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public OeDataOrigin DataOrigin { get; set; }
        public bool Activated { get; set; }
        public string DefaultLanguageCode { get; set; } = string.Empty;
        public IEnumerable<NacabelsCodeDTO> Sectors { get; set; } = Enumerable.Empty<NacabelsCodeDTO>();

    }
}
