using LearningPlatformApi.Data;

namespace LearningPlatformApi.Tests.Data
{
    public class CategoriesSeedDataTests
    {
        [Fact]
        public void Categories_ContainsExpectedCoreTopics()
        {
            Assert.True(CategoriesSeedData.Categories.ContainsKey("Databases"));
            Assert.True(CategoriesSeedData.Categories.ContainsKey("Cyber Security"));
            Assert.True(CategoriesSeedData.Categories.ContainsKey("Artificial Intelligence & Machine Learning"));
        }

        [Fact]
        public void Categories_AllHaveAtLeastOneSubCategory()
        {
            foreach (var (name, subs) in CategoriesSeedData.Categories)
            {
                Assert.NotEmpty(subs);
                Assert.All(subs, sub => Assert.False(string.IsNullOrWhiteSpace(sub)));
            }
        }

        [Fact]
        public void Categories_HasMinimumExpectedCount()
        {
            Assert.True(CategoriesSeedData.Categories.Count >= 40);
        }
    }
}
