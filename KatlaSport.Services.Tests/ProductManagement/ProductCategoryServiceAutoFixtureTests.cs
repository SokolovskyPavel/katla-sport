using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.Xunit2;
using AutoMapper;
using FluentAssertions;
using KatlaSport.DataAccess.ProductCatalogue;
using Bll = KatlaSport.Services.ProductManagement;
using Moq;
using Xunit;

namespace KatlaSport.Services.Tests.ProductManagement
{
    public class ProductCategoryServiceAutoFixtureTests
    {
        private List<ProductCategory> _productCategory;

        public ProductCategoryServiceAutoFixtureTests()
        {
            Mapper.Reset();
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<ProductCategory, ProductCategory>().ReverseMap();
                cfg.CreateMap<ProductCategory, Bll.ProductCategoryListItem>().ReverseMap();
                cfg.CreateMap<Bll.UpdateProductCategoryRequest, ProductCategory>().ReverseMap();
            });
        }

        [Theory]
        [AutoMoqData]
        public async void GetProductCategoriesAsync_TenEntities_SeccessfulResult([Frozen] Mock<IProductCatalogueContext> context, Bll.ProductCategoryService service, IFixture fixture)
        {
            Configure(context, fixture);
            var product = fixture.CreateMany<CatalogueProduct>(10).ToList();
            context.Setup(c => c.Products).ReturnsAsyncEntitySet(product);

            var result = await service.GetCategoriesAsync(0, _productCategory.Count);

            result.Should().HaveCount(_productCategory.Count);
        }

        [Theory]
        [AutoMoqData]
        public async void GetProductCategoryAsync_OneValidEntity_ValidEntityReturns([Frozen] Mock<IProductCatalogueContext> context, Bll.ProductCategoryService service, IFixture fixture)
        {
            Configure(context, fixture);

            var result = await service.GetCategoryAsync(_productCategory[0].Id);

            result.Code.Should().Be(_productCategory[0].Code);
            result.Name.Should().Be(_productCategory[0].Name);
        }

        [Theory]
        [AutoMoqData]
        public async void GetProductCategoryAsync_NotExistedEntityIdentifier_CustomExceptionThrows([Frozen] Mock<IProductCatalogueContext> context, Bll.ProductCategoryService service, IFixture fixture)
        {
            Configure(context, fixture);
            var id = 0;

            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.GetCategoryAsync(id));
        }

        [Theory]
        [AutoMoqData]
        public async void CreateProductCategoryAsync_ValidEntity_SuccessfullHiveAddition([Frozen] Mock<IProductCatalogueContext> context, Bll.ProductCategoryService service, IFixture fixture)
        {
            Configure(context, fixture);
            var updateProductCategoryRequest = fixture.Create<Bll.UpdateProductCategoryRequest>();

            var result = await service.CreateCategoryAsync(updateProductCategoryRequest);

            result.Code.Should().Be(updateProductCategoryRequest.Code);
            result.Name.Should().Be(updateProductCategoryRequest.Name);
        }

        [Theory]
        [AutoMoqData]
        public async void CreateProductCategoryAsync_EntityWithExistedCode_CustomExceptionThrows([Frozen] Mock<IProductCatalogueContext> context, Bll.ProductCategoryService service, IFixture fixture)
        {
            Configure(context, fixture);
            var updateProductCategoryRequest = fixture.Create<Bll.UpdateProductCategoryRequest>();
            updateProductCategoryRequest.Code = _productCategory[0].Code;

            await Assert.ThrowsAsync<RequestedResourceHasConflictException>(() => service.CreateCategoryAsync(updateProductCategoryRequest));
        }

        [Theory]
        [AutoMoqData]
        public async void UpdateProductCategoryAsync_ValidEntity_SuccessfullHiveEdition([Frozen] Mock<IProductCatalogueContext> context, Bll.ProductCategoryService service, IFixture fixture)
        {
            Configure(context, fixture);
            var updateProductCategoryRequest = fixture.Create<Bll.UpdateProductCategoryRequest>();
            int id = _productCategory[0].Id;

            var result = await service.UpdateCategoryAsync(id, updateProductCategoryRequest);

            result.Id.Should().Be(id);
            result.Code.Should().Be(updateProductCategoryRequest.Code);
            result.Name.Should().Be(updateProductCategoryRequest.Name);
        }

        [Theory]
        [AutoMoqData]
        public async void UpdateProductCategoryAsync_EntityWithExistedCode_CustomExceptionThrows([Frozen] Mock<IProductCatalogueContext> context, Bll.ProductCategoryService service, IFixture fixture)
        {
            Configure(context, fixture);
            var updateProductCategoryRequest = fixture.Create<Bll.UpdateProductCategoryRequest>();
            updateProductCategoryRequest.Code = _productCategory[0].Code;
            int id = _productCategory[1].Id;

            await Assert.ThrowsAsync<RequestedResourceHasConflictException>(() => service.UpdateCategoryAsync(id, updateProductCategoryRequest));
        }

        [Theory]
        [AutoMoqData]
        public async void UpdateProductCategoryAsync_NotExistedEntityIdentifier_CustomExceptionThrows([Frozen] Mock<IProductCatalogueContext> context, Bll.ProductCategoryService service, IFixture fixture)
        {
            Configure(context, fixture);
            var updateProductCategoryRequest = fixture.Create<Bll.UpdateProductCategoryRequest>();
            int id = 0;

            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.UpdateCategoryAsync(id, updateProductCategoryRequest));
        }

        [Theory]
        [AutoMoqData]
        public async void DeleteCategoryAsync_ExistedIdentifierFlagIsDeletedTrue_SuccessfulDeleting([Frozen] Mock<IProductCatalogueContext> context, Bll.ProductCategoryService service, IFixture fixture)
        {
            Configure(context, fixture);
            var id = _productCategory[0].Id;
            _productCategory[0].IsDeleted = true;

            await service.DeleteCategoryAsync(id);

            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.GetCategoryAsync(id));
        }

        [Theory]
        [AutoMoqData]
        public async void DeleteProductCategoryAsync_NotExistedIdentifierFlagIsDeletedTrue_CustomExceptionThrows([Frozen] Mock<IProductCatalogueContext> context, Bll.ProductCategoryService service, IFixture fixture)
        {
            Configure(context, fixture);
            var id = 0;
            _productCategory[0].IsDeleted = true;

            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.DeleteCategoryAsync(id));
        }

        [Theory]
        [AutoMoqData]
        public async void DeleteProductCategoryAsync_ExistedIdentifierFlagIsDeletedFalse_CustomExceptionThrows([Frozen] Mock<IProductCatalogueContext> context, Bll.ProductCategoryService service, IFixture fixture)
        {
            Configure(context, fixture);
            var id = _productCategory[0].Id;
            _productCategory[0].IsDeleted = false;

            await Assert.ThrowsAsync<RequestedResourceHasConflictException>(() => service.DeleteCategoryAsync(id));
        }

        [Theory]
        [AutoMoqData]
        public async void SetStatusAsync_EntityHasFlagIsDeletedTrueSetFalse_SuccsessfulChange([Frozen] Mock<IProductCatalogueContext> context, Bll.ProductCategoryService service, IFixture fixture)
        {
            Configure(context, fixture);
            _productCategory[0].IsDeleted = true;

            await service.SetStatusAsync(_productCategory[0].Id, false);

            _productCategory[0].IsDeleted.Should().Be(false);
        }

        [Theory]
        [AutoMoqData]
        public async void SetStatusAsync_EntityHasFlagIsDeletedFalseSetTrue_SuccsessfulChange([Frozen] Mock<IProductCatalogueContext> context, Bll.ProductCategoryService service, IFixture fixture)
        {
            Configure(context, fixture);
            _productCategory[0].IsDeleted = false;

            await service.SetStatusAsync(_productCategory[0].Id, true);

            _productCategory[0].IsDeleted.Should().Be(true);
        }

        [Theory]
        [AutoMoqData]
        public async void SetStatusAsync_NotExistedEntityIdentifier_CustomExceptionThrows([Frozen] Mock<IProductCatalogueContext> context, Bll.ProductCategoryService service, IFixture fixture)
        {
            Configure(context, fixture);
            var id = 0;

            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.SetStatusAsync(id, false));
        }

        private void Configure(Mock<IProductCatalogueContext> context, IFixture fixture)
        {
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _productCategory = fixture.CreateMany<ProductCategory>(10).ToList();
            context.Setup(c => c.Categories).ReturnsAsyncEntitySet(_productCategory);
        }
    }
}