using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper.Configuration.Annotations;

namespace MyFanc.BusinessObjects
{
    public class Wizard : AuditedEntity
    {

        public int Id { get; set; }

        public virtual ICollection<Translation> Titles { get; set; } = new HashSet<Translation>();
        public virtual ICollection<Translation> IntroductionTexts { get; set; } = new HashSet<Translation>();
        
        [Ignore]
        public virtual ICollection<Question> Questions { get; set; } = new HashSet<Question>();
    }
}
