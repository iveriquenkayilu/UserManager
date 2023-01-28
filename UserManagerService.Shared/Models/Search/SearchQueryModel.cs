namespace UserManagerService.Shared.Models.Search
{
    public class SearchQueryModel
    {
        public string Key { get; set; }
        public bool Companies { get; set; } = true;
        public bool Users { get; set; }=true;
    }
}
