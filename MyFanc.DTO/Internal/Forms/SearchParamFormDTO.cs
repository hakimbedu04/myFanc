
using Microsoft.EntityFrameworkCore.Diagnostics;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.Json.Serialization;

namespace MyFanc.DTO.Internal.Forms
{
   
    public class SearchParamFormDTO
    {
        [SwaggerParameter("Filter on 'ID' value", Required = false)]
        public int? Id { get; set; }
        
        [SwaggerParameter("Filter on 'Label' value", Required = false)]
        public string? Label { get; set; }

        [SwaggerParameter("Filter on 'CategoryId' value, multi select Ex: '1,2,3'. To get list of category use end point: '/Form/FormCategories' ", Required = false)]
        public string? Category { get; set; }

        [SwaggerParameter("Filter on 'Status' value, multi select Ex: '0,1'. Available status: 0 = Draft, 1 = Online, 2 = Offline", Required = false)]
        public string? Status { get; set; }

        [SwaggerParameter("Filter on 'Tag' value, multi select Ex: 'Transport,RadioPhysics,Dentist'. To get list of tags/sectors use end point: '/Data/sectors'", Required = false)]
        public string? Tag { get; set; }

        [SwaggerParameter("Filter on 'Type' value, multi select Ex: 'Webform,Pdf'. Available type:[Webform, Pdf]", Required = false)]
        public string? Type { get; set; }

        [SwaggerParameter("Current selected UI language, Ex: 'en'", Required = true)]
        public string LanguageCode { get; set; } = "en";    
    }

    public class SearchParamFormDTOExten : SearchParamFormDTO
    {
        public List<int> CategoriesList
        {
            get
            {
                return string.IsNullOrEmpty(Category) ? new List<int>() : Category.Split(',').ToList().Select(c => { int.TryParse(c, out int res); return res; }).ToList();
            }
        }
        public List<int> StatusList
        {
            get
            {
                return string.IsNullOrEmpty(Status) ? new List<int>() : Status.Split(',').ToList().Select(s => { int.TryParse(s, out int res); return res; }).ToList();
            }
        }
        public List<string> TagList
        {
            get
            {
                return string.IsNullOrEmpty(Tag) ? new List<string>() : Tag.ToLower().Split(',').ToList();
            }
        }
        public List<string> TypeList
        {
            get
            {
                return string.IsNullOrEmpty(Type) ? new List<string>() : Type.ToLower().Split(',').ToList();
            }

        }
    }
}
