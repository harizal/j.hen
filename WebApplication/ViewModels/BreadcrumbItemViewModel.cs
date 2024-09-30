namespace WApp.ViewModels
{
    public class BreadcrumbItemViewModel
    {
        public string? Title { get; set; }
        public string? Url { get; set; }
        public bool IsActive { get; set; } = false;
    }
}
