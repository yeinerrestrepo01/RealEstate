using System;
using Microsoft.EntityFrameworkCore;
using RealEstate.Infrastructure;

namespace RealEstate.Tests
{
    internal static class TestDbHelper
    {
        public static RealEstateDbContext CreateInMemory()
        {
            var options = new DbContextOptionsBuilder<RealEstateDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new RealEstateDbContext(options);
        }
    }
}