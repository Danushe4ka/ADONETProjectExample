using Lab2Places.Models;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2Places.Service
{
    internal class CachedPackService(Db8011Context db8011Context, IMemoryCache memoryCache) : ICachedPackService
    {
        private readonly IMemoryCache _memoryCache = memoryCache;
        private readonly Db8011Context _db8011Context = db8011Context;

        public IEnumerable<Pack> GetObject(int rowsNumber = 20)
        {
            return _db8011Context.Packs.Take(rowsNumber).ToList();
        }

        public void AddObjects(string cacheKey, int rowsNumber = 20)
        {
            IEnumerable<Pack> packs = _db8011Context.Packs.Take(rowsNumber).ToList();
            if (packs != null)
            {
                _memoryCache.Set(cacheKey, packs, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(244)
                });
            }
        }

        public IEnumerable<Pack> GetObject(string cacheKey, int rowsNumber = 20)
        {
            if (!_memoryCache.TryGetValue(cacheKey, out IEnumerable<Pack> packs))
            {
                packs = _db8011Context.Packs.Take(rowsNumber).ToList();
                if (packs != null)
                {
                    _memoryCache.Set(cacheKey, packs,
                        new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(244)));
                }
            }
            return packs;
        }
    }
}
