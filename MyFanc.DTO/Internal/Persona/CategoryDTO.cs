using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFanc.DTO.Internal.Persona
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int TagId { get; set; }
        public string TagTitle { get; set; } = string.Empty;
        public bool SelectedEnable { get; set; }
        public List<CategoryDTO> SubItems { get; set; } = new List<CategoryDTO>();
    }


    public class SubCategoryDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int TagId { get; set; }
        public string TagTitle { get; set; } = string.Empty;
        public bool SelectedEnable { get; set; }
    }
}
