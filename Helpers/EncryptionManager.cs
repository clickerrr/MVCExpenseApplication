using BCrypt.Net;

namespace MVCBeginner.Helpers
{
    public static class EncryptionManager
    {
        public static (string hashedPassword, string salt) encryptPassword(String unencryptedPassword)
        {
            string salt = BCrypt.Net.BCrypt.GenerateSalt();
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(unencryptedPassword, salt);
            return(hashedPassword: hashedPassword,  salt: salt);
        }

        public static Boolean checkPassword(string encryptedPassword, string encryptedSalt, string passwordToCheck)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(passwordToCheck, encryptedSalt);
            return hashedPassword.Equals(encryptedPassword);


        }

    }
}
