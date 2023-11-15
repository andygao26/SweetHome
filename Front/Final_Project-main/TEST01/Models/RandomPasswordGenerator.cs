using System.Text;
using System.Linq;
using System;
namespace FifthGroup_front.Models
{
    public class RandomPasswordGenerator
    {
        private static readonly Random random = new Random();
        private static readonly string characters =
       "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789~!@#$";
        public static string GeneratePassword(int length)
        {
            if (length < 8)
            {
                throw new ArgumentException("密碼長度必須大於8");
            }
            StringBuilder password = new StringBuilder(length);
        

            password.Append(GetRandomCharacter("ABCDEFGHIJKLMNOPQRSTUVWXYZ")); // 大寫字母


            password.Append(GetRandomCharacter("abcdefghijklmnopqrstuvwxyz")); // 小寫字母
            password.Append(GetRandomCharacter("0123456789")); // 數字
            password.Append(GetRandomCharacter("~!@#$")); // 特殊字符
                                                          // 剩餘的字符隨機生成
            for (int i = 4; i < length; i++)
            {
                password.Append(GetRandomCharacter(characters));
            }
            // 密碼字符隨機排序
            string shuffledPassword = new
           string(password.ToString().ToCharArray().OrderBy(x =>
           random.Next()).ToArray());
            return shuffledPassword;
        }
        private static char GetRandomCharacter(string characterSet)
        {
            int index = random.Next(characterSet.Length);
            return characterSet[index];
        }

    }
}
