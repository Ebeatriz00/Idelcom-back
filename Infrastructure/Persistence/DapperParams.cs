using Dapper;
using System.Data;
using System.Reflection;

namespace Infrastructure.Persistence
{
    /// <summary>
    /// Representa un contenedor fluido de parámetros para Dapper que permite
    /// combinar valores simples, parámetros con metadatos explícitos y salidas de procedimientos almacenados.
    /// </summary>
    public class DapperParams : DynamicParameters
    {
        /// <summary>
        /// Inicializa una nueva instancia vacía de <see cref="DapperParams"/>.
        /// </summary>
        public DapperParams()
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="DapperParams"/> a partir de un objeto base.
        /// Las propiedades del objeto se agregan como parámetros de entrada y, si alguna propiedad
        /// contiene un wrapper <see cref="DbParamValue"/>, se respetan sus metadatos explícitos.
        /// </summary>
        /// <param name="template">
        /// Objeto base cuyas propiedades se convertirán en parámetros. Puede ser nulo.
        /// </param>
        public DapperParams(object? template)
        {
            AddTemplate(template);
        }

        /// <summary>
        /// Crea una instancia de <see cref="DapperParams"/> a partir de un objeto base.
        /// </summary>
        /// <param name="template">
        /// Objeto cuyas propiedades se tomarán como parámetros iniciales. Puede incluir wrappers <see cref="DbParamValue"/>.
        /// </param>
        /// <returns>Una instancia de <see cref="DapperParams"/> configurada con los parámetros del objeto recibido.</returns>
        public static DapperParams From(object? template = null) => new(template);

        /// <summary>
        /// Agrega un parámetro de entrada usando únicamente nombre y valor.
        /// </summary>
        /// <param name="name">Nombre del parámetro que se enviará al comando o procedimiento almacenado.</param>
        /// <param name="value">Valor del parámetro. Puede ser nulo.</param>
        /// <returns>La misma instancia actual para permitir encadenamiento fluido.</returns>
        public DapperParams WithInput(string name, object? value)
        {
            Add(name, value);
            return this;
        }

        /// <summary>
        /// Agrega un parámetro de entrada con tipo de dato explícito y tamaño opcional.
        /// </summary>
        /// <param name="name">Nombre del parámetro que se enviará al comando o procedimiento almacenado.</param>
        /// <param name="value">Valor del parámetro. Puede ser nulo.</param>
        /// <param name="dbType">Tipo de dato ADO.NET que debe usarse al enviar el parámetro.</param>
        /// <param name="size">Tamaño del parámetro cuando aplique, por ejemplo en cadenas.</param>
        /// <returns>La misma instancia actual para permitir encadenamiento fluido.</returns>
        public DapperParams WithInput(string name, object? value, DbType dbType, int? size = null)
        {
            Add(name, value, dbType: dbType, size: size);
            return this;
        }

        /// <summary>
        /// Agrega un parámetro de salida entero de 32 bits.
        /// </summary>
        /// <param name="name">Nombre del parámetro de salida definido en el procedimiento almacenado.</param>
        /// <returns>La misma instancia actual para permitir encadenamiento fluido.</returns>
        public DapperParams WithOutputInt(string name) => WithOutput(name, DbType.Int32);

        /// <summary>
        /// Agrega un parámetro de salida entero de 64 bits.
        /// </summary>
        /// <param name="name">Nombre del parámetro de salida definido en el procedimiento almacenado.</param>
        /// <returns>La misma instancia actual para permitir encadenamiento fluido.</returns>
        public DapperParams WithOutputLong(string name) => WithOutput(name, DbType.Int64);

        /// <summary>
        /// Agrega un parámetro de salida de texto con tamaño explícito.
        /// </summary>
        /// <param name="name">Nombre del parámetro de salida definido en el procedimiento almacenado.</param>
        /// <param name="size">Longitud máxima esperada para el valor retornado.</param>
        /// <returns>La misma instancia actual para permitir encadenamiento fluido.</returns>
        public DapperParams WithOutputString(string name, int size) => WithOutput(name, DbType.String, size);

        /// <summary>
        /// Agrega un parámetro de salida booleano.
        /// </summary>
        /// <param name="name">Nombre del parámetro de salida definido en el procedimiento almacenado.</param>
        /// <returns>La misma instancia actual para permitir encadenamiento fluido.</returns>
        public DapperParams WithOutputBool(string name) => WithOutput(name, DbType.Boolean);

        /// <summary>
        /// Agrega un parámetro de salida decimal.
        /// </summary>
        /// <param name="name">Nombre del parámetro de salida definido en el procedimiento almacenado.</param>
        /// <returns>La misma instancia actual para permitir encadenamiento fluido.</returns>
        public DapperParams WithOutputDecimal(string name) => WithOutput(name, DbType.Decimal);

        /// <summary>
        /// Agrega un parámetro de salida de fecha y hora.
        /// </summary>
        /// <param name="name">Nombre del parámetro de salida definido en el procedimiento almacenado.</param>
        /// <returns>La misma instancia actual para permitir encadenamiento fluido.</returns>
        public DapperParams WithOutputDateTime(string name) => WithOutput(name, DbType.DateTime);

        /// <summary>
        /// Agrega un parámetro estructurado basado en un <see cref="DataTable"/> para enviarlo como TVP.
        /// </summary>
        /// <param name="name">Nombre del parámetro estructurado definido en el procedimiento almacenado.</param>
        /// <param name="value">Tabla en memoria con los datos que se enviarán al TVP.</param>
        /// <param name="typeName">Nombre del tipo tabla definido en SQL Server.</param>
        /// <returns>La misma instancia actual para permitir encadenamiento fluido.</returns>
        public DapperParams WithTable(string name, DataTable value, string typeName)
        {
            Add(name, value.AsTableValuedParameter(typeName));
            return this;
        }

        /// <summary>
        /// Agrega un parámetro manual con control total sobre valor, tipo, dirección y tamaño.
        /// </summary>
        /// <param name="name">Nombre del parámetro que se enviará al comando o procedimiento almacenado.</param>
        /// <param name="value">Valor del parámetro. Puede ser nulo.</param>
        /// <param name="dbType">Tipo de dato ADO.NET a utilizar. Puede omitirse cuando no se requiera control explícito.</param>
        /// <param name="direction">Dirección del parámetro, por ejemplo entrada o salida.</param>
        /// <param name="size">Tamaño del parámetro cuando aplique.</param>
        /// <returns>La misma instancia actual para permitir encadenamiento fluido.</returns>
        public DapperParams WithParameter(
            string name,
            object? value = null,
            DbType? dbType = null,
            ParameterDirection direction = ParameterDirection.Input,
            int? size = null)
        {
            Add(name, value, dbType: dbType, direction: direction, size: size);
            return this;
        }

        /// <summary>
        /// Recorre las propiedades de un objeto y las agrega como parámetros de entrada.
        /// Si una propiedad contiene un wrapper <see cref="DbParamValue"/>, se aplican los metadatos declarados en dicho wrapper.
        /// </summary>
        /// <param name="template">Objeto que contiene las propiedades a transformar en parámetros.</param>
        private void AddTemplate(object? template)
        {
            if (template is null)
            {
                return;
            }

            foreach (var property in template.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var name = property.Name;
                var value = property.GetValue(template);

                if (value is DbParamValue wrapped)
                {
                    AddWrapped(name, wrapped);
                    continue;
                }

                Add(name, value);
            }
        }

        /// <summary>
        /// Agrega un parámetro previamente envuelto con metadatos explícitos.
        /// </summary>
        /// <param name="name">Nombre de la propiedad del objeto origen, usado como nombre del parámetro.</param>
        /// <param name="wrapped">Wrapper que contiene el valor y la configuración adicional del parámetro.</param>
        private void AddWrapped(string name, DbParamValue wrapped)
        {
            if (wrapped.IsTableValued)
            {
                if (wrapped.Value is not DataTable table)
                {
                    throw new InvalidOperationException($"El parámetro '{name}' requiere un DataTable para enviarse como TVP.");
                }

                Add(name, table.AsTableValuedParameter(wrapped.TableTypeName!));
                return;
            }

            Add(
                name,
                wrapped.Value,
                dbType: wrapped.DbType,
                direction: wrapped.Direction,
                size: wrapped.Size);
        }

        /// <summary>
        /// Agrega un parámetro de salida usando el tipo ADO.NET indicado.
        /// </summary>
        /// <param name="name">Nombre del parámetro de salida definido en el procedimiento almacenado.</param>
        /// <param name="dbType">Tipo de dato ADO.NET que se usará para recuperar el valor de salida.</param>
        /// <param name="size">Tamaño del parámetro cuando aplique, por ejemplo en cadenas.</param>
        /// <returns>La misma instancia actual para permitir encadenamiento fluido.</returns>
        private DapperParams WithOutput(string name, DbType dbType, int? size = null)
        {
            Add(name, dbType: dbType, direction: ParameterDirection.Output, size: size);
            return this;
        }
    }
}
