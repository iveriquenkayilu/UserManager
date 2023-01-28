using System.Collections.Generic;

namespace UserManagerService.Shared.Models.Search
{
    public class ResultsModel
    {
        public List<SearchResultModel> Users { get; set; }
        public List<SearchResultModel> Companies { get; set; }
    }
}
