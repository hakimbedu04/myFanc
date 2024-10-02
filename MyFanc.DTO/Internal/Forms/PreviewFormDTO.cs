using MyFanc.DTO.Internal.Translation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MyFanc.Core.Enums;

namespace MyFanc.DTO.Internal.Forms
{
    public class PreviewFormDTO
    {
        public Guid FormId { get; set; }
        public PreviewDetailsDTO Details { get; set; } = new PreviewDetailsDTO() { };
        public List<NodesDTO> Nodes { get; set; } = new List<NodesDTO> { };
    }

    public class NodesDTO
    {
        public Guid Id { get; set; }
        public Guid ParentId { get; set; }
        public int Order { get; set; }
        public IEnumerable<TranslationDTO> Labels { get; set; } = new HashSet<TranslationDTO>();
        public string Type { get; set; } = string.Empty;
        public List<PropertiesDTO> Properties { get; set; } = new List<PropertiesDTO>() { };
        public List<FormConditionalDTO> Conditionals { get; set; } = new List<FormConditionalDTO>() { };
    }

    public class PropertiesDTO
    {
        public string Name { get; set; } = string.Empty;
        public object? Value { get; set;}
        public string Type { get; set; } = FormNodeFieldEncodeType.Text.ToString();
    }

    public class PreviewDetailsDTO
    {
        public string Version { get; set; } = string.Empty;
        public string CaregoryId { get; set; } = string.Empty;
        public IEnumerable<TranslationDTO> Labels { get; set; } = new HashSet<TranslationDTO>();
        public IEnumerable<TranslationDTO> Description { get; set; } = new HashSet<TranslationDTO>();
        public List<PreviewDetailsNacabelDTO> Nacabels { get; set; } = new List<PreviewDetailsNacabelDTO>() { };
        public string Type { get; set; } = string.Empty;
        public int ExternalId { get; set; }
        public List<PreviewDetailsDocumentDTO> Documents { get; set; } = new List<PreviewDetailsDocumentDTO>() { };
        public List<FormUrlDTO> Urls { get; set; } = new List<FormUrlDTO>() { };
    }
    public class PreviewDetailsDocumentDTO
    {
        public Guid Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }
    public class PreviewDetailsNacabelDTO
    {
        public string Code { get; set; } = string.Empty;
        public IEnumerable<TranslationDTO> Translations { get; set; } = new HashSet<TranslationDTO>();
    }
    public class DocumentsDTO
    {
        public Guid Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public string Mime { get; set; } = string.Empty;
    }
}
