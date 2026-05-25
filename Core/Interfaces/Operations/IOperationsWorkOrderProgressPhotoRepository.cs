using SharedKernel;
using System;
using System.Threading.Tasks;

namespace Core.Interfaces.Operations
{
    public interface IOperationsWorkOrderProgressPhotoRepository
    {
        /// <summary>
        /// Registra la vinculación entre un avance de obra y una foto en la tabla puente.
        /// </summary>
        /// <param name="progressId">ID del avance de obra.</param>
        /// <param name="fileUid">GUID de la foto (SYS_FILES).</param>
        /// <param name="createdBy">ID del usuario que realiza el registro.</param>
        /// <returns>Respuesta con el estado de la operación.</returns>
        Task<BaseResponse> InsertPhotoAsync(long progressId, Guid fileUid, long createdBy);

        /// <summary>
        /// Obtiene los UIDs de las fotos asociadas a un avance de obra.
        /// </summary>
        /// <param name="progressId">ID del avance de obra.</param>
        /// <returns>Colección de GUIDs de los archivos.</returns>
        Task<IEnumerable<Guid>> GetPhotosByProgressIdAsync(long progressId);
    }
}
