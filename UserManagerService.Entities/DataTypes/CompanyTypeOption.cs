using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace UserManagerService.Entities.Datatypes
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CompanyTypeOption
    {
        LLC = 1
    }
}