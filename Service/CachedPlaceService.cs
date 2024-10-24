using Lab2Places.Models;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2Places.Service
{
    public class CachedPlaceService(Db8011Context db8011Context, IMemoryCache memoryCache) : ICachedPlaceService
    {
        private readonly IMemoryCache _memoryCache = memoryCache;
        private readonly Db8011Context _db8011Context = db8011Context;

        public IEnumerable<Place> GetObject(int rowsNumber = 20)
        {
            return _db8011Context.Places.Take(rowsNumber).ToList();
        }

        public void AddObjects(string cacheKey, int rowsNumber = 20)
        {
            IEnumerable<Place> places = _db8011Context.Places.Take(rowsNumber).ToList();
            if (places != null)
            {
                _memoryCache.Set(cacheKey, places, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(244)
                });
            }
        }

        public IEnumerable<Place> GetObject(string cacheKey, int rowsNumber = 20)
        {
            if (!_memoryCache.TryGetValue(cacheKey, out IEnumerable<Place> places))
            {
                places = _db8011Context.Places.Take(rowsNumber).ToList();
                if (places != null)
                {
                    _memoryCache.Set(cacheKey, places,
                        new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(244)));
                }
            }
            return places;
        }
    }
}
