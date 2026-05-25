using System.Data;

namespace Infrastructure.Persistence
{
    /// <summary>
    /// Provee métodos de fábrica para envolver valores que requieren metadatos explícitos
    /// al transformarse en parámetros de Dapper desde objetos anónimos.
    /// </summary>
    public static class DbParam
    {
        /// <summary>
        /// Crea un wrapper genérico para un parámetro de entrada con tipo ADO.NET explícito.
        /// </summary>
        /// <param name="value">Valor real del parámetro.</param>
        /// <param name="dbType">Tipo ADO.NET que debe usarse al enviar el valor.</param>
        /// <param name="size">Tamaño del parámetro cuando aplique, por ejemplo en cadenas.</param>
        /// <returns>Un wrapper con los metadatos necesarios para crear el parámetro.</returns>
        public static DbParamValue Raw(object? value, DbType dbType, int? size = null) =>
            new(value, dbType, ParameterDirection.Input, size);

        /// <summary>
        /// Crea un wrapper para enviar un valor como tipo SQL <c>time</c>.
        /// </summary>
        /// <param name="value">Valor horario que se enviará al procedimiento almacenado.</param>
        /// <returns>Un wrapper configurado para enviarse como <see cref="DbType.Time"/>.</returns>
        public static DbParamValue Time(object? value) => Raw(value, DbType.Time);

        /// <summary>
        /// Crea un wrapper para enviar un valor como tipo SQL <c>date</c>.
        /// </summary>
        /// <param name="value">Valor de fecha que se enviará al procedimiento almacenado.</param>
        /// <returns>Un wrapper configurado para enviarse como <see cref="DbType.Date"/>.</returns>
        public static DbParamValue Date(object? value) => Raw(value, DbType.Date);

        /// <summary>
        /// Crea un wrapper para enviar un valor como tipo SQL <c>datetime</c>.
        /// </summary>
        /// <param name="value">Valor de fecha y hora que se enviará al procedimiento almacenado.</param>
        /// <returns>Un wrapper configurado para enviarse como <see cref="DbType.DateTime"/>.</returns>
        public static DbParamValue DateTime(object? value) => Raw(value, DbType.DateTime);

        /// <summary>
        /// Crea un wrapper para enviar un valor decimal con tipo explícito.
        /// </summary>
        /// <param name="value">Valor decimal que se enviará al procedimiento almacenado.</param>
        /// <returns>Un wrapper configurado para enviarse como <see cref="DbType.Decimal"/>.</returns>
        public static DbParamValue Decimal(object? value) => Raw(value, DbType.Decimal);

        /// <summary>
        /// Crea un wrapper para enviar un valor booleano con tipo explícito.
        /// </summary>
        /// <param name="value">Valor booleano que se enviará al procedimiento almacenado.</param>
        /// <returns>Un wrapper configurado para enviarse como <see cref="DbType.Boolean"/>.</returns>
        public static DbParamValue Bool(object? value) => Raw(value, DbType.Boolean);

        /// <summary>
        /// Crea un wrapper para enviar una cadena con tamaño explícito.
        /// </summary>
        /// <param name="value">Texto que se enviará al procedimiento almacenado.</param>
        /// <param name="size">Longitud máxima del parámetro.</param>
        /// <returns>Un wrapper configurado para enviarse como <see cref="DbType.String"/>.</returns>
        public static DbParamValue String(string? value, int size) => Raw(value, DbType.String, size);

        /// <summary>
        /// Crea un wrapper para enviar un <see cref="DataTable"/> como parámetro estructurado (TVP).
        /// </summary>
        /// <param name="value">Tabla en memoria con los registros que se enviarán al procedimiento almacenado.</param>
        /// <param name="typeName">Nombre del tipo tabla definido en SQL Server.</param>
        /// <returns>Un wrapper configurado como parámetro estructurado.</returns>
        public static DbParamValue Table(DataTable value, string typeName) => DbParamValue.ForTable(value, typeName);
    }

    /// <summary>
    /// Encapsula un valor junto con los metadatos requeridos para crear un parámetro de Dapper
    /// cuando se usa dentro de un objeto anónimo enviado a <see cref="DapperParams.From(object?)"/>.
    /// </summary>
    public sealed class DbParamValue
    {
        /// <summary>
        /// Inicializa una nueva instancia de <see cref="DbParamValue"/> para parámetros simples.
        /// </summary>
        /// <param name="value">Valor del parámetro.</param>
        /// <param name="dbType">Tipo ADO.NET del parámetro.</param>
        /// <param name="direction">Dirección del parámetro.</param>
        /// <param name="size">Tamaño del parámetro cuando aplique.</param>
        internal DbParamValue(object? value, DbType? dbType, ParameterDirection direction, int? size = null)
        {
            Value = value;
            DbType = dbType;
            Direction = direction;
            Size = size;
        }

        /// <summary>
        /// Obtiene el valor real del parámetro.
        /// </summary>
        public object? Value { get; }

        /// <summary>
        /// Obtiene el tipo ADO.NET configurado para el parámetro.
        /// </summary>
        public DbType? DbType { get; }

        /// <summary>
        /// Obtiene la dirección del parámetro.
        /// </summary>
        public ParameterDirection Direction { get; }

        /// <summary>
        /// Obtiene el tamaño configurado para el parámetro cuando aplica.
        /// </summary>
        public int? Size { get; }

        /// <summary>
        /// Indica si el wrapper representa un parámetro estructurado (TVP).
        /// </summary>
        public bool IsTableValued => TableTypeName is not null;

        /// <summary>
        /// Obtiene el nombre del tipo tabla en SQL Server cuando el wrapper representa un TVP.
        /// </summary>
        public string? TableTypeName { get; private init; }

        /// <summary>
        /// Crea un wrapper para un <see cref="DataTable"/> que será enviado como TVP.
        /// </summary>
        /// <param name="value">Tabla en memoria con los datos a enviar.</param>
        /// <param name="typeName">Nombre del tipo tabla definido en SQL Server.</param>
        /// <returns>Un wrapper configurado como parámetro estructurado.</returns>
        internal static DbParamValue ForTable(DataTable value, string typeName) =>
            new(value, dbType: null, ParameterDirection.Input)
            {
                TableTypeName = typeName
            };
    }
}
