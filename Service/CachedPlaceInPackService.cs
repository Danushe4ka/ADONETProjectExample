﻿using Lab2Places.Models;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2Places.Service
{
    internal class CachedPlaceInPackService(Db8011Context db8011Context, IMemoryCache memoryCache) : ICachedPlaceInPackService
    {
        private readonly IMemoryCache _memoryCache = memoryCache;
        private readonly Db8011Context _db8011Context = db8011Context;

        public IEnumerable<PlaceInPack> GetObject(int rowsNumber = 20)
        {
            return _db8011Context.PlaceInPacks.Take(rowsNumber).ToList();
        }

        public void AddObjects(string cacheKey, int rowsNumber = 20)
        {
            IEnumerable<PlaceInPack> placeInPacks = _db8011Context.PlaceInPacks.Take(rowsNumber).ToList();
            if (placeInPacks != null)
            {
                _memoryCache.Set(cacheKey, placeInPacks, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(244)
                });
            }
        }

        public IEnumerable<PlaceInPack> GetObject(string cacheKey, int rowsNumber = 20)
        {
            if (!_memoryCache.TryGetValue(cacheKey, out IEnumerable<PlaceInPack> placeInPacks))
            {
                placeInPacks = _db8011Context.PlaceInPacks.Take(rowsNumber).ToList();
                if (placeInPacks != null)
                {
                    _memoryCache.Set(cacheKey, placeInPacks,
                        new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(244)));
                }
            }
            return placeInPacks;
        }
    }
}
