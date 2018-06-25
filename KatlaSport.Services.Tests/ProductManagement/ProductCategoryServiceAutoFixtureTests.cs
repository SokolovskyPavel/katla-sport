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
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var productCategory = fixture.CreateMany<ProductCategory>(10).ToList();
            context.Setup(c => c.Categories).ReturnsAsyncEntitySet(productCategory);
            var product = fixture.CreateMany<CatalogueProduct>(10).ToList();
            context.Setup(c => c.Products).ReturnsAsyncEntitySet(product);

            var result = await service.GetCategoriesAsync(0, productCategory.Count);

            result.Should().HaveCount(productCategory.Count);
        }

        [Theory]
        [AutoMoqData]
        public async void GetProductCategoryAsync_OneValidEntity_ValidEntityReturns([Frozen] Mock<IProductCatalogueContext> context, Bll.ProductCategoryService service, IFixture fixture)
        {
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var productCategory = fixture.CreateMany<ProductCategory>(1).ToList();
            context.Setup(c => c.Categories).ReturnsAsyncEntitySet(productCategory);

            var result = await service.GetCategoryAsync(productCategory[0].Id);

            result.Code.Should().Be(productCategory[0].Code);
            result.Name.Should().Be(productCategory[0].Name);
        }

        [Theory]
        [AutoMoqData]
        public async void GetProductCategoryAsync_NotExistedEntityIdentifier_CustomExceptionThrows([Frozen] Mock<IProductCatalogueContext> context, Bll.ProductCategoryService service, IFixture fixture)
        {
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var productCategory = fixture.CreateMany<ProductCategory>(10).ToList();
            context.Setup(c => c.Categories).ReturnsAsyncEntitySet(productCategory);
            var id = 0;

            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.GetCategoryAsync(id));
        }

        [Theory]
        [AutoMoqData]
        public async void CreateProductCategoryAsync_ValidEntity_SuccessfullHiveAddition([Frozen] Mock<IProductCatalogueContext> context, Bll.ProductCategoryService service, IFixture fixture)
        {
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var productCategory = fixture.CreateMany<ProductCategory>(10).ToList();
            context.Setup(c => c.Categories).ReturnsAsyncEntitySet(productCategory);
            var updateProductCategoryRequest = fixture.Create<Bll.UpdateProductCategoryRequest>();

            var result = await service.CreateCategoryAsync(updateProductCategoryRequest);

            result.Code.Should().Be(updateProductCategoryRequest.Code);
            result.Name.Should().Be(updateProductCategoryRequest.Name);
        }

        [Theory]
        [AutoMoqData]
        public async void CreateProductCategoryAsync_EntityWithExistedCode_CustomExceptionThrows([Frozen] Mock<IProductCatalogueContext> context, Bll.ProductCategoryService service, IFixture fixture)
        {
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var productCategory = fixture.CreateMany<ProductCategory>(10).ToList();
            context.Setup(c => c.Categories).ReturnsAsyncEntitySet(productCategory);
            var updateProductCategoryRequest = fixture.Create<Bll.UpdateProductCategoryRequest>();
            updateProductCategoryRequest.Code = productCategory[0].Code;

            await Assert.ThrowsAsync<RequestedResourceHasConflictException>(() => service.CreateCategoryAsync(updateProductCategoryRequest));
        }

        [Theory]
        [AutoMoqData]
        public async void UpdateProductCategoryAsync_ValidEntity_SuccessfullHiveEdition([Frozen] Mock<IProductCatalogueContext> context, Bll.ProductCategoryService service, IFixture fixture)
        {
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var productCategory = fixture.CreateMany<ProductCategory>(10).ToList();
            context.Setup(c => c.Categories).ReturnsAsyncEntitySet(productCategory);
            var updateProductCategoryRequest = fixture.Create<Bll.UpdateProductCategoryRequest>();
            int id = productCategory[0].Id;

            var result = await service.UpdateCategoryAsync(id, updateProductCategoryRequest);

            result.Id.Should().Be(id);
            result.Code.Should().Be(updateProductCategoryRequest.Code);
            result.Name.Should().Be(updateProductCategoryRequest.Name);
        }

        [Theory]
        [AutoMoqData]
        public async void UpdateProductCategoryAsync_EntityWithExistedCode_CustomExceptionThrows([Frozen] Mock<IProductCatalogueContext> context, Bll.ProductCategoryService service, IFixture fixture)
        {
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var productCategory = fixture.CreateMany<ProductCategory>(10).ToList();
            context.Setup(c => c.Categories).ReturnsAsyncEntitySet(productCategory);
            var updateProductCategoryRequest = fixture.Create<Bll.UpdateProductCategoryRequest>();
            updateProductCategoryRequest.Code = productCategory[0].Code;
            int id = productCategory[1].Id;

            await Assert.ThrowsAsync<RequestedResourceHasConflictException>(() => service.UpdateCategoryAsync(id, updateProductCategoryRequest));
        }

        [Theory]
        [AutoMoqData]
        public async void UpdateProductCategoryAsync_NotExistedEntityIdentifier_CustomExceptionThrows([Frozen] Mock<IProductCatalogueContext> context, Bll.ProductCategoryService service, IFixture fixture)
        {
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var productCategory = fixture.CreateMany<ProductCategory>(10).ToList();
            context.Setup(c => c.Categories).ReturnsAsyncEntitySet(productCategory);
            var updateProductCategoryRequest = fixture.Create<Bll.UpdateProductCategoryRequest>();
            int id = 0;

            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.UpdateCategoryAsync(id, updateProductCategoryRequest));
        }

        [Theory]
        [AutoMoqData]
        public async void DeleteCategoryAsync_ExistedIdentifierFlagIsDeletedTrue_SuccessfulDeleting([Frozen] Mock<IProductCatalogueContext> context, Bll.ProductCategoryService service, IFixture fixture)
        {
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var productCategory = fixture.CreateMany<ProductCategory>(10).ToList();
            context.Setup(c => c.Categories).ReturnsAsyncEntitySet(productCategory);
            var id = productCategory[0].Id;
            productCategory[0].IsDeleted = true;

            await service.DeleteCategoryAsync(id);

            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.GetCategoryAsync(id));
        }

        [Theory]
        [AutoMoqData]
        public async void DeleteProductCategoryAsync_NotExistedIdentifierFlagIsDeletedTrue_CustomExceptionThrows([Frozen] Mock<IProductCatalogueContext> context, Bll.ProductCategoryService service, IFixture fixture)
        {
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var productCategory = fixture.CreateMany<ProductCategory>(10).ToList();
            context.Setup(c => c.Categories).ReturnsAsyncEntitySet(productCategory);
            var id = 0;
            productCategory[0].IsDeleted = true;

            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.DeleteCategoryAsync(id));
        }

        [Theory]
        [AutoMoqData]
        public async void DeleteProductCategoryAsync_ExistedIdentifierFlagIsDeletedFalse_CustomExceptionThrows([Frozen] Mock<IProductCatalogueContext> context, Bll.ProductCategoryService service, IFixture fixture)
        {
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var productCategory = fixture.CreateMany<ProductCategory>(10).ToList();
            context.Setup(c => c.Categories).ReturnsAsyncEntitySet(productCategory);
            var id = productCategory[0].Id;
            productCategory[0].IsDeleted = false;

            await Assert.ThrowsAsync<RequestedResourceHasConflictException>(() => service.DeleteCategoryAsync(id));
        }

        [Theory]
        [AutoMoqData]
        public async void SetStatusAsync_EntityHasFlagIsDeletedTrueSetFalse_SuccsessfulChange([Frozen] Mock<IProductCatalogueContext> context, Bll.ProductCategoryService service, IFixture fixture)
        {
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var productCategory = fixture.CreateMany<ProductCategory>(10).ToList();
            context.Setup(c => c.Categories).ReturnsAsyncEntitySet(productCategory);
            productCategory[0].IsDeleted = true;

            await service.SetStatusAsync(productCategory[0].Id, false);

            productCategory[0].IsDeleted.Should().Be(false);
        }

        [Theory]
        [AutoMoqData]
        public async void SetStatusAsync_EntityHasFlagIsDeletedFalseSetTrue_SuccsessfulChange([Frozen] Mock<IProductCatalogueContext> context, Bll.ProductCategoryService service, IFixture fixture)
        {
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var productCategory = fixture.CreateMany<ProductCategory>(10).ToList();
            context.Setup(c => c.Categories).ReturnsAsyncEntitySet(productCategory);
            productCategory[0].IsDeleted = false;

            await service.SetStatusAsync(productCategory[0].Id, true);

            productCategory[0].IsDeleted.Should().Be(true);
        }

        [Theory]
        [AutoMoqData]
        public async void SetStatusAsync_NotExistedEntityIdentifier_CustomExceptionThrows([Frozen] Mock<IProductCatalogueContext> context, Bll.ProductCategoryService service, IFixture fixture)
        {
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var productCategory = fixture.CreateMany<ProductCategory>(10).ToList();
            context.Setup(c => c.Categories).ReturnsAsyncEntitySet(productCategory);
            var id = 0;

            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.SetStatusAsync(id, false));
        }
    }
}