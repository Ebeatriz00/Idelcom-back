using Application.DTOs.Operations.OperationsWorkOrderProgress;
using Core.Interfaces;
using Core.Interfaces.Operations;

namespace Application.UseCases.Operations.OperationsWorkOrderProgress
{
    public class GetAppOperationsWorkOrderProgressPhotos(
        IOperationsWorkOrderProgressPhotoRepository repository,
        IFileUrlBuilder fileUrlBuilder)
    {
        private readonly IOperationsWorkOrderProgressPhotoRepository _repository = repository;
        private readonly IFileUrlBuilder _fileUrlBuilder = fileUrlBuilder;

        public async Task<IEnumerable<OperationsWorkOrderProgressPhotoDto>> ExecuteAsync(long progressId)
        {
            var photoUids = await _repository.GetPhotosByProgressIdAsync(progressId);

            return photoUids.Select(uid => new OperationsWorkOrderProgressPhotoDto
            {
                FileUid = uid,
                Url = _fileUrlBuilder.BuildFileUrl(uid)
            });
        }
    }
}
