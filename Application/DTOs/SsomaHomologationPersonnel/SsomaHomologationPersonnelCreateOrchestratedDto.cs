using Application.DTOs.SsomaHomologationPersonnelDocument;

namespace Application.DTOs.SsomaHomologationPersonnel
{
    public class SsomaHomologationPersonnelCreateOrchestratedDto
    {
        public SsomaHomologationPersonnelCreateDto HomologationPersonnel { get; set; } = new();
        public List<SsomaHomologationPersonnelDocumentCreateDto> Documents { get; set; } = new();
    }
}
