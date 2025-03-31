namespace LibMgmt.Repositories.interfaces
{
    public interface IBaseRepo<T, K>
    {
        Task<RepositoryResult<T?>> Add(T value);
        Task<RepositoryResult<T?>> GetByKey(K key);
        Task<RepositoryResult<bool>> Delete(K key);
        Task<RepositoryResult<IEnumerable<T>>> GetAll();
        Task<RepositoryResult<T?>> Update(T value);
    }
}
