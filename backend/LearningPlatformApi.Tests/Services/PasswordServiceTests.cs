using LearningPlatformApi.Services;

namespace LearningPlatformApi.Tests.Services
{
    public class PasswordServiceTests
    {
        private readonly PasswordService _service = new();

        [Fact]
        public void Hash_ReturnsDifferentValueFromPlainPassword()
        {
            var hash = _service.Hash("MySecurePassword123!");
            Assert.NotEqual("MySecurePassword123!", hash);
            Assert.NotEmpty(hash);
        }

        [Fact]
        public void Verify_ReturnsTrueForCorrectPassword()
        {
            const string password = "Admin123!";
            var hash = _service.Hash(password);
            Assert.True(_service.Verify(password, hash));
        }

        [Fact]
        public void Verify_ReturnsFalseForWrongPassword()
        {
            var hash = _service.Hash("correct-password");
            Assert.False(_service.Verify("wrong-password", hash));
        }
    }
}
