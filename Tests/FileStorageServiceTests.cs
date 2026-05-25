using Core.Interfaces;
using Core.Options;
using Infrastructure.ExternalServices;
using Microsoft.Extensions.Options;
using Moq;

namespace Tests
{
    public class FileStorageServiceTests
    {
        private readonly Mock<IFileRepository> _fileRepositoryMock;
        private readonly Mock<IOptions<StorageOptions>> _optionsMock;
        private readonly FileStorageService _service;

        public FileStorageServiceTests()
        {
            _fileRepositoryMock = new Mock<IFileRepository>();
            _optionsMock = new Mock<IOptions<StorageOptions>>();
            _optionsMock.Setup(o => o.Value).Returns(new StorageOptions
            {
                RootPath = Path.Combine(Path.GetTempPath(), "IdelcomTest"),
                MaxFileSize = 1024 * 1024
            });

            _service = new FileStorageService(_fileRepositoryMock.Object, _optionsMock.Object);
        }
    }
}
