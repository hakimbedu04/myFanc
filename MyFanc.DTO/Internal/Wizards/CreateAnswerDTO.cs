using MyFanc.DTO.Internal.Translation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFanc.DTO.Internal.Wizards
{
    public class CreateAnswerDTO
    {
        public int QuestionId { get; set; }
        public IEnumerable<TranslationDTO> Labels { get; set; } = Enumerable.Empty<TranslationDTO>();
        public IEnumerable<TranslationDTO> Details { get; set; } = Enumerable.Empty<TranslationDTO>();
        public IEnumerable<string> Tags { get; set; } = Enumerable.Empty<string>();
        public int? LinkedQuestionId { get; set; }
        public IEnumerable<TranslationDTO> Links { get; set; } = Enumerable.Empty<TranslationDTO>();
        public bool IsFinalAnswer { get; set; }
        public IEnumerable<TranslationDTO> FinalAnswerTexts { get; set; } = Enumerable.Empty<TranslationDTO>();
    }
}
