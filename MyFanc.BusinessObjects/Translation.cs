using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFanc.BusinessObjects
{
    public class Translation
    {
        public int Id { get; set; }
        public string LanguageCode { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;

        public virtual ICollection<Wizard> WizardsTitles { get; set; } = new HashSet<Wizard>();
        public virtual ICollection<Wizard> WizardsTexts { get; set; } = new HashSet<Wizard>();

        public virtual ICollection<Question> QuestionsTitles { get; set; } = new HashSet<Question>();
        public virtual ICollection<Question> QuestionsTexts { get; set; } = new HashSet<Question>();

        public virtual ICollection<Answer> AnswersLabels { get; set; } = new HashSet<Answer>();
        public virtual ICollection<Answer> AnswersDetails { get; set; } = new HashSet<Answer>();
        public virtual ICollection<Answer> AnswersLinks { get; set; } = new HashSet<Answer>();
        public virtual ICollection<Answer> AnswersFinalAnswerTexts { get; set; } = new HashSet<Answer>();

        public virtual ICollection<Form> FormsLabels { get; set; } = new HashSet<Form>();
        public virtual ICollection<Form> FormsDescriptions { get; set; } = new HashSet<Form>();
        public virtual ICollection<Form> FormsUrls{ get; set; } = new HashSet<Form>();

        public virtual ICollection<FormNodes> FormNodesLabels { get; set; } = new HashSet<FormNodes>();
        public virtual ICollection<FormNodeFields> FormNodeFieldsLabels { get; set; } = new HashSet<FormNodeFields>();
        public virtual ICollection<FormValueFields> FormValueFieldsLabels { get; set; } = new HashSet<FormValueFields>();
        public virtual ICollection<PersonaCategories> PersonaCategoriesLabel { get; set; } = new HashSet<PersonaCategories>();

    }
}
