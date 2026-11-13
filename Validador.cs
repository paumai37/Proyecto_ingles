using System;
using System.Text.RegularExpressions;

namespace ProyectoEnglish
{
    public static class Validador
    {
        private static readonly Regex RxDocumento =
            new Regex(@"^([0-9]{8}|[XYZxyz][0-9]{7})[A-Za-z]$", RegexOptions.Compiled);
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