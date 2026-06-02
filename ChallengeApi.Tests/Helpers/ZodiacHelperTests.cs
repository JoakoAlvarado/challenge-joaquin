using ChallengeApi.Application.Helpers;
using FluentAssertions;
using Xunit;

namespace ChallengeApi.Tests.Helpers
{
    public class ZodiacHelperTests
    {
        [Theory]
        [InlineData(1, 19, "Capricorn")]
        [InlineData(1, 20, "Aquarius")]
        [InlineData(2, 18, "Aquarius")]
        [InlineData(2, 19, "Pisces")]
        [InlineData(3, 20, "Pisces")]
        [InlineData(3, 21, "Aries")]
        [InlineData(4, 19, "Aries")]
        [InlineData(4, 20, "Taurus")]
        [InlineData(5, 20, "Taurus")]
        [InlineData(5, 21, "Gemini")]
        [InlineData(6, 20, "Gemini")]
        [InlineData(6, 21, "Cancer")]
        [InlineData(7, 22, "Cancer")]
        [InlineData(7, 23, "Leo")]
        [InlineData(8, 22, "Leo")]
        [InlineData(8, 23, "Virgo")]
        [InlineData(9, 22, "Virgo")]
        [InlineData(9, 23, "Libra")]
        [InlineData(10, 22, "Libra")]
        [InlineData(10, 23, "Scorpio")]
        [InlineData(11, 21, "Scorpio")]
        [InlineData(11, 22, "Sagittarius")]
        [InlineData(12, 21, "Sagittarius")]
        [InlineData(12, 22, "Capricorn")]
        public void GetSigno_DebeRetornarSignoCorrecto(int mes, int dia, string signoEsperado)
        {
            var birthDate = new DateOnly(2000, mes, dia);

            var resultado = ZodiacHelper.GetSigno(birthDate);

            resultado.Should().Be(signoEsperado);
        }

        [Fact]
        public void GetSigno_FechaValida_NoDebeLanzarExcepcion()
        {
            var action = () => ZodiacHelper.GetSigno(new DateOnly(2000, 1, 1));
            action.Should().NotThrow();
        }

        [Fact]
        public void GetDiasHastaCumpleanos_CumpleanosHoy_DebeRetornarCero()
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var resultado = ZodiacHelper.GetDiasHastaCumpleanos(today);

            resultado.Should().Be(0);
        }

        [Fact]
        public void GetDiasHastaCumpleanos_DebeRetornarValorPositivo()
        {
            // Una fecha que nunca sea hoy — usamos ayer del año pasado
            var birthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));

            var resultado = ZodiacHelper.GetDiasHastaCumpleanos(birthDate);

            resultado.Should().BeGreaterThan(0);
        }

        [Fact]
        public void GetDiasHastaCumpleanos_NacidoEl29Feb_NoDebeLanzarExcepcion()
        {
            var birthDate = new DateOnly(2000, 2, 29);

            var action = () => ZodiacHelper.GetDiasHastaCumpleanos(birthDate);

            action.Should().NotThrow();
        }
    }
}
