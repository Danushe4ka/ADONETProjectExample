using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2Places.Service
{
    public class CachedPlacesTypeService(Db8011Context db8011Context, IMemoryCache memoryCache) : ICachedPlacesType
    {
        private readonly IMemoryCache _memoryCache = memoryCache;
        private readonly Db8011Context _db8011Context = db8011Context;

        public IEnumerable<PlacesType> GetObject(int rowsNumber = 20)
        {
            return _db8011Context.PlacesTypes.Take(rowsNumber).ToList();
        }

        public void AddObjects(string cacheKey, int rowsNumber = 20)
        {
            IEnumerable<PlacesType> placesTypes = _db8011Context.PlacesTypes.Take(rowsNumber).ToList();
            if(placesTypes != null)
            {
                _memoryCache.Set(cacheKey, placesTypes, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(244)
                });
            }
        }

        public IEnumerable<PlacesType> GetObject(string cacheKey, int rowsNumber = 20)
        {
            if( !_memoryCache.TryGetValue(cacheKey, out IEnumerable<PlacesType> placesTypes))
            {
                placesTypes = _db8011Context.PlacesTypes.Take(rowsNumber).ToList();
                if(placesTypes != null)
                {
                    _memoryCache.Set(cacheKey, placesTypes,
                        new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(244)));
                }
            }
            return placesTypes;
        }
    }
}
