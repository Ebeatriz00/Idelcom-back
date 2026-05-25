//using Application.DTOs.DocumentType;
//using Application.Exceptions;
//using AutoMapper;
//using Core.Interfaces;
//using FluentValidation;
//using SharedKernel;
//using SharedKernel.Constants;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using static System.Runtime.InteropServices.JavaScript.JSType;
//using AppValidationException = Application.Exceptions.ValidationException;

//namespace Application.UseCases.DocumentType
//{
//    public class CreateDocumentType
//    {
//        private readonly IDocumentTypeRepository _repository;
//        private readonly IValidator<DocumentTypeCreateDto> _validator;
//        private readonly IMapper _mapper;


//        public CreateDocumentType(IDocumentTypeRepository repository, IValidator<DocumentTypeCreateDto> validator, IMapper mapper)
//        {
//            _repository = repository;
//            _validator = validator;
//            _mapper = mapper;
//        }

//        /// AGREGADO
//        public string GenerateAbrv(string description)
//        {
//            if (string.IsNullOrWhiteSpace(description))
//            {
//                return string.Empty;
//            }

//            var words = description.ToUpper().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

//            if (words.Length > 1)
//            {
//                var acronym = new System.Text.StringBuilder();
//                foreach (var word in words)
//                {
//                    acronym.Append(word[0]);
//                }
//                return acronym.ToString();
//            }
//            else if (words.Length == 1)
//            {
//                var word = words[0];
//                return word.Length > 3 ? word.Substring(0, 3) : word;
//            }

//            return string.Empty;
//        }



//        public async Task<GlobalResponse> ExecuteAsync(DocumentTypeCreateDto dto)
//        {
//            var validation = await _validator.ValidateAsync(dto);
//            if (!validation.IsValid)
//            {
//                var errores = validation.Errors
//                            .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
//                            .ToList();

//                 throw new AppValidationException(errores);


//            }
//            string generatedAbrv = GenerateAbrv(dto.Description);
//            var result = await _repository.CreateAsync(dto, generatedAbrv);


//            var yaExiste = await _repository.ExistsAsync(dto.Description, dto.CodeSunat, dto.BusinessId);
//            if (yaExiste)
//                throw new DuplicateEntryException("El tipo de documento ya existe para este negocio.");


//            var entity = _mapper.Map<Core.Entities.DocumentType>(dto);

//            await _repository.AddAsync(entity);

//            return new GlobalResponse
//            {
//                Status = 1,
//                Message = "Tipo de documento creado exitosamente.",

//            };



//        }
//    }
//}


using Application.DTOs.DocumentType;
using Application.Exceptions;
using AutoMapper;
using Core.Interfaces;
using FluentValidation;
using SharedKernel;
using SharedKernel.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using AppValidationException = Application.Exceptions.ValidationException;

namespace Application.UseCases.DocumentType
{
    public class CreateDocumentType
    {
        private readonly IDocumentTypeRepository _repository;
        private readonly IValidator<DocumentTypeCreateDto> _validator;
        private readonly IMapper _mapper;

        public CreateDocumentType(IDocumentTypeRepository repository, IValidator<DocumentTypeCreateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(DocumentTypeCreateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                                        .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                                        .ToList();
                throw new AppValidationException(errores);
            }

            var yaExiste = await _repository.ExistsAsync(dto.Description, dto.CodeSunat, dto.BusinessId);
            if (yaExiste)
                throw new DuplicateEntryException("El tipo de documento ya existe para este negocio.");

            var entity = _mapper.Map<Core.Entities.DocumentType>(dto);

            // --- INICIO DE LA LÓGICA AÑADIDA ---

            // 1. Se genera la abreviatura a partir de la descripción del DTO.
            entity.Abrv = GenerateAbrv(dto.Description);

            // --- FIN DE LA LÓGICA AÑADIDA ---

            await _repository.AddAsync(entity);

            return new GlobalResponse
            {
                Status = 1,
                Message = "Tipo de documento creado exitosamente.",
            };
        }

        // 👇 SE AÑADE ESTA FUNCIÓN PRIVADA DENTRO DE LA CLASE 👇
        private string GenerateAbrv(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                return string.Empty;
            }

            // Dividimos la descripción en palabras, eliminando espacios extra
            var words = description.ToUpper().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (words.Length > 1)
            {
                // Si hay más de una palabra, tomamos la primera letra de cada una (acrónimo)
                // Ejemplo: "DOCUMENTO NACIONAL DE IDENTIDAD" -> "DNI"
                var acronym = new StringBuilder();
                foreach (var word in words)
                {
                    acronym.Append(word[0]);
                }
                return acronym.ToString();
            }
            else if (words.Length == 1)
            {
                // Si es una sola palabra, tomamos las 3 primeras letras
                // Ejemplo: "PASAPORTE" -> "PAS"
                var word = words[0];
                return word.Length > 3 ? word.Substring(0, 3) : word;
            }

            return string.Empty;
        }
    }
}