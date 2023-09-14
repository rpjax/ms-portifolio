//using ModularSystem.Core.Security;

//namespace ModularSystem.Core
//{
//    public interface IUseCase
//    {
//        public IResourcePolicy GetResourcePolicy();
//        public Task EnforceResourcePolicy();
//    }

//    public abstract class UseCaseBase : IUseCase
//    {
//        protected UseCaseBase()
//        {

//        }

//        public abstract IResourcePolicy GetResourcePolicy();
//        public virtual async Task EnforceResourcePolicy()
//        {
//            await GetResourcePolicy().AuthorizeAsync(GetContext().Identity);
//        }
//        protected abstract RequestContext GetContext();
//    }
//}
