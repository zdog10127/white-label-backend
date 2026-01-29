using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WhiteLabel.Domain.Entity.Enum
{
    /// <summary>
    /// Permissões específicas do sistema AMPARA
    /// </summary>
    public static class Permissions
    {
        // ==========================================
        // PACIENTES
        // ==========================================
        public const string ViewPatients = "ViewPatients";
        public const string CreatePatients = "CreatePatients";
        public const string EditPatients = "EditPatients";
        public const string DeletePatients = "DeletePatients";

        // ==========================================
        // PRONTUÁRIOS E EVOLUÇÕES
        // ==========================================
        public const string ViewMedicalRecords = "ViewMedicalRecords";
        public const string CreateMedicalRecords = "CreateMedicalRecords";
        public const string EditMedicalRecords = "EditMedicalRecords";
        public const string DeleteMedicalRecords = "DeleteMedicalRecords";

        // ==========================================
        // AGENDA/AGENDAMENTOS
        // ==========================================
        public const string ViewAppointments = "ViewAppointments";
        public const string CreateAppointments = "CreateAppointments";
        public const string EditAppointments = "EditAppointments";
        public const string DeleteAppointments = "DeleteAppointments";

        // ==========================================
        // USUÁRIOS
        // ==========================================
        public const string ViewUsers = "ViewUsers";
        public const string CreateUsers = "CreateUsers";
        public const string EditUsers = "EditUsers";
        public const string DeleteUsers = "DeleteUsers";
        public const string ManageRoles = "ManageRoles";

        // ==========================================
        // RELATÓRIOS E EXPORTAÇÕES
        // ==========================================
        public const string ViewReports = "ViewReports";
        public const string ExportData = "ExportData";

        // ==========================================
        // CONFIGURAÇÕES
        // ==========================================
        public const string ManageSettings = "ManageSettings";

        /// <summary>
        /// Retorna todas as permissões de uma role
        /// </summary>
        public static string[] GetRolePermissions(string role)
        {
            return role switch
            {
                // ADMINISTRADOR: Acesso total
                UserRoles.Administrator => new[]
                {
                    ViewPatients, CreatePatients, EditPatients, DeletePatients,
                    ViewMedicalRecords, CreateMedicalRecords, EditMedicalRecords, DeleteMedicalRecords,
                    ViewAppointments, CreateAppointments, EditAppointments, DeleteAppointments,
                    ViewUsers, CreateUsers, EditUsers, DeleteUsers, ManageRoles,
                    ViewReports, ExportData,
                    ManageSettings
                },

                // ASSISTENTE SOCIAL: Gestão completa de pacientes + relatórios
                UserRoles.SocialWorker => new[]
                {
                    ViewPatients, CreatePatients, EditPatients, DeletePatients,
                    ViewMedicalRecords, // Pode visualizar prontuários mas não editar
                    ViewAppointments, CreateAppointments, EditAppointments,
                    ViewReports, ExportData
                },

                // NUTRICIONISTA: Gestão de prontuários + visualização de pacientes
                UserRoles.Nutritionist => new[]
                {
                    ViewPatients, EditPatients, // Pode ver e editar dados básicos
                    ViewMedicalRecords, CreateMedicalRecords, EditMedicalRecords, // Gestão completa de prontuários
                    ViewAppointments, CreateAppointments, EditAppointments, // Gestão de agenda
                    ViewReports
                },

                // PSICÓLOGO: Gestão de prontuários + visualização de pacientes
                UserRoles.Psychologist => new[]
                {
                    ViewPatients, EditPatients,
                    ViewMedicalRecords, CreateMedicalRecords, EditMedicalRecords,
                    ViewAppointments, CreateAppointments, EditAppointments,
                    ViewReports
                },

                // FISIOTERAPEUTA: Gestão de prontuários + visualização de pacientes
                UserRoles.Physiotherapist => new[]
                {
                    ViewPatients, EditPatients,
                    ViewMedicalRecords, CreateMedicalRecords, EditMedicalRecords,
                    ViewAppointments, CreateAppointments, EditAppointments,
                    ViewReports
                },

                // SECRETÁRIA: Apenas visualização de pacientes + gestão de agenda
                UserRoles.Secretary => new[]
                {
                    ViewPatients, // APENAS visualização, SEM criar/editar/deletar
                    ViewAppointments, CreateAppointments, EditAppointments, DeleteAppointments // Gestão completa de agenda
                },

                _ => new string[] { } // Sem permissões
            };
        }

        /// <summary>
        /// Verifica se uma role tem determinada permissão
        /// </summary>
        public static bool HasPermission(string role, string permission)
        {
            var rolePermissions = GetRolePermissions(role);
            return rolePermissions.Contains(permission);
        }

        /// <summary>
        /// Verifica se uma role tem alguma das permissões
        /// </summary>
        public static bool HasAnyPermission(string role, params string[] permissions)
        {
            var rolePermissions = GetRolePermissions(role);
            return permissions.Any(p => rolePermissions.Contains(p));
        }

        /// <summary>
        /// Verifica se uma role tem todas as permissões
        /// </summary>
        public static bool HasAllPermissions(string role, params string[] permissions)
        {
            var rolePermissions = GetRolePermissions(role);
            return permissions.All(p => rolePermissions.Contains(p));
        }

        /// <summary>
        /// Retorna descrição amigável da permissão
        /// </summary>
        public static string GetPermissionLabel(string permission)
        {
            return permission switch
            {
                ViewPatients => "Visualizar Pacientes",
                CreatePatients => "Criar Pacientes",
                EditPatients => "Editar Pacientes",
                DeletePatients => "Excluir Pacientes",
                ViewMedicalRecords => "Visualizar Prontuários",
                CreateMedicalRecords => "Criar Prontuários",
                EditMedicalRecords => "Editar Prontuários",
                DeleteMedicalRecords => "Excluir Prontuários",
                ViewAppointments => "Visualizar Agenda",
                CreateAppointments => "Criar Agendamentos",
                EditAppointments => "Editar Agendamentos",
                DeleteAppointments => "Excluir Agendamentos",
                ViewUsers => "Visualizar Usuários",
                CreateUsers => "Criar Usuários",
                EditUsers => "Editar Usuários",
                DeleteUsers => "Excluir Usuários",
                ManageRoles => "Gerenciar Funções",
                ViewReports => "Visualizar Relatórios",
                ExportData => "Exportar Dados",
                ManageSettings => "Gerenciar Configurações",
                _ => permission
            };
        }
    }
}