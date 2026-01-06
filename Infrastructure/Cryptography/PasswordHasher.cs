using Application.Common.Interfaces.Security;
using Konscious.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Cryptography
{
    public sealed class PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;

        private const int Iterations = 4;
        private const int MemorySize = 65536;
        private const int Parallelism = 8;

        public string Hash(string password)
        {
            var salt = GenerateSalt(SaltSize);
            var hash = ComputeHash(password, salt);

            return string.Join(
                '.',
                Iterations,
                MemorySize,
                Parallelism,
                Convert.ToBase64String(salt),
                Convert.ToBase64String(hash)
                );
        }
        public bool Verify(string passwordHashed, string password)
        {
            var parts = passwordHashed.Split('.');

            if (parts.Length != 5) return false;

            var iterations = int.Parse(parts[0]);
            var memory = int.Parse(parts[1]);
            var parallelism = int.Parse(parts[2]);
            var salt = Convert.FromBase64String(parts[3]);
            var expectedHash = Convert.FromBase64String(parts[4]);

            var currentHash = ComputeHash(
                password, salt, iterations,
                memory, parallelism
                );

            return CryptographicOperations.FixedTimeEquals
                (currentHash, expectedHash);
        
        }

        private static byte[] ComputeHash(
            string password,
            byte[] salt,
            int iterations = Iterations,
            int memorySize = MemorySize,
            int parallelism = Parallelism)
        {
            using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            { 
                Salt = salt,
                Iterations = iterations,
                MemorySize = memorySize,
                DegreeOfParallelism = parallelism
            
            };

            return argon2.GetBytes(HashSize);
        }


        private static byte[] GenerateSalt(int length)
        {
            var salt = new byte[length];
            RandomNumberGenerator.Fill(salt);
            return salt;
        }

    }
}
