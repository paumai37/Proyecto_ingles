using System;
using System.Security.Cryptography;

namespace ProyectoEnglish
{
    public static class PasswordHasher
    {
        private const int SaltSize = 16;      // 128 bits
        private const int KeySize = 32;       // 256 bits
        private const int Iterations = 100000;

        // Se usa al CREAR o CAMBIAR una contraseña. Devuelve "iteraciones.salt.hash"
        public static string Hash(string password)
        {
            byte[] salt = new byte[SaltSize];
            using (var rng = new RNGCryptoServiceProvider())
                rng.GetBytes(salt);

            byte[] key;
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
                key = pbkdf2.GetBytes(KeySize);

            return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";
        }

        // Se usa en el LOGIN: hashea lo escrito con el salt guardado y compara
        public static bool Verify(string password, string stored)
        {
            if (!IsHashed(stored)) return false;

            var parts = stored.Split('.');
            int iterations = int.Parse(parts[0]);
            byte[] salt = Convert.FromBase64String(parts[1]);
            byte[] expected = Convert.FromBase64String(parts[2]);

            byte[] actual;
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
                actual = pbkdf2.GetBytes(expected.Length);

            return FixedTimeEquals(actual, expected);
        }

        // ¿El texto guardado tiene formato de hash? (sirve para detectar contraseñas antiguas en texto plano)
        public static bool IsHashed(string stored)
        {
            if (string.IsNullOrEmpty(stored)) return false;
            var parts = stored.Split('.');
            int n;
            return parts.Length == 3 && int.TryParse(parts[0], out n);
        }

        // Comparación en tiempo constante (evita timing attacks)
        private static bool FixedTimeEquals(byte[] a, byte[] b)
        {
            if (a.Length != b.Length) return false;
            int diff = 0;
            for (int i = 0; i < a.Length; i++) diff |= a[i] ^ b[i];
            return diff == 0;
        }
    }
}