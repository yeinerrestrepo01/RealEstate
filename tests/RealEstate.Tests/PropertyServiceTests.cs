using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using RealEstate.Application.DTOs;
using RealEstate.Application.Services;
using RealEstate.Infrastructure;

namespace RealEstate.Tests
{
    public class PropertyServiceTests
    {
        private IPropertyService _service = default!;
        private RealEstateDbContext _db = default!;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<RealEstateDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _db = new RealEstateDbContext(options);
            _service = new PropertyService(_db);
        }

        [Test]
        public async Task Create_And_Get_Should_Work()
        {
            var create = new CreatePropertyRequest
            {
                Code = "NYC-0001",
                Title = "Cozy Apartment in Manhattan",
                Address = "123 Main St",
                City = "New York",
                State = "NY",
                ZipCode = "10001",
                Bedrooms = 2,
                Bathrooms = 1.5m,
                AreaSqFt = 900,
                YearBuilt = 1995,
                Price = 950000
            };

            var entity = await _service.CreateAsync(create);
            entity.Id.Should().NotBe(Guid.Empty);

            var fetched = await _service.GetAsync(entity.Id);
            fetched.Should().NotBeNull();
            fetched!.Code.Should().Be("NYC-0001");
        }

        [Test]
        public async Task ChangePrice_Should_Update_Value()
        {
            var prop = await _service.CreateAsync(new CreatePropertyRequest
            {
                Code = "LA-0002",
                Title = "House in LA",
                Address = "1 Ocean Dr",
                City = "Los Angeles",
                State = "CA",
                ZipCode = "90001",
                Bedrooms = 3,
                Bathrooms = 2,
                AreaSqFt = 1500,
                YearBuilt = 2001,
                Price = 1200000
            });

            var updated = await _service.ChangePriceAsync(prop.Id, 1300000);
            updated.Price.Should().Be(1300000);
        }

        [Test]
        public async Task AddImage_Should_Set_Cover_Singleton()
        {
            var prop = await _service.CreateAsync(new CreatePropertyRequest
            {
                Code = "MIA-0003",
                Title = "Miami Beach Condo",
                Address = "10 Beach Ave",
                City = "Miami",
                State = "FL",
                ZipCode = "33101",
                Bedrooms = 2,
                Bathrooms = 2,
                AreaSqFt = 1100,
                YearBuilt = 2010,
                Price = 750000
            });

            var img1 = await _service.AddImageAsync(prop.Id, "https://cdn/img1.jpg", true);
            var img2 = await _service.AddImageAsync(prop.Id, "https://cdn/img2.jpg", true);

            var fetched = await _service.GetAsync(prop.Id);
            fetched!.Images.Count(i => i.IsCover).Should().Be(1);
            fetched.Images.Single(i => i.IsCover).Url.Should().Be("https://cdn/img2.jpg");
        }

        [Test]
        public async Task List_With_Filters_Should_Paginate()
        {
            for (int i = 0; i < 25; i++)
            {
                await _service.CreateAsync(new CreatePropertyRequest
                {
                    Code = $"SEA-{i:D4}",
                    Title = $"Seattle Apt {i}",
                    Address = $"{i} Pike St",
                    City = "Seattle",
                    State = "WA",
                    ZipCode = "98101",
                    Bedrooms = 1 + (i % 3),
                    Bathrooms = 1,
                    AreaSqFt = 600 + i * 10,
                    YearBuilt = 2000 + (i % 10),
                    Price = 300000 + i * 10000
                });
            }

            var page1 = await _service.ListAsync(new ListPropertiesQuery { City = "Seattle", Page = 1, PageSize = 10, SortBy = "price", Desc = true });
            page1.Items.Should().HaveCount(10);
            page1.Total.Should().Be(25);

            var page3 = await _service.ListAsync(new ListPropertiesQuery { City = "Seattle", Page = 3, PageSize = 10 });
            page3.Items.Should().HaveCount(5);
        }
    }
}
