using System;
using System.Runtime.InteropServices;
using System.Security;

namespace ImageMaker.Common.Helpers
{
    public interface IHashBuilder
    {
        string HashPassword(string password);
        bool ValidatePassword(string password, string correctHash);
    }

    public class HashBuilder : IHashBuilder
    {
        private string GetRandomSalt()
        {
            return BCrypt.Net.BCrypt.GenerateSalt(12);
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, GetRandomSalt());
        }

        public bool ValidatePassword(string password, string correctHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, correctHash);
        }
    }
}
