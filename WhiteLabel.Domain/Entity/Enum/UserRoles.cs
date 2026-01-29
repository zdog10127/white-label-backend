using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WhiteLabel.Domain.Entity.Enum
{
    /// <summary>
    /// Roles/Funções do sistema
    /// </summary>
    public static class UserRoles
    {
        public const string Administrator = "Administrator";        // Administrador
        public const string SocialWorker = "SocialWorker";         // Assistente Social
        public const string Nutritionist = "Nutritionist";         // Nutricionista
        public const string Psychologist = "Psychologist";         // Psicólogo
        public const string Physiotherapist = "Physiotherapist";   // Fisioterapeuta
        public const string Secretary = "Secretary";               // Secretária

        /// <summary>
        /// Retorna todas as roles disponíveis
        /// </summary>
        public static string[] GetAllRoles()
        {
            return new[]
            {
                Administrator,
                SocialWorker,
                Nutritionist,
                Psychologist,
                Physiotherapist,
                Secretary
            };
        }

        /// <summary>
        /// Retorna label amigável para exibição
        /// </summary>
        public static string GetRoleLabel(string role)
        {
            return role switch
            {
                Administrator => "Administrador",
                SocialWorker => "Assistente Social",
                Nutritionist => "Nutricionista",
                Psychologist => "Psicólogo",
                Physiotherapist => "Fisioterapeuta",
                Secretary => "Secretária",
                _ => role
            };
        }

        /// <summary>
        /// Verifica se a role é válida
        /// </summary>
        public static bool IsValidRole(string role)
        {
            return role == Administrator ||
                   role == SocialWorker ||
                   role == Nutritionist ||
                   role == Psychologist ||
                   role == Physiotherapist ||
                   role == Secretary;
        }

        /// <summary>
        /// Verifica se a role é profissional de saúde (pode acessar prontuários)
        /// </summary>
        public static bool IsHealthProfessional(string role)
        {
            return role == Nutritionist ||
                   role == Psychologist ||
                   role == Physiotherapist;
        }
    }
}