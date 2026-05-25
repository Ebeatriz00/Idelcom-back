using Application.DTOs.CostCenters;
using Application.DTOs.Exercises;
using Application.UseCases.CostCenters;
using Application.UseCases.Exercises;
using Microsoft.AspNetCore.Mvc;

namespace GlueMark.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExercisesController : ControllerBase
    {
        private readonly CreateExercises _createExercises;
        private readonly GetAllExercises _getAllExercises;
        private readonly GetByIdExercises _getByIdExercises;
        private readonly UpdateExercises _updateExercises;
        private readonly PatchExercisesStatus _patchExercisesStatus;
        private readonly GetSelectExercises _getSelectExercises;
        private readonly ToggleBlockExercises _toggleBlockExercises;

        public ExercisesController(
            CreateExercises createExercises,
            GetAllExercises getAllExercises,
            GetByIdExercises getByIdExercises,
            UpdateExercises updateExercises,
            PatchExercisesStatus patchExercisesStatus,
            GetSelectExercises getSelectExercises,
            ToggleBlockExercises toggleBlockExercises)
        {
            _createExercises = createExercises;
            _getAllExercises = getAllExercises;
            _getByIdExercises = getByIdExercises;
            _updateExercises = updateExercises;
            _patchExercisesStatus = patchExercisesStatus;
            _getSelectExercises = getSelectExercises;
            _toggleBlockExercises = toggleBlockExercises;
        }

        [HttpPost]
        [Route("ExercisesCreate")]
        public async Task<IActionResult> Create([FromBody] ExercisesCreateDto dto)
        {
            var result = await _createExercises.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpGet]
        [Route("ExercisesList")]
        public async Task<IActionResult> GetList([FromQuery] long business_id, [FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] long? usersBy = null)
        {
            var result = await _getAllExercises.ExecuteAsync(business_id, search, page, pageSize, usersBy);
            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron ejercicios." });

            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();

            return Ok(result);
        }

        [HttpGet]
        [Route("ExercisesSelect")]
        public async Task<IActionResult> GetSelect(
            [FromQuery] long business_id,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _getSelectExercises.ExecuteAsync(business_id, search, page, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("ExercisesById")]
        public async Task<IActionResult> GetById([FromQuery] long exercisesId)
        {
            var result = await _getByIdExercises.ExecuteAsync(exercisesId);
            if (result == null)
                return NotFound(new { message = "No se encontró el ejercicio." });

            return Ok(result);
        }

        [HttpPut]
        [Route("ExercisesUpdate")]
        public async Task<IActionResult> Update([FromBody] ExercisesUpdateDto dto)
        {
            var result = await _updateExercises.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpPatch]
        [Route("ExercisesStatus")]
        public async Task<IActionResult> PatchStatus([FromBody] ExercisesStatusToggleDto dto)
        {
            var result = await _patchExercisesStatus.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpPatch]
        [Route("ExercisesBlock")]
        public async Task<IActionResult> PatchBlock([FromBody] ExercisesBlockToggleDto dto)
        {
            var result = await _toggleBlockExercises.ExecuteAsync(dto);
            return Ok(result);
        }
    }
}
