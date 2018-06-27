using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.Xunit2;
using AutoMapper;
using FluentAssertions;
using KatlaSport.DataAccess.ProductCatalogue;
using KatlaSport.Services.ProductManagement;
using Moq;
using Xunit;

namespace KatlaSport.Services.Tests.ProductManagement
{
    public class ProductCatalogueServiceAutoFixtureTests
    {
        private List<CatalogueProduct> _product;

        public ProductCatalogueServiceAutoFixtureTests()
        {
            Mapper.Reset();
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<CatalogueProduct, Product>().ReverseMap();
                cfg.CreateMap<CatalogueProduct, ProductListItem>().ReverseMap();
                cfg.CreateMap<UpdateProductRequest, CatalogueProduct>().ReverseMap();
            });
        }

        [Theory]
        [AutoMoqData]
        public async void GetProductsAsync_TenEntities_SeccessfulResult([Frozen] Mock<IProductCatalogueContext> context, ProductCatalogueService service, IFixture fixture)
        {
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _product = fixture.CreateMany<CatalogueProduct>(10).ToList();
            context.Setup(c => c.Products).ReturnsAsyncEntitySet(_product);

            var result = await service.GetProductsAsync(0, _product.Count);

            result.Should().HaveCount(_product.Count);
        }

        [Theory]
        [AutoMoqData]
        public async void GetProductAsync_OneValidEntity_ValidEntityReturns([Frozen] Mock<IProductCatalogueContext> context, ProductCatalogueService service, IFixture fixture)
        {
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _product = fixture.CreateMany<CatalogueProduct>(1).ToList();
            context.Setup(c => c.Products).ReturnsAsyncEntitySet(_product);

            var result = await service.GetProductAsync(_product[0].Id);

            result.Code.Should().Be(_product[0].Code);
            result.Name.Should().Be(_product[0].Name);
        }

        [Theory]
        [AutoMoqData]
        public async void GetProductAsync_NotExistedEntityIdentifier_CustomExceptionThrows([Frozen] Mock<IProductCatalogueContext> context, ProductCatalogueService service, IFixture fixture)
        {
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _product = fixture.CreateMany<CatalogueProduct>(10).ToList();
            context.Setup(c => c.Products).ReturnsAsyncEntitySet(_product);
            var id = 0;

            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.GetProductAsync(id));
        }

        [Theory]
        [AutoMoqData]
        public async void CreateProductAsync_ValidEntity_SuccessfullHiveAddition([Frozen] Mock<IProductCatalogueContext> context, ProductCatalogueService service, IFixture fixture)
        {
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _product = fixture.CreateMany<CatalogueProduct>(10).ToList();
            context.Setup(c => c.Products).ReturnsAsyncEntitySet(_product);
            var updateProductRequest = fixture.Create<UpdateProductRequest>();

            var result = await service.CreateProductAsync(updateProductRequest);

            result.Code.Should().Be(updateProductRequest.Code);
            result.Name.Should().Be(updateProductRequest.Name);
        }

        [Theory]
        [AutoMoqData]
        public async void CreateProductAsync_EntityWithExistedCode_CustomExceptionThrows([Frozen] Mock<IProductCatalogueContext> context, ProductCatalogueService service, IFixture fixture)
        {
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _product = fixture.CreateMany<CatalogueProduct>(10).ToList();
            context.Setup(c => c.Products).ReturnsAsyncEntitySet(_product);
            var updateProductRequest = fixture.Create<UpdateProductRequest>();
            updateProductRequest.Code = _product[0].Code;

            await Assert.ThrowsAsync<RequestedResourceHasConflictException>(() => service.CreateProductAsync(updateProductRequest));
        }

        [Theory]
        [AutoMoqData]
        public async void UpdateProductAsync_ValidEntity_SuccessfullHiveEdition([Frozen] Mock<IProductCatalogueContext> context, ProductCatalogueService service, IFixture fixture)
        {
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _product = fixture.CreateMany<CatalogueProduct>(10).ToList();
            context.Setup(c => c.Products).ReturnsAsyncEntitySet(_product);
            var updateProductRequest = fixture.Create<UpdateProductRequest>();
            int id = _product[0].Id;

            var result = await service.UpdateProductAsync(id, updateProductRequest);

            result.Id.Should().Be(id);
            result.Code.Should().Be(updateProductRequest.Code);
            result.Name.Should().Be(updateProductRequest.Name);
        }

        [Theory]
        [AutoMoqData]
        public async void UpdateProductAsync_EntityWithExistedCode_CustomExceptionThrows([Frozen] Mock<IProductCatalogueContext> context, ProductCatalogueService service, IFixture fixture)
        {
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _product = fixture.CreateMany<CatalogueProduct>(10).ToList();
            context.Setup(c => c.Products).ReturnsAsyncEntitySet(_product);
            var updateProductRequest = fixture.Create<UpdateProductRequest>();
            updateProductRequest.Code = _product[0].Code;
            int id = _product[1].Id;

            await Assert.ThrowsAsync<RequestedResourceHasConflictException>(() => service.UpdateProductAsync(id, updateProductRequest));
        }

        [Theory]
        [AutoMoqData]
        public async void UpdateProductAsync_NotExistedEntityIdentifier_CustomExceptionThrows([Frozen] Mock<IProductCatalogueContext> context, ProductCatalogueService service, IFixture fixture)
        {
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _product = fixture.CreateMany<CatalogueProduct>(10).ToList();
            context.Setup(c => c.Products).ReturnsAsyncEntitySet(_product);
            var updateProductRequest = fixture.Create<UpdateProductRequest>();
            int id = 0;

            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.UpdateProductAsync(id, updateProductRequest));
        }

        [Theory]
        [AutoMoqData]
        public async void DeleteAsync_ExistedIdentifierFlagIsDeletedTrue_SuccessfulDeleting([Frozen] Mock<IProductCatalogueContext> context, ProductCatalogueService service, IFixture fixture)
        {
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _product = fixture.CreateMany<CatalogueProduct>(10).ToList();
            context.Setup(c => c.Products).ReturnsAsyncEntitySet(_product);
            var id = _product[0].Id;
            _product[0].IsDeleted = true;

            await service.DeleteProductAsync(id);

            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.GetProductAsync(id));
        }

        [Theory]
        [AutoMoqData]
        public async void DeleteProductAsync_NotExistedIdentifierFlagIsDeletedTrue_CustomExceptionThrows([Frozen] Mock<IProductCatalogueContext> context, ProductCatalogueService service, IFixture fixture)
        {
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _product = fixture.CreateMany<CatalogueProduct>(10).ToList();
            context.Setup(c => c.Products).ReturnsAsyncEntitySet(_product);
            var id = 0;
            _product[0].IsDeleted = true;

            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.DeleteProductAsync(id));
        }

        [Theory]
        [AutoMoqData]
        public async void DeleteProductAsync_ExistedIdentifierFlagIsDeletedFalse_CustomExceptionThrows([Frozen] Mock<IProductCatalogueContext> context, ProductCatalogueService service, IFixture fixture)
        {
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _product = fixture.CreateMany<CatalogueProduct>(10).ToList();
            context.Setup(c => c.Products).ReturnsAsyncEntitySet(_product);
            var id = _product[0].Id;
            _product[0].IsDeleted = false;

            await Assert.ThrowsAsync<RequestedResourceHasConflictException>(() => service.DeleteProductAsync(id));
        }

        [Theory]
        [AutoMoqData]
        public async void SetStatusAsync_EntityHasFlagIsDeletedTrueSetFalse_SuccsessfulChange([Frozen] Mock<IProductCatalogueContext> context, ProductCatalogueService service, IFixture fixture)
        {
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _product = fixture.CreateMany<CatalogueProduct>(10).ToList();
            context.Setup(c => c.Products).ReturnsAsyncEntitySet(_product);
            _product[0].IsDeleted = true;

            await service.SetStatusAsync(_product[0].Id, false);

            _product[0].IsDeleted.Should().Be(false);
        }

        [Theory]
        [AutoMoqData]
        public async void SetStatusAsync_EntityHasFlagIsDeletedFalseSetTrue_SuccsessfulChange([Frozen] Mock<IProductCatalogueContext> context, ProductCatalogueService service, IFixture fixture)
        {
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _product = fixture.CreateMany<CatalogueProduct>(10).ToList();
            context.Setup(c => c.Products).ReturnsAsyncEntitySet(_product);
            _product[0].IsDeleted = false;

            await service.SetStatusAsync(_product[0].Id, true);

            _product[0].IsDeleted.Should().Be(true);
        }

        [Theory]
        [AutoMoqData]
        public async void SetStatusAsync_NotExistedEntityIdentifier_CustomExceptionThrows([Frozen] Mock<IProductCatalogueContext> context, ProductCatalogueService service, IFixture fixture)
        {
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _product = fixture.CreateMany<CatalogueProduct>(10).ToList();
            context.Setup(c => c.Products).ReturnsAsyncEntitySet(_product);
            var id = 0;

            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.SetStatusAsync(id, false));
        }
    }
}