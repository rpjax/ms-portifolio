namespace ModularSystem.Core;

public class SearchRequestValidator<T> where T : class
{
    readonly IQuery<T> request;

    public SearchRequestValidator(IQuery<T> request)
    {
        this.request = request;
    }


    public void Validate()
    {
        ValidatePagination();
    }

    void ValidatePagination() => new PaginationInValidator(request.Pagination).Validate();

}