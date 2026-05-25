namespace Core.Interfaces
{
    public interface IFileUrlBuilder
    {
        string BuildFileUrl(Guid fileUid);
        string BuildFileUrl(Guid? fileUid);
    }
}
