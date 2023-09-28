using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Security.Cryptography;
using System.Text;

namespace FinDash.Services
{
    public class PasswordService
    {
        public string GenerateSalt()
        {
            byte[] saltBytes = new byte[16];
            RandomNumberGenerator.Fill(saltBytes);
            return Convert.ToBase64String(saltBytes);
        }

        public string HashPassword(string password, string salt)
        {
            var saltedPassword = password + salt;
            using (var sha256 = SHA256.Create())
            {
                var saltedPasswordBytes = Encoding.UTF8.GetBytes(saltedPassword);
                var hashBytes = sha256.ComputeHash(saltedPasswordBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }
        public bool VerifyPassword(string storedHash, string storedSalt, string passwordToVerify)
        {
            // Hash the incoming password with the stored salt
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: passwordToVerify,
                salt: Convert.FromBase64String(storedSalt),
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            // Compare the hashes
            return hashed == storedHash;
        }
    }


}
