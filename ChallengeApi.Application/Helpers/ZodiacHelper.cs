using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallengeApi.Application.Helpers
{
    public static class ZodiacHelper
    {
        public static string GetSigno(DateOnly birthDate)
        {
            int day = birthDate.Day;
            int month = birthDate.Month;

            return month switch
            {
                1 => day <= 19 ? "Capricorn" : "Aquarius",
                2 => day <= 18 ? "Aquarius" : "Pisces",
                3 => day <= 20 ? "Pisces" : "Aries",
                4 => day <= 19 ? "Aries" : "Taurus",
                5 => day <= 20 ? "Taurus" : "Gemini",
                6 => day <= 20 ? "Gemini" : "Cancer",
                7 => day <= 22 ? "Cancer" : "Leo",
                8 => day <= 22 ? "Leo" : "Virgo",
                9 => day <= 22 ? "Virgo" : "Libra",
                10 => day <= 22 ? "Libra" : "Scorpio",
                11 => day <= 21 ? "Scorpio" : "Sagittarius",
                12 => day <= 21 ? "Sagittarius" : "Capricorn",
                _ => throw new ArgumentOutOfRangeException(nameof(birthDate), "Mes inválido")
            };
        }

        public static int GetDiasHastaCumpleanos(DateOnly birthDate)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            // correccion por febrero 29 en años no bisiestos
            int dia = birthDate.Month == 2 && birthDate.Day == 29
                      && !DateTime.IsLeapYear(today.Year)
                      ? 28
                      : birthDate.Day;

            var nextBirthday = new DateOnly(today.Year, birthDate.Month, dia);

            if (nextBirthday < today)
            {
                int nextYear = today.Year + 1;

                // Mismo control para el año siguiente
                dia = birthDate.Month == 2 && birthDate.Day == 29
                      && !DateTime.IsLeapYear(nextYear)
                      ? 28
                      : birthDate.Day;

                nextBirthday = new DateOnly(nextYear, birthDate.Month, dia);
            }

            return nextBirthday.DayNumber - today.DayNumber;
        }
    }
}
