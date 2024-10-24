using Lab2Places.Models;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2Places.Service
{
    internal class CachedUserService(Db8011Context db8011Context, IMemoryCache memoryCache) : ICachedUserService
    {
        private readonly IMemoryCache _memoryCache = memoryCache;
        private readonly Db8011Context _db8011Context = db8011Context;

        public IEnumerable<User> GetObject(int rowsNumber = 20)
        {
            return _db8011Context.Users.Take(rowsNumber).ToList();
        }

        public void AddObjects(string cacheKey, int rowsNumber = 20)
        {
            IEnumerable<User> users = _db8011Context.Users.Take(rowsNumber).ToList();
            if (users != null)
            {
                _memoryCache.Set(cacheKey, users, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(244)
                });
            }
        }

        public IEnumerable<User> GetObject(string cacheKey, int rowsNumber = 20)
        {
            if (!_memoryCache.TryGetValue(cacheKey, out IEnumerable<User> users))
            {
                users = _db8011Context.Users.Take(rowsNumber).ToList();
                if (users != null)
                {
                    _memoryCache.Set(cacheKey, users,
                        new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(244)));
                }
            }
            return users;
        }
    }
}
