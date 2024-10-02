using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyFanc.Core
{
    public class Enums
    {
        public enum AuthRedirection
        {
            ProfilePage = 0,
            WelcomePage = 1,
            WaitingValidationPage = 2
        }

        public enum ValidationStatus
        {
            Pending = 0,
            Approved = 1,
            Refused = 2
        }

        public enum DataProcessingType
        {
            Breadcrumb,
            Others
        }

        public enum OeDataOrigin
        {
            CBE = 1,
            Manual = 2
        }

        public enum EmailType
        {
            Confirmation = 1,
            Invitation = 2,
            DeleteUserFromOe = 3,
            AcceptInvitation = 4,
            RefuseInvitation = 5
        }

        public enum SendEmailStatus
        {
            Success = 1,
            Failed = 2,
            Approved = 3
        }
        public enum FormStatus
        {
            Draft,
            Online,
            Offline
        }

        public enum FormType
        {
            Block,
            Webform,
            Pdf
        }

        [Flags]
        public enum IsUsedParam
        {
            All, 
            Used,
            NotUsed
        }
        public enum FormNodeType
        {
            FormField,
            FormBlock,
            Section
        }

        public enum FormNodeFieldType
        {
            [Description("Text")]
            Text,
            [Description("Check Box")]
            CheckBox,
            [Description("List")]
            List,
            [Description("Upload File")]
            UploadFile,
            [Description("Paragraph")]
            Paragraph,
            [Description("Hidden Value")]
            HiddenValue
        }

        public enum PersonaType
        {
            //[Description("User")]
            User = 1,
            //[Description("Company")]
            Company = 2
        }

        public enum FormNodeFieldEncodeType
        {
            Text,
            Translation,
            ArrayText,
            File,
            CustomList,
            PredefineList,
            Conditional,
            Container
        }

        public enum FormSubmissionType
        {
            Webform,
            Pdf
        }

        public static Dictionary<string, string> GetEnumDescriptions<TEnum>()
        {
            if (!typeof(TEnum).IsEnum)
            {
                throw new ArgumentException($"{nameof(TEnum)} must be an enumerated type");
            }

            Dictionary<string, string> enumDictionary = new Dictionary<string, string>();

            foreach (TEnum value in Enum.GetValues(typeof(TEnum)))
            {
                FieldInfo? fieldInfo = value.GetType().GetField(value.ToString()??"");

                DescriptionAttribute? descriptionAttribute = fieldInfo != null ? (DescriptionAttribute?)Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute)) : null;

                string description = (descriptionAttribute != null) ? descriptionAttribute.Description : value.ToString() ?? "";

                enumDictionary.Add(value.ToString() ?? "", description);
            }

            return enumDictionary;
        }

    }
}
