using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using CatalogService.DTOs;

namespace CatalogService.Tests
{
    public class CatalogEndpointsTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public CatalogEndpointsTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        private HttpClient AdminClient()
        {
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestJwt.Admin());
            return client;
        }

        [Fact]
        public async Task GetCategories_ReturnsSeededCatalog()
        {
            await _factory.ResetDatabaseAsync();
            var client = _factory.CreateClient();

            var categories = await client.GetFromJsonAsync<List<CategoryDto>>("/api/Categories");
            Assert.NotNull(categories);
            Assert.NotEmpty(categories!);
            Assert.All(categories!, c => Assert.NotEmpty(c.SubCategories));
        }

        [Fact]
        public async Task InternalValidate_WithValidSelection_ReturnsNames()
        {
            await _factory.ResetDatabaseAsync();
            var client = _factory.CreateClient();
            var categories = (await client.GetFromJsonAsync<List<CategoryDto>>("/api/Categories"))!;
            var cat = categories.First();
            var sub = cat.SubCategories.First();

            var response = await client.GetAsync($"/internal/validate?categoryId={cat.Id}&subCategoryId={sub.Id}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var dto = await response.Content.ReadFromJsonAsync<ValidateSelectionDto>();
            Assert.Equal(cat.Name, dto!.CategoryName);
            Assert.Equal(sub.Name, dto.SubCategoryName);
        }

        [Fact]
        public async Task InternalValidate_WithInvalidSelection_ReturnsNotFound()
        {
            await _factory.ResetDatabaseAsync();
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/internal/validate?categoryId=99999&subCategoryId=88888");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task AdminCreateCategory_WithoutToken_ReturnsUnauthorized()
        {
            await _factory.ResetDatabaseAsync();
            var client = _factory.CreateClient();

            var response = await client.PostAsJsonAsync("/api/Admin/categories",
                new CreateCategoryRequest { Name = "Unauthorized Cat" });
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task AdminCreateCategory_AsNonAdmin_ReturnsForbidden()
        {
            await _factory.ResetDatabaseAsync();
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestJwt.User());

            var response = await client.PostAsJsonAsync("/api/Admin/categories",
                new CreateCategoryRequest { Name = "Forbidden Cat" });
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task AdminCreateCategory_AsAdmin_CreatesCategory()
        {
            await _factory.ResetDatabaseAsync();
            var client = AdminClient();

            var response = await client.PostAsJsonAsync("/api/Admin/categories",
                new CreateCategoryRequest { Name = "Brand New Category" });
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var created = await response.Content.ReadFromJsonAsync<CategoryDto>();
            Assert.Equal("Brand New Category", created!.Name);
            Assert.True(created.Id > 0);
        }

        [Fact]
        public async Task DeleteCategory_WhenInUse_IsBlocked()
        {
            await _factory.ResetDatabaseAsync();
            var client = AdminClient();
            var categories = (await client.GetFromJsonAsync<List<CategoryDto>>("/api/Categories"))!;
            var target = categories.First();

            _factory.Usage.CategoryInUse = true;

            var response = await client.DeleteAsync($"/api/Admin/categories/{target.Id}");
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task DeleteCategory_WhenNotInUse_Succeeds()
        {
            await _factory.ResetDatabaseAsync();
            var client = AdminClient();
            var categories = (await client.GetFromJsonAsync<List<CategoryDto>>("/api/Categories"))!;
            var target = categories.First();

            _factory.Usage.CategoryInUse = false;

            var response = await client.DeleteAsync($"/api/Admin/categories/{target.Id}");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
    }
}
