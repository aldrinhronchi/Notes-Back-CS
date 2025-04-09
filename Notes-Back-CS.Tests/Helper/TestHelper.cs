using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Notes_Back_CS.Connections.Database;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Notes_Back_CS.Tests.Helper
{
    public static class TestHelper
    {
        public static string GerarJwtFake(int IDUsuario = 123)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ChaveSecretaSuperSeguraParaTeste"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "TesteIssuer",
                audience: "TesteAudience",
                claims: new List<Claim> { new Claim(ClaimTypes.NameIdentifier, IDUsuario.ToString()) },
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static DatabaseContext CriarDbContextFake()
        {
            var serviceProvider = new ServiceCollection()
                .AddDbContext<DatabaseContext>(options =>
                    options.UseInMemoryDatabase("DbTest"))
                .BuildServiceProvider();

            return serviceProvider.GetRequiredService<DatabaseContext>();
        }

        public static string EncriptarSenha(string senha)
        {
            if (string.IsNullOrWhiteSpace(senha))
            {
                throw new ArgumentException("A senha não pode ser vazia ou nula.");
            }

            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes("ChaveSecretaSuperSeguraParaTeste")))
            {
                byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(senha));
                StringBuilder stringBuilder = new StringBuilder();

                foreach (byte caracter in hash)
                {
                    stringBuilder.Append(caracter.ToString("X2"));
                }

                return stringBuilder.ToString();
            }
        }

        public static Mock<IConfiguration> CriarConfigMock()
        {
            var configMock = new Mock<IConfiguration>();

            var sectionMock = new Mock<IConfigurationSection>();
            sectionMock.Setup(s => s.Value).Returns("ChaveSecretaSuperSeguraParaTeste");
            configMock.Setup(c => c.GetSection("SecuritySettings")).Returns(sectionMock.Object);
            configMock.Setup(c => c["SecuritySettings:SecretKey"]).Returns("ChaveSecretaSuperSeguraParaTeste");

            var sectionMockJwtKey = new Mock<IConfigurationSection>();
            sectionMockJwtKey.Setup(s => s.Value).Returns("ChaveSecretaSuperSeguraParaTeste");

            var sectionMockIssuer = new Mock<IConfigurationSection>();
            sectionMockIssuer.Setup(s => s.Value).Returns("TesteIssuer");

            var sectionMockAudience = new Mock<IConfigurationSection>();
            sectionMockAudience.Setup(s => s.Value).Returns("TesteAudience");

            configMock.Setup(c => c.GetSection("Jwt:Key")).Returns(sectionMockJwtKey.Object);
            configMock.Setup(c => c["Jwt:Key"]).Returns("ChaveSecretaSuperSeguraParaTeste");

            configMock.Setup(c => c.GetSection("Jwt:Issuer")).Returns(sectionMockIssuer.Object);
            configMock.Setup(c => c["Jwt:Issuer"]).Returns("TesteIssuer");

            configMock.Setup(c => c.GetSection("Jwt:Audience")).Returns(sectionMockAudience.Object);
            configMock.Setup(c => c["Jwt:Audience"]).Returns("TesteAudience");

            return configMock;
        }
    }
}