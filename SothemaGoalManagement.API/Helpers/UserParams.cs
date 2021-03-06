namespace SothemaGoalManagement.API.Helpers
{
    public class UserParams
    {
        private const int MaxPageSize = 50;
        public int PageNumber { get; set; } = 1;
        private int pageSize = 10;
        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = (value > MaxPageSize ? MaxPageSize : value); }
        }

        public int UserId { get; set; }
        // public string Gender { get; set; }

        public int DepartmentId { get; set; } = 0;

        public int UserStatusId { get; set; } = 0;

        public string UserToSearch { get; set; }

        public string OrderBy { get; set; }

    }
}