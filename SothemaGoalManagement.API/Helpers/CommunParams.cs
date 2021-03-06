namespace SothemaGoalManagement.API.Helpers
{
    public class CommunParams
    {
        private const int MaxPageSize = 500;
        public int PageNumber { get; set; } = 1;
        private int pageSize = 10;
        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = (value > MaxPageSize ? MaxPageSize : value); }
        }

        public int OwnerId { get; set; }

        public string Status { get; set; }

        public int Year { get; set; }

        public string OrderBy { get; set; }

        public string UserToSearch { get; set; }
        public int PoleId { get; set; } = 0;
        public int UserStatusId { get; set; } = 0;

    }
}