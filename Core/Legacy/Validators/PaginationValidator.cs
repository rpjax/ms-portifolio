namespace ModularSystem.Core
{
    public class PaginationInValidator
    {
        public PaginationInValidator(PaginationIn pagination)
        {
            this.pagination = pagination;
        }

        PaginationIn pagination { get; }
        readonly int defaultLimit = 30;

        public void Validate()
        {
            ValidateLimit();
            ValdiateOffset();
        }

        void ValidateLimit()
        {
            if (pagination.Limit == 0)
            {
                pagination.Limit = defaultLimit;
            }
            if (pagination.Limit < 0)
            {
                pagination.Limit = pagination.Limit * -1;
            }
        }
        void ValdiateOffset()
        {
            if (pagination.Offset < 0)
            {
                pagination.Offset = pagination.Offset * -1;
            }
        }
    }
}