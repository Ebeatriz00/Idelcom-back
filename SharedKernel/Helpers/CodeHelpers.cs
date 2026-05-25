using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Helpers
{
    public class CodeHelpers
    {
        public static string GenerateCode(string code)
        {
            // Si el código es nulo o vacío, empezamos desde "001"
            if (string.IsNullOrEmpty(code))
            {
                return "01";
            }

            // Intentamos convertir el código en número
            if (int.TryParse(code, out int numero))
            {
                // Incrementamos y retornamos en formato de 3 dígitos
                return (numero + 1).ToString("D2");
            }

            // Si el código no es numérico, devolvemos "001" como fallback
            return "00";
        }
    }
}
