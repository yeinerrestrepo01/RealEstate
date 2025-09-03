using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using RealEstate.Application.DTOs;
using RealEstate.Application.Services;
using RealEstate.Domain.Entities;
using RealEstate.Infrastructure;

namespace RealEstate.Tests
{
    [TestFixture]
    public class OwnerServiceTests
    {
        private RealEstateDbContext _db = default!;
        private IOwnerService _service = default!;

        [SetUp]
        public void Setup()
        {
            _db = TestDbHelper.CreateInMemory();
            _service = new OwnerService(_db);
        }

        [Test]
        public async Task Create_Should_CreateOwner()
        {
            var created = await _service.CreateAsync(new CreateOwnerRequest
            {
                Name = "Alice",
                Address = "Street 1",
                Photo = "https://img/1.jpg",
                Birthday = new DateTime(1990, 1, 1)
            });

            created.IdOwner.Should().BeGreaterThan(0);
            (await _db.Owners.CountAsync()).Should().Be(1);
        }

        [Test]
        public async Task Get_Should_ReturnOwner()
        {
            var o = _db.Owners.Add(new Owner { Name = "Bob", Address = "Addr" }).Entity;
            await _db.SaveChangesAsync();

            var found = await _service.GetAsync(o.IdOwner);
            found.Should().NotBeNull();
            found!.Name.Should().Be("Bob");
        }

        [Test]
        public async Task List_Should_Filter_And_Paginate()
        {
            _db.Owners.AddRange(
                new Owner { Name = "Ana", Address = "A1" },
                new Owner { Name = "Andres", Address = "A2" },
                new Owner { Name = "Bruno", Address = "B1" },
                new Owner { Name = "Carla", Address = "C1" }
            );
            await _db.SaveChangesAsync();

            var page = await _service.ListAsync(new ListOwnersQuery { Name = "An", Page = 1, PageSize = 2 });
            page.Total.Should().Be(2);
            page.Items.Should().HaveCount(2);
            page.Items.Select(x => x.Name).Should().Contain(new[] { "Ana", "Andres" });
        }

        [Test]
        public async Task Update_Should_ModifyFields()
        {
            var o = _db.Owners.Add(new Owner { Name = "Old", Address = "Addr" }).Entity;
            await _db.SaveChangesAsync();

            var updated = await _service.UpdateAsync(o.IdOwner, new UpdateOwnerRequest
            {
                Name = "New",
                Address = "Addr2",
                Photo = null
            });

            updated.Name.Should().Be("New");
            updated.Address.Should().Be("Addr2");
        }

        [Test]
        public async Task Delete_Should_ReturnFalse_When_NotFound()
        {
            var ok = await _service.DeleteAsync(123);
            ok.Should().BeFalse();
        }

        [Test]
        public async Task Delete_Should_Throw_When_HasProperties()
        {
            var owner = _db.Owners.Add(new Owner { Name = "X", Address = "A" }).Entity;
            _db.Properties.Add(new Property
            {
                Name = "P1",
                Address = "AA",
                Price = 1,
                CodeInternal = "C1",
                Year = 2000,
                IdOwner = owner.IdOwner
            });
            await _db.SaveChangesAsync();

            var act = async () => await _service.DeleteAsync(owner.IdOwner);
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*related properties*");
        }

        [Test]
        public async Task Delete_Should_Remove_When_NoProperties()
        {
            var owner = _db.Owners.Add(new Owner { Name = "Y", Address = "B" }).Entity;
            await _db.SaveChangesAsync();

            var ok = await _service.DeleteAsync(owner.IdOwner);
            ok.Should().BeTrue();
            (await _db.Owners.AnyAsync(o => o.IdOwner == owner.IdOwner)).Should().BeFalse();
        }
    }
}