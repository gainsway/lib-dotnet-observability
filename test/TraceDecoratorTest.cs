namespace Gainsway.Observability.Tests
{
    public interface ITestService
    {
        void DoWork();
        Task DoWorkAsync();
        Task<int> GetNumberAsync();
    }

    public class TestService : ITestService
    {
        public void DoWork()
        {
            // Simulate work
        }

        public async Task DoWorkAsync()
        {
            await Task.Delay(100); // Simulate async work
        }

        public async Task<int> GetNumberAsync()
        {
            await Task.Delay(100); // Simulate async work
            return 42;
        }
    }

    [TestFixture]
    public class TraceDecoratorTest
    {
        private ITestService _service;

        [SetUp]
        public void SetUp()
        {
            var decoratedService = new TestService();
            _service = TraceDecorator<ITestService>.Create(decoratedService);
        }

        [Test]
        public void DoWork_ShouldInvokeWithoutException()
        {
            Assert.DoesNotThrow(() => _service.DoWork());
        }

        [Test]
        public void DoWorkAsync_ShouldInvokeWithoutException()
        {
            Assert.DoesNotThrowAsync(async () => await _service.DoWorkAsync());
        }

        [Test]
        public async Task GetNumberAsync_ShouldReturnExpectedResult()
        {
            var result = await _service.GetNumberAsync();
            Assert.That(result, Is.EqualTo(42));
        }
    }
}
