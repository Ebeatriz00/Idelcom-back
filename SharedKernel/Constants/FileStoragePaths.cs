namespace SharedKernel.Constants
{
    /// <summary>
    /// Define las rutas sugeridas para el almacenamiento de archivos en el servidor.
    /// Estas rutas se combinan con la estructura de fecha (yyyy/MM) en el servicio de almacenamiento.
    /// </summary>
    public static class FileStoragePaths
    {
        public const string OperationsWorkOrderProgress = "Operations/WorkOrders/Progress/Photos";
        public const string OperationsAttendanceWorkers = "Operations/Attendances/Workers/Photos";
        public const string OperationsAttendanceGroups = "Operations/Attendances/Groups/Photos";
    }
}
