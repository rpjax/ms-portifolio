using ModularSystem.Core;

namespace ModularSystem.Tester;

public class CoinValdiator : IValidator<Coin>
{
    public Task<Exception?> ValidateAsync(Coin instance)
    {
        throw new NotImplementedException();
    }
}

public class PaperValdiator : IValidator<Paper>
{
    public Task<Exception?> ValidateAsync(Paper instance)
    {
        throw new NotImplementedException();
    }
}

public class CredentialValidator : IValidator<Credential>
{
    public Task<Exception?> ValidateAsync(Credential instance)
    {
        throw new NotImplementedException();
    }
}