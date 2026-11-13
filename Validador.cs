using System;
using System.Text.RegularExpressions;

namespace ProyectoEnglish
{
    public static class Validador
    {
        // DNI (8 números + letra) o NIE (X/Y/Z + 7 números + letra)
        private static readonly Regex RxDocumento =
            new Regex(@"^([0-9]{8}|[XYZxyz][0-9]{7})[A-Za-z]$", RegexOptions.Compiled);

        // Opcional +prefijo y de 9 a 15 dígitos (tras quitar espacios y guiones)
        private static readonly Regex RxTelefono =
            new Regex(@"^\+?[0-9]{9,15}$", RegexOptions.Compiled);

        public static bool EsDocumento(string s)
        {
            return RxDocumento.IsMatch(s ?? "");
        }

        public static bool EsTelefono(string s)
        {
            string limpio = Regex.Replace(s ?? "", @"[\s\-]", "");
            return RxTelefono.IsMatch(limpio);
        }
    }
}