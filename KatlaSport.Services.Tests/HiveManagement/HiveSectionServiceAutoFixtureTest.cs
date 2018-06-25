using System.Linq;
using AutoFixture;
using AutoFixture.Xunit2;
using AutoMapper;
using FluentAssertions;
using KatlaSport.DataAccess.ProductStoreHive;
using KatlaSport.Services.HiveManagement;
using Moq;
using Xunit;

namespace KatlaSport.Services.Tests.HiveManagement
{
    public class HiveSectionServiceAutoFixtureTest
    {
        private Mock<IProductStoreHiveContext> _context;

        private IUserContext _userContext;

        private HiveSectionService _service;

        public HiveSectionServiceAutoFixtureTest()
        {
            Mapper.Reset();
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<StoreHiveSection, HiveSection>().ReverseMap();
                cfg.CreateMap<StoreHiveSection, HiveSectionListItem>().ReverseMap();
                cfg.CreateMap<UpdateHiveSectionRequest, StoreHiveSection>().ReverseMap();
            });
        }

        [Theory]
        [AutoMoqData]
        public async void GetHiveSectionsAsync_TenEntities_SeccessfulResult([Frozen] Mock<IProductStoreHiveContext> context, HiveSectionService service, IFixture fixture)
        {
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var sections = fixture.CreateMany<StoreHiveSection>(10).ToList();
            context.Setup(c => c.Sections).ReturnsAsyncEntitySet(sections);

            var result = await service.GetHiveSectionsAsync();

            result.Should().HaveCount(sections.Count);
        }

        [Theory]
        [AutoMoqData]
        public async void GetHiveSectionAsync_OneValidEntity_ValidEntityReturns([Frozen] Mock<IProductStoreHiveContext> context, HiveSectionService service, IFixture fixture)
        {
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var section = fixture.CreateMany<StoreHiveSection>(1).ToList();
            context.Setup(c => c.Sections).ReturnsAsyncEntitySet(section);

            var result = await service.GetHiveSectionAsync(section[0].Id);

            result.Code.Should().Be(section[0].Code);
            result.Name.Should().Be(section[0].Name);
        }

        [Theory]
        [AutoMoqData]
        public async void GetHiveSectionAsync_NotExistedEntityIdentifier_CustomExceptionThrows([Frozen] Mock<IProductStoreHiveContext> context, HiveSectionService service, IFixture fixture)
        {
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var section = fixture.CreateMany<StoreHiveSection>(10).ToList();
            context.Setup(c => c.Sections).ReturnsAsyncEntitySet(section);
            var id = 0;

            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.GetHiveSectionAsync(id));
        }

        [Theory]
        [AutoMoqData]
        public async void CreateHiveSectionAsync_ValidEntity_SuccessfullHiveAddition([Frozen] Mock<IProductStoreHiveContext> context, HiveSectionService service, IFixture fixture)
        {
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var section = fixture.CreateMany<StoreHiveSection>(10).ToList();
            context.Setup(c => c.Sections).ReturnsAsyncEntitySet(section);
            var updateHiveSectionRequest = fixture.Create<UpdateHiveSectionRequest>();

            var result = await service.CreateHiveSectionAsync(updateHiveSectionRequest);

            result.Code.Should().Be(updateHiveSectionRequest.Code);
            result.Name.Should().Be(updateHiveSectionRequest.Name);
        }

        [Theory]
        [AutoMoqData]
        public async void CreateHiveSectionAsync_EntityWithExistedCode_CustomExceptionThrows([Frozen] Mock<IProductStoreHiveContext> context, HiveSectionService service, IFixture fixture)
        {
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var section = fixture.CreateMany<StoreHiveSection>(10).ToList();
            context.Setup(c => c.Sections).ReturnsAsyncEntitySet(section);
            var updateHiveSectionRequest = fixture.Create<UpdateHiveSectionRequest>();
            updateHiveSectionRequest.Code = section[0].Code;

            await Assert.ThrowsAsync<RequestedResourceHasConflictException>(() => service.CreateHiveSectionAsync(updateHiveSectionRequest));
        }

        [Theory]
        [AutoMoqData]
        public async void UpdateHiveSectionAsync_ValidEntity_SuccessfullHiveEdition([Frozen] Mock<IProductStoreHiveContext> context, HiveSectionService service, IFixture fixture)
        {
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var section = fixture.CreateMany<StoreHiveSection>(10).ToList();
            context.Setup(c => c.Sections).ReturnsAsyncEntitySet(section);
            var updateHiveSectionRequest = fixture.Create<UpdateHiveSectionRequest>();
            int id = section[0].Id;

            var result = await service.UpdateHiveSectionAsync(id, updateHiveSectionRequest);

            result.Id.Should().Be(id);
            result.Code.Should().Be(updateHiveSectionRequest.Code);
            result.Name.Should().Be(updateHiveSectionRequest.Name);
        }

        [Theory]
        [AutoMoqData]
        public async void UpdateHiveSectionAsync_NotExistedEntityIdentifier_CustomExceptionThrows([Frozen] Mock<IProductStoreHiveContext> context, HiveSectionService service, IFixture fixture)
        {
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var section = fixture.CreateMany<StoreHiveSection>(10).ToList();
            context.Setup(c => c.Sections).ReturnsAsyncEntitySet(section);
            var updateHiveSectionRequest = fixture.Create<UpdateHiveSectionRequest>();
            int id = 0;

            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.UpdateHiveSectionAsync(id, updateHiveSectionRequest));
        }

        [Theory]
        [AutoMoqData]
        public async void DeleteHiveSectionAsync_ExistedIdentifierFlagIsDeletedTrue_SuccessfulDeleting([Frozen] Mock<IProductStoreHiveContext> context, HiveSectionService service, IFixture fixture)
        {
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var section = fixture.CreateMany<StoreHiveSection>(10).ToList();
            context.Setup(c => c.Sections).ReturnsAsyncEntitySet(section);
            var id = section[0].Id;
            section[0].IsDeleted = true;

            await service.DeleteHiveSectionAsync(id);

            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.GetHiveSectionAsync(id));
        }

        [Theory]
        [AutoMoqData]
        public async void DeleteHiveSectionAsync_NotExistedIdentifierFlagIsDeletedTrue_CustomExceptionThrows([Frozen] Mock<IProductStoreHiveContext> context, HiveSectionService service, IFixture fixture)
        {
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var section = fixture.CreateMany<StoreHiveSection>(10).ToList();
            context.Setup(c => c.Sections).ReturnsAsyncEntitySet(section);
            var id = 0;
            section[0].IsDeleted = true;

            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.DeleteHiveSectionAsync(id));
        }

        [Theory]
        [AutoMoqData]
        public async void DeleteHiveSectionAsync_ExistedIdentifierFlagIsDeletedFalse_CustomExceptionThrows([Frozen] Mock<IProductStoreHiveContext> context, HiveSectionService service, IFixture fixture)
        {
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var section = fixture.CreateMany<StoreHiveSection>(10).ToList();
            context.Setup(c => c.Sections).ReturnsAsyncEntitySet(section);
            var id = section[0].Id;
            section[0].IsDeleted = false;

            await Assert.ThrowsAsync<RequestedResourceHasConflictException>(() => service.DeleteHiveSectionAsync(id));
        }

        [Theory]
        [AutoMoqData]
        public async void SetStatusAsync_EntityHasFlagIsDeletedTrueSetFalse_SuccsessfulChange([Frozen] Mock<IProductStoreHiveContext> context, HiveSectionService service, IFixture fixture)
        {
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var section = fixture.CreateMany<StoreHiveSection>(10).ToList();
            context.Setup(c => c.Sections).ReturnsAsyncEntitySet(section);
            section[0].IsDeleted = true;

            await service.SetStatusAsync(section[0].Id, false);

            section[0].IsDeleted.Should().Be(false);
        }

        [Theory]
        [AutoMoqData]
        public async void SetStatusAsync_EntityHasFlagIsDeletedFalseSetTrue_SuccsessfulChange([Frozen] Mock<IProductStoreHiveContext> context, HiveSectionService service, IFixture fixture)
        {
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var section = fixture.CreateMany<StoreHiveSection>(10).ToList();
            context.Setup(c => c.Sections).ReturnsAsyncEntitySet(section);
            section[0].IsDeleted = false;

            await service.SetStatusAsync(section[0].Id, true);

            section[0].IsDeleted.Should().Be(true);
        }

        [Theory]
        [AutoMoqData]
        public async void SetStatusAsync_NotExistedEntityIdentifier_CustomExceptionThrows([Frozen] Mock<IProductStoreHiveContext> context, HiveSectionService service, IFixture fixture)
        {
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var section = fixture.CreateMany<StoreHiveSection>(10).ToList();
            context.Setup(c => c.Sections).ReturnsAsyncEntitySet(section);
            var id = 0;

            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.SetStatusAsync(id, false));
        }
    }
}