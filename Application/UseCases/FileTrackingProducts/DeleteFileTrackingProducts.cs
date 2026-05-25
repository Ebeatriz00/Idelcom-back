using Application.DTOs.FileTrackingProducts;
using Core.Interfaces.Logistic;
using FluentValidation;
using Infrastructure.Persistence;
using SharedKernel;
using AppValidationException = Application.Exceptions.ValidationException;

namespace Application.UseCases.FileTrackingProducts
{
    public class DeleteProductFile
    {
        private readonly IFileTrackingProductsRepository _repository;
        private readonly IValidator<FileTrackingProductsDeleteDto> _validator;
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public DeleteProductFile(
            IFileTrackingProductsRepository repository,
            IValidator<FileTrackingProductsDeleteDto> validator,
            ISqlConnectionFactory sqlConnectionFactory)
        {
            _repository = repository;
            _validator = validator;
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task<BaseResponse> ExecuteAsync(FileTrackingProductsDeleteDto dto, long businessId)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errors = validation.Errors
                    .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                    .ToList();
                throw new AppValidationException(errors);
            }

            using var connection = _sqlConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var deleted = await _repository.DeleteAsync(
                    dto.FileTrackingProductsId,
                    dto.ProductsId,
                    businessId,
                    transaction);

                transaction.Commit();
                return deleted;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
