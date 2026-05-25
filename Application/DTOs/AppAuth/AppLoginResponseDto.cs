using SharedKernel;

namespace Application.DTOs.AppAuth
{
    public class AppLoginResponseDto : BaseResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public long UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public long BusinessId { get; set; }

    }
}
