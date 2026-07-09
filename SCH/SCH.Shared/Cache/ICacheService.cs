namespace SCH.Shared.Cache
{
    using System.Diagnostics.CodeAnalysis;

    public interface ICacheService
    {
        [return: MaybeNull]
        T Get<T>(string key);

        void Add<T>(
            string key, 
            T value,
            TimeSpan? absoluteExpirationRelativeToNow = null,
            TimeSpan? slidingExpiration = null,
            DateTimeOffset? absoluteExpiration = null);

        void Remove(string key);

        void Clear();
    }
}
