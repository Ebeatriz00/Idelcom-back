using Application.Common.Validations;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace Tests
{
    public class FileValidationExtensionsTests
    {
        private class TestDto { public IFormFile? File { get; set; } }

        private class TestValidator : AbstractValidator<TestDto>
        {
            public TestValidator()
            {
                RuleFor(x => x.File)
                    .MustBeValidImage()
                    .MaxSizeInBytes(1024); // 1KB limit for testing
            }
        }

        [Fact]
        public void Should_Fail_When_File_Is_Not_Image()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            var content = "esto no es una imagen";
            var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));
            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
            fileMock.Setup(f => f.Length).Returns(stream.Length);

            var validator = new TestValidator();

            // Act
            var result = validator.Validate(new TestDto { File = fileMock.Object });

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("no es una imagen permitida", result.Errors[0].ErrorMessage);
        }

        [Fact]
        public void Should_Fail_When_File_Exceeds_Size()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            var content = new byte[2048]; // 2KB, limit is 1KB
            var stream = new MemoryStream(content);
            // Simular una imagen JPG para pasar la primera validación
            content[0] = 0xFF; content[1] = 0xD8; content[2] = 0xFF;

            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
            fileMock.Setup(f => f.Length).Returns(stream.Length);

            var validator = new TestValidator();

            // Act
            var result = validator.Validate(new TestDto { File = fileMock.Object });

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("excede el tamaño máximo", result.Errors[0].ErrorMessage);
        }

        [Fact]
        public void Should_Succeed_When_File_Is_Valid_Image()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            var content = new byte[8];
            content[0] = 0x89; content[1] = 0x50; content[2] = 0x4E; content[3] = 0x47; // PNG
            var stream = new MemoryStream(content);

            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
            fileMock.Setup(f => f.Length).Returns(stream.Length);

            var validator = new TestValidator();

            // Act
            var result = validator.Validate(new TestDto { File = fileMock.Object });

            // Assert
            Assert.True(result.IsValid);
        }
    }
}
