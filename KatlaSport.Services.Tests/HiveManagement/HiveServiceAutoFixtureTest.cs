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
    public class HiveServiceAutoFixtureTest
    {
        private Mock<IProductStoreHiveContext> _context;

        private IUserContext _userContext;

        private HiveService _service;

        public HiveServiceAutoFixtureTest()
        {
            Mapper.Reset();
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<StoreHive, HiveListItem>().ReverseMap();
                cfg.CreateMap<StoreHiveSection, HiveSectionListItem>().ReverseMap();
                cfg.CreateMap<UpdateHiveRequest, StoreHive>().ReverseMap();
            });
        }

        [Theory]
        [AutoMoqData]
        public async void GetHivesAsync_TenEntities_SeccessfulResult([Frozen] Mock<IProductStoreHiveContext> context, HiveService service, IFixture fixture)
        {
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var hives = fixture.CreateMany<StoreHive>(10).ToList();
            var sections = fixture.CreateMany<StoreHiveSection>(10).ToList();
            context.Setup(c => c.Hives).ReturnsAsyncEntitySet(hives);
            context.Setup(c => c.Sections).ReturnsAsyncEntitySet(sections);

            var result = await service.GetHivesAsync();

            result.Should().HaveCount(hives.Count);
        }

        [Theory]
        [AutoMoqData]
        public async void GetHiveAsync_OneValidEntity_ValidEntityReturns([Frozen] Mock<IProductStoreHiveContext> context, HiveService service, IFixture fixture)
        {
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var hive = fixture.CreateMany<StoreHive>(1).ToList();
            context.Setup(c => c.Hives).ReturnsAsyncEntitySet(hive);

            var result = await service.GetHiveAsync(hive[0].Id);

            result.Code.Should().Be(hive[0].Code);
            result.Name.Should().Be(hive[0].Name);
            result.Address.Should().Be(hive[0].Address);
        }

        [Theory]
        [AutoMoqData]
        public async void GetHiveAsync_NotExistedEntityIdentifier_CustomExceptionThrows([Frozen] Mock<IProductStoreHiveContext> context, HiveService service, IFixture fixture)
        {
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var hive = fixture.CreateMany<StoreHive>(10).ToList();
            context.Setup(c => c.Hives).ReturnsAsyncEntitySet(hive);

            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.GetHiveAsync(0));
        }

        [Theory]
        [AutoMoqData]
        public async void CreateHiveAsync_ValidEntity_SuccessfullHiveAddition([Frozen] Mock<IProductStoreHiveContext> context, HiveService service, IFixture fixture)
        {
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var hive = fixture.CreateMany<StoreHive>(10).ToList();
            context.Setup(c => c.Hives).ReturnsAsyncEntitySet(hive);
            var updateHiveRequest = fixture.Create<UpdateHiveRequest>();

            var result = await service.CreateHiveAsync(updateHiveRequest);

            result.Code.Should().Be(updateHiveRequest.Code);
            result.Name.Should().Be(updateHiveRequest.Name);
            result.Address.Should().Be(updateHiveRequest.Address);
        }

        [Theory]
        [AutoMoqData]
        public async void CreateHiveAsync_EntityWithExistedCode_CustomExceptionThrows([Frozen] Mock<IProductStoreHiveContext> context, HiveService service, IFixture fixture)
        {
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var hive = fixture.CreateMany<StoreHive>(10).ToList();
            context.Setup(c => c.Hives).ReturnsAsyncEntitySet(hive);
            var updateHiveRequest = fixture.Create<UpdateHiveRequest>();
            updateHiveRequest.Code = hive[0].Code;

            await Assert.ThrowsAsync<RequestedResourceHasConflictException>(() => service.CreateHiveAsync(updateHiveRequest));
        }

        [Theory]
        [AutoMoqData]
        public async void UpdateHiveAsync_ValidEntity_SuccessfullHiveEdition([Frozen] Mock<IProductStoreHiveContext> context, HiveService service, IFixture fixture)
        {
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var hive = fixture.CreateMany<StoreHive>(10).ToList();
            context.Setup(c => c.Hives).ReturnsAsyncEntitySet(hive);
            var updateHiveRequest = fixture.Create<UpdateHiveRequest>();
            int id = hive[0].Id;

            var result = await service.UpdateHiveAsync(id, updateHiveRequest);

            result.Id.Should().Be(id);
            result.Code.Should().Be(updateHiveRequest.Code);
            result.Name.Should().Be(updateHiveRequest.Name);
            result.Address.Should().Be(updateHiveRequest.Address);
        }

        [Theory]
        [AutoMoqData]
        public async void UpdateHiveAsync_EntityWithExistedCode_CustomExceptionThrows([Frozen] Mock<IProductStoreHiveContext> context, HiveService service, IFixture fixture)
        {
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var hive = fixture.CreateMany<StoreHive>(10).ToList();
            context.Setup(c => c.Hives).ReturnsAsyncEntitySet(hive);
            var updateHiveRequest = fixture.Create<UpdateHiveRequest>();
            updateHiveRequest.Code = hive[0].Code;
            int id = hive[1].Id;

            await Assert.ThrowsAsync<RequestedResourceHasConflictException>(() => service.UpdateHiveAsync(id, updateHiveRequest));
        }

        [Theory]
        [AutoMoqData]
        public async void UpdateHiveAsync_NotExistedEntityIdentifier_CustomExceptionThrows([Frozen] Mock<IProductStoreHiveContext> context, HiveService service, IFixture fixture)
        {
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var hive = fixture.CreateMany<StoreHive>(10).ToList();
            context.Setup(c => c.Hives).ReturnsAsyncEntitySet(hive);
            var updateHiveRequest = fixture.Create<UpdateHiveRequest>();
            int id = 0;

            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.UpdateHiveAsync(id, updateHiveRequest));
        }

        [Theory]
        [AutoMoqData]
        public async void DeleteAsync_ExistedIdentifierFlagIsDeletedTrue_SuccessfulDeleting([Frozen] Mock<IProductStoreHiveContext> context, HiveService service, IFixture fixture)
        {
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var hive = fixture.CreateMany<StoreHive>(10).ToList();
            context.Setup(c => c.Hives).ReturnsAsyncEntitySet(hive);
            var id = hive[0].Id;
            hive[0].IsDeleted = true;

            await service.DeleteHiveAsync(id);

            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.GetHiveAsync(id));
        }

        [Theory]
        [AutoMoqData]
        public async void DeleteAsync_NotExistedIdentifierFlagIsDeletedTrue_CustomExceptionThrows([Frozen] Mock<IProductStoreHiveContext> context, HiveService service, IFixture fixture)
        {
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var hive = fixture.CreateMany<StoreHive>(10).ToList();
            context.Setup(c => c.Hives).ReturnsAsyncEntitySet(hive);
            var id = 0;
            hive[0].IsDeleted = true;

            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.DeleteHiveAsync(id));
        }

        [Theory]
        [AutoMoqData]
        public async void DeleteAsync_ExistedIdentifierFlagIsDeletedFalse_CustomExceptionThrows([Frozen] Mock<IProductStoreHiveContext> context, HiveService service, IFixture fixture)
        {
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var hive = fixture.CreateMany<StoreHive>(10).ToList();
            context.Setup(c => c.Hives).ReturnsAsyncEntitySet(hive);
            var id = hive[0].Id;
            hive[0].IsDeleted = false;

            await Assert.ThrowsAsync<RequestedResourceHasConflictException>(() => service.DeleteHiveAsync(id));
        }

        [Theory]
        [AutoMoqData]
        public async void SetStatusAsync_EntityHasFlagIsDeletedTrueSetFalse_SuccsessfulChange([Frozen] Mock<IProductStoreHiveContext> context, HiveService service, IFixture fixture)
        {
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var hive = fixture.CreateMany<StoreHive>(10).ToList();
            context.Setup(c => c.Hives).ReturnsAsyncEntitySet(hive);
            hive[0].IsDeleted = true;

            await service.SetStatusAsync(hive[0].Id, false);

            hive[0].IsDeleted.Should().Be(false);
        }

        [Theory]
        [AutoMoqData]
        public async void SetStatusAsync_EntityHasFlagIsDeletedFalseSetTrue_SuccsessfulChange([Frozen] Mock<IProductStoreHiveContext> context, HiveService service, IFixture fixture)
        {
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var hive = fixture.CreateMany<StoreHive>(10).ToList();
            context.Setup(c => c.Hives).ReturnsAsyncEntitySet(hive);
            hive[0].IsDeleted = false;

            await service.SetStatusAsync(hive[0].Id, true);

            hive[0].IsDeleted.Should().Be(true);
        }

        [Theory]
        [AutoMoqData]
        public async void SetStatusAsync_NotExistedEntityIdentifier_CustomExceptionThrows([Frozen] Mock<IProductStoreHiveContext> context, HiveService service, IFixture fixture)
        {
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var hive = fixture.CreateMany<StoreHive>(10).ToList();
            context.Setup(c => c.Hives).ReturnsAsyncEntitySet(hive);
            var id = 0;

            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.SetStatusAsync(id, false));
        }
    }
}