using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using RealEstate.Application.DTOs;
using RealEstate.Application.Services;
using RealEstate.Domain.Entities;
using RealEstate.Infrastructure;

namespace RealEstate.Tests
{
    [TestFixture]
    public class PropertyServiceTests
    {
        private RealEstateDbContext _db = default!;
        private IPropertyService _service = default!;

        [SetUp]
        public void Setup()
        {
            _db = TestDbHelper.CreateInMemory();
            _service = new PropertyService(_db);

            _db.Owners.Add(new Owner { Name = "Owner 1", Address = "Addr 1" });
            _db.SaveChanges();
        }

        [Test]
        public async Task Create_And_Get_Should_Work()
        {
            var ownerId = _db.Owners.First().IdOwner;
            var create = new CreatePropertyRequest
            {
                Name = "House",
                Address = "123 Main",
                Price = 100000,
                CodeInternal = "CODE-1",
                Year = 2000,
                IdOwner = ownerId
            };

            var entity = await _service.CreateAsync(create);
            entity.IdProperty.Should().BeGreaterThan(0);

            var fetched = await _service.GetAsync(entity.IdProperty);
            fetched.Should().NotBeNull();
            fetched!.CodeInternal.Should().Be("CODE-1");
            fetched.Owner.IdOwner.Should().Be(ownerId);
        }

        [Test]
        public async Task Create_Should_Throw_On_Duplicate_CodeInternal()
        {
            var ownerId = _db.Owners.First().IdOwner;
            await _service.CreateAsync(new CreatePropertyRequest
            {
                Name = "A",
                Address = "A",
                Price = 1,
                CodeInternal = "DUP",
                Year = 2001,
                IdOwner = ownerId
            });

            var act = async () => await _service.CreateAsync(new CreatePropertyRequest
            {
                Name = "B",
                Address = "B",
                Price = 2,
                CodeInternal = "DUP",
                Year = 2002,
                IdOwner = ownerId
            });

            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*already exists*");
        }

        [Test]
        public async Task Update_Should_Modify_And_Check_Unique_Code()
        {
            var ownerId = _db.Owners.First().IdOwner;
            var a = await _service.CreateAsync(new CreatePropertyRequest
            {
                Name = "A",
                Address = "AA",
                Price = 1,
                CodeInternal = "U-1",
                Year = 1999,
                IdOwner = ownerId
            });
            var b = await _service.CreateAsync(new CreatePropertyRequest
            {
                Name = "B",
                Address = "BB",
                Price = 2,
                CodeInternal = "U-2",
                Year = 2000,
                IdOwner = ownerId
            });

            var updated = await _service.UpdateAsync(a.IdProperty, new UpdatePropertyRequest
            {
                Name = "A2",
                Address = "AA2",
                Price = 10,
                CodeInternal = "U-1",
                Year = 2001,
                IdOwner = ownerId
            });
            updated.Name.Should().Be("A2");

            var act = async () => await _service.UpdateAsync(a.IdProperty, new UpdatePropertyRequest
            {
                Name = "A3",
                Address = "AA3",
                Price = 20,
                CodeInternal = "U-2",
                Year = 2002,
                IdOwner = ownerId
            });
            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*already exists*");
        }

        [Test]
        public async Task Update_Should_Throw_When_Owner_NotFound()
        {
            var ownerId = _db.Owners.First().IdOwner;
            var p = await _service.CreateAsync(new CreatePropertyRequest
            {
                Name = "X",
                Address = "AX",
                Price = 1,
                CodeInternal = "C-X",
                Year = 2000,
                IdOwner = ownerId
            });

            var act = async () => await _service.UpdateAsync(p.IdProperty, new UpdatePropertyRequest
            {
                Name = "X2",
                Address = "AX2",
                Price = 2,
                CodeInternal = "C-X",
                Year = 2001,
                IdOwner = 9999
            });
            await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("*Owner not found*");
        }

        [Test]
        public async Task ChangePrice_Should_Update_Value()
        {
            var ownerId = _db.Owners.First().IdOwner;
            var prop = await _service.CreateAsync(new CreatePropertyRequest
            {
                Name = "Depto",
                Address = "1 Ocean",
                Price = 1200000,
                CodeInternal = "CODE-2",
                Year = 2001,
                IdOwner = ownerId
            });

            var updated = await _service.ChangePriceAsync(prop.IdProperty, 1300000);
            updated.Price.Should().Be(1300000);
        }

        [Test]
        public async Task AddImage_And_Trace_Should_Work()
        {
            var ownerId = _db.Owners.First().IdOwner;
            var prop = await _service.CreateAsync(new CreatePropertyRequest
            {
                Name = "Depto2",
                Address = "10 Beach",
                Price = 750000,
                CodeInternal = "CODE-3",
                Year = 2010,
                IdOwner = ownerId
            });

            var img = await _service.AddImageAsync(prop.IdProperty, "https://cdn/img1.jpg", true);
            img.IdPropertyImage.Should().BeGreaterThan(0);

            var trace = await _service.AddTraceAsync(prop.IdProperty, new AddTraceRequest
            {
                DateSale = DateTime.UtcNow.Date,
                Name = "Buyer A",
                Value = 800000,
                Tax = 10000
            });
            trace.IdPropertyTrace.Should().BeGreaterThan(0);
        }

        [Test]
        public async Task GetTraces_Should_Return_Sorted_Desc()
        {
            var ownerId = _db.Owners.First().IdOwner;
            var prop = await _service.CreateAsync(new CreatePropertyRequest
            {
                Name = "THouse",
                Address = "Trace Ave",
                Price = 1,
                CodeInternal = "TRACE-1",
                Year = 2000,
                IdOwner = ownerId
            });

            await _service.AddTraceAsync(prop.IdProperty, new AddTraceRequest
            {
                DateSale = new DateTime(2020, 1, 1),
                Name = "A",
                Value = 1,
                Tax = 0
            });
            await _service.AddTraceAsync(prop.IdProperty, new AddTraceRequest
            {
                DateSale = new DateTime(2021, 1, 1),
                Name = "B",
                Value = 2,
                Tax = 0
            });

            var traces = await _service.GetTracesAsync(prop.IdProperty);
            traces.Should().HaveCount(2);
            traces[0].DateSale.Should().Be(new DateTime(2021, 1, 1));
            traces[1].DateSale.Should().Be(new DateTime(2020, 1, 1));
        }

        [Test]
        public async Task List_With_Filters_Sort_And_Paginate()
        {
            var ownerId = _db.Owners.First().IdOwner;
            for (int i = 0; i < 25; i++)
            {
                await _service.CreateAsync(new CreatePropertyRequest
                {
                    Name = $"Prop {i}",
                    Address = $"Addr {i}",
                    Price = 300000 + i * 10000,
                    CodeInternal = $"CODE-{i:D3}",
                    Year = 1990 + (i % 30),
                    IdOwner = ownerId
                });
            }

            var page1 = await _service.ListAsync(new ListPropertiesQuery { MinPrice = 320000, Page = 1, PageSize = 10, SortBy = "price", Desc = true });
            page1.Items.Should().HaveCount(10);
            page1.Total.Should().BeGreaterThan(10);

            var page3 = await _service.ListAsync(new ListPropertiesQuery { Page = 3, PageSize = 10 });
            page3.Items.Count.Should().BeGreaterThanOrEqualTo(5);
        }
    }
}