namespace DatingApp.API.Helpers
{
    public class PaginationHeader
    {
         public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int ItemsPerPage { get; set; }

        public int TotalItems { get; set; }

        public PaginationHeader(int cp, int ipp, int ti, int tp)
        {
            CurrentPage = cp;
            ItemsPerPage = ipp;
            TotalPages = tp;
            this.TotalItems = ti;
        }
    }
}