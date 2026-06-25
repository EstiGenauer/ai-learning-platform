using AuthService.Services;

namespace AuthService.Tests
{
    public class PasswordServiceTests
    {
        private readonly PasswordService _service = new();

        [Fact]
        public void Hash_ProducesVerifiableHash()
        {
            var hash = _service.Hash("Password123!");
            Assert.False(string.IsNullOrWhiteSpace(hash));
            Assert.NotEqual("Password123!", hash);
            Assert.True(_service.Verify("Password123!", hash));
        }

        [Fact]
        public void Verify_WithWrongPassword_ReturnsFalse()
        {
            var hash = _service.Hash("Password123!");
            Assert.False(_service.Verify("wrong", hash));
        }
    }
}
