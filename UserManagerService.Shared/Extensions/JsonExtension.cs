using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace UserManagerService.Shared.Extensions
{
    public static class JsonExtension
    {
        /// <summary>
        /// Serializing class object to a camelized Json string.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJsonCamelized<T>(this T obj) where T : class
        {
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            return obj == null ? string.Empty : JsonConvert.SerializeObject(obj, jsonSerializerSettings);
        }
    }
}
