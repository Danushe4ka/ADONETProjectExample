using Lab2Places.Models;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2Places.Service
{
    internal class CachedReviewService(Db8011Context db8011Context, IMemoryCache memoryCache) : ICachedReviewService
    {
        private readonly IMemoryCache _memoryCache = memoryCache;
        private readonly Db8011Context _db8011Context = db8011Context;

        public IEnumerable<Review> GetObject(int rowsNumber = 20)
        {
            return _db8011Context.Reviews.Take(rowsNumber).ToList();
        }

        public void AddObjects(string cacheKey, int rowsNumber = 20)
        {
            IEnumerable<Review> reviews = _db8011Context.Reviews.Take(rowsNumber).ToList();
            if (reviews != null)
            {
                _memoryCache.Set(cacheKey, reviews, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(244)
                });
            }
        }

        public IEnumerable<Review> GetObject(string cacheKey, int rowsNumber = 20)
        {
            if (!_memoryCache.TryGetValue(cacheKey, out IEnumerable<Review> reviews))
            {
                reviews = _db8011Context.Reviews.Take(rowsNumber).ToList();
                if (reviews != null)
                {
                    _memoryCache.Set(cacheKey, reviews,
                        new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(244)));
                }
            }
            return reviews;
        }
    }
}
