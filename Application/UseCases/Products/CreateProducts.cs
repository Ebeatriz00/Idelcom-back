using Application.DTOs.Products;
using Application.Exceptions;
using AutoMapper;
using Core.Interfaces.Audit;
using Core.Interfaces.Logistic;
using FluentValidation;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using SharedKernel;
using SharedKernel.Constants;
using System.Threading.Tasks;
using AppValidationException = Application.Exceptions.ValidationException;


namespace Application.UseCases.Products
{
    public class CreateProducts(
        IProductsRepository repository,
        IValidator<ProductsCreateDto> validator,
        IMapper mapper,
        ISqlConnectionFactory sqlConnectionFactory,
        IAuditService auditService,
        IAuditLogFactory auditLogFactory,
        IFileTrackingProductsRepository fileTrackingProductsRepository
        )
    {
        private readonly IProductsRepository _repository = repository;
        private readonly IValidator<ProductsCreateDto> _validator = validator;
        private readonly IMapper _mapper = mapper;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;
        private readonly IAuditService _auditService = auditService;
        private readonly IAuditLogFactory _auditLogFactory = auditLogFactory;
        private readonly IFileTrackingProductsRepository _fileTrackingProductsRepository = fileTrackingProductsRepository;

       
        public async Task<BaseResponseId> ExecuteAsync(ProductsCreateDto dto, long userId, long businessId)
        {
            var validation = await _validator.ValidateAsync(dto);

            if (!validation.IsValid)
            {
                var errores = validation.Errors.Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage)).ToList();
                throw new AppValidationException(errores);
            }

            if (await _repository.ExistsAsync(dto.Description, businessId))
                throw new DuplicateEntryException("El producto ya existe para este negocio.");

            using var connection = _sqlConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                var entity = _mapper.Map<Core.Entities.Logistic.Products>(dto);
                entity.CreateUser = userId;
                entity.BusinessId = businessId;
                entity.Files = GetProductFileUrls(dto.Files);

                var created = await _repository.AddAsync(entity, transaction);
                if (created.Id is null)
                    throw new DatabaseException("Error al registrar la categoría.", "No se obtuvo el id creado.");

                entity.ProductsId = (long)created.Id;
                entity.BusinessId = (long)businessId;

                foreach (var file in dto.Files.Where(file => !string.IsNullOrWhiteSpace(file.FileUrl)))
                {
                    var productFile = new Core.Entities.Logistic.FileTrackingProducts
                    {
                        BusinessId = businessId,
                        ProductsId = entity.ProductsId,
                        FileTitle = file.FileTitle ?? string.Empty,
                        FileUrl = file.FileUrl ?? string.Empty,
                        RelativePath = file.RelativePath ?? string.Empty,
                        CreateUser = userId
                    };

                    await _fileTrackingProductsRepository.AddAsync(productFile, transaction);
                }

                var auditLog = _auditLogFactory.Create(
                    businessId,
                    TableNames.Products,
                    (long)created.Id,
                    userId);

                await _auditService.RegisterCreateAsync(entity, auditLog, transaction);

                transaction.Commit();
                return created;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private static string GetProductFileUrls(IEnumerable<Application.DTOs.FileTrackingProducts.FileTrackingProductsCreateDto> files)
        {
            return string.Join(",", files
                .Select(file => file.FileUrl)
                .Where(fileUrl => !string.IsNullOrWhiteSpace(fileUrl))
                .Select(fileUrl => fileUrl!.Trim()));
        }
    }
}
