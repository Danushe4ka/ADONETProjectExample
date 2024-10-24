using Lab2Places.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2Places.Service
{
    public interface ICachedPlaceService
    {
        public IEnumerable<Place> GetObject(int rowsNumber = 20);
        public IEnumerable<Place> GetObject(string cacheKey, int rowsNumber = 20);
        public void AddObjects(string cacheKey, int rowsNumber = 20);
    }
}
