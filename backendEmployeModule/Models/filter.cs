namespace backendEmployeModule.Models
{
    public partial class filter

    {

        public string? gte { get; set; }
        public string? lte { get; set; }
        public decimal? min_pay { get; set; }
        public decimal? max_pay { get; set; }

        public bool? is_download { get; set; }

    }
}
