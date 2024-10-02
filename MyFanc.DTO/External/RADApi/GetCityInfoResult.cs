namespace MyFanc.DTO.External.RADApi
{
    public class GetCityInfoResult
    {
        public int NisCode { get; set; }
        public int PostCode { get; set; }
        public string OfficialLangCode1 { get; set; } = string.Empty;
        public string OfficialLangCode2 { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public IEnumerable<CityNameInfo> Names { get; set; } = Enumerable.Empty<CityNameInfo>();

    }

    public class CityNameInfo
    {
        public string LangCode { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}
