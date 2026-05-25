using Core.Entities;
using Core.Interfaces;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public  class BusinessRepository : IBusinessRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;

        public BusinessRepository(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<bool> ExistsAsync(string ruc_business, string code_license, long businessId, long? excludeId = null)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();

                var baseQuery = new StringBuilder("SELECT COUNT(*) FROM BUSINESS WHERE BUSINESS_RUC = @BUSINESS_RUC AND CODE_LICENSE = @CODE_LICENSE");

                if (excludeId.HasValue)
                    baseQuery.Append(" AND ID <> @ID");

                using var cmd = new SqlCommand(baseQuery.ToString(), connection);
                cmd.Parameters.AddWithValue("@BUSINESS_RUC", ruc_business);
                cmd.Parameters.AddWithValue("@CODE_LICENSE", code_license);
                cmd.Parameters.AddWithValue("@BID", businessId);
                if (excludeId.HasValue)
                    cmd.Parameters.AddWithValue("@ID", excludeId.Value);

                await connection.OpenAsync();
                var count = (int)await cmd.ExecuteScalarAsync();
                return count > 0;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al validar existencia de la empresa.", ex.Message);
            }

        }

        public async Task AddAsync(Business entity)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                using var cmd = new SqlCommand("SP_WS_REGISTER_BUSINESS", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@CODE_LICENSE", SqlDbType.VarChar).Value = entity.CodeLicense;
                cmd.Parameters.AddWithValue("@LICENSE", SqlDbType.VarChar).Value = entity.License;
                cmd.Parameters.AddWithValue("@BUSINESS_RUC", SqlDbType.VarChar).Value = entity.BusinessRuc;
                cmd.Parameters.AddWithValue("@BUSINESS_START_DATE", SqlDbType.VarChar).Value = entity.BusinessStartDate;
                cmd.Parameters.AddWithValue("@BUSINESS_EXERCISE", SqlDbType.VarChar).Value = entity.BusinessExercise;
                cmd.Parameters.AddWithValue("@COMPANY_NAME", SqlDbType.VarChar).Value = entity.CompanyName;
                cmd.Parameters.AddWithValue("@BUSINESS_NAME", SqlDbType.VarChar).Value = entity.BusinessName;
                cmd.Parameters.AddWithValue("@ABOUT_BUSINESS", SqlDbType.Text).Value = entity.AboutBusiness;
                cmd.Parameters.AddWithValue("@WEB_COMPANY", SqlDbType.VarChar).Value = entity.Website;
                cmd.Parameters.AddWithValue("@IS_MAIN_BUSINESS", SqlDbType.Bit).Value = entity.IsMain;
                cmd.Parameters.AddWithValue("@IS_VERIFIED", SqlDbType.Text).Value = entity.IsVerified;
                cmd.Parameters.AddWithValue("@BUSINESS_ADDRESS", SqlDbType.VarChar).Value = entity.BusinessAddress;
                cmd.Parameters.AddWithValue("@BUSINESS_COUNTRY", SqlDbType.VarChar).Value = entity.BusinessCountry;
                cmd.Parameters.AddWithValue("@DEPARTMENT", SqlDbType.VarChar).Value = entity.Department;
                cmd.Parameters.AddWithValue("@PROVINCE", SqlDbType.VarChar).Value = entity.Province;
                cmd.Parameters.AddWithValue("@DISTRICT", SqlDbType.VarChar).Value = entity.District;
                cmd.Parameters.AddWithValue("@BUSINESS_PHONE", SqlDbType.VarChar).Value = entity.BusinessPhone;
                cmd.Parameters.AddWithValue("@BUSINESS_EMAIL", SqlDbType.VarChar).Value = entity.BusinessEmail;
                cmd.Parameters.AddWithValue("@BUSINESS_LEGAL_REPRE", SqlDbType.VarChar).Value = entity.BusinessLegalRepre;
                cmd.Parameters.AddWithValue("@LEGAL_DOCUMENT_TYPE", SqlDbType.BigInt).Value = entity.LegalDocumentType;
                cmd.Parameters.AddWithValue("@LEGAL_DOCUMENT", SqlDbType.VarChar).Value = entity.LegalDocument;
                cmd.Parameters.AddWithValue("@LEGAL_FIRM", SqlDbType.VarChar).Value = entity.LegalFirm;
                cmd.Parameters.AddWithValue("@FILE_RUC", SqlDbType.VarChar).Value = entity.FileRuc;
                cmd.Parameters.AddWithValue("@FILE_COMPLIANCE_CERTIFICATE", SqlDbType.VarChar).Value = entity.FileComplanceCertificate;
                cmd.Parameters.AddWithValue("@CURRENCY_MAIN", SqlDbType.BigInt).Value = entity.CurrencyMain;
                cmd.Parameters.AddWithValue("@CURRENCY_SECONDARY", SqlDbType.BigInt).Value = entity.CurrencySecondary;
                cmd.Parameters.AddWithValue("@BUSINESS_LOGO", SqlDbType.VarChar).Value = entity.BusinessLogo;
                cmd.Parameters.AddWithValue("@RETENTION_AGENT", SqlDbType.Char).Value = entity.RetentionAgent;
                cmd.Parameters.AddWithValue("@PERCEPTION_AGENT", SqlDbType.Char).Value = entity.PerceptionAgent;
                cmd.Parameters.AddWithValue("@PRICOS", SqlDbType.Char).Value = entity.Pricos;
                cmd.Parameters.AddWithValue("@CREATE_USER", SqlDbType.Int).Value = entity.UsersBy;

                await connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al registrar la empresa en la base de datos.", ex.Message);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al guardar la empresa.", ex.Message);
            }
        }

        public async Task<Business?> GetByViewAsync(long businessId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                // --- 1) Obtener la empresa ---
                using (var cmd = new SqlCommand("SP_WS_BUSINESS_VIEW", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                })

                {
                    cmd.Parameters.Add("@BUSINESS_ID", SqlDbType.BigInt).Value = businessId;

                    using var reader = await cmd.ExecuteReaderAsync();

                    if (!reader.HasRows || !await reader.ReadAsync())
                        return null;

                    int oBusinessRuc = reader.GetOrdinal("BUSINESS_RUC");
                    int oBusinessName = reader.GetOrdinal("BUSINESS_NAME");
                    int oAboutBusiness = reader.GetOrdinal("ABOUT_BUSINESS");
                    int oIsMain = reader.GetOrdinal("IS_MAIN_BUSINESS");
                    int oIsVerified = reader.GetOrdinal("IS_VERIFIED");
                    int oBusinessEmail = reader.GetOrdinal("BUSINESS_EMAIL");
                    int oBusinessPhone = reader.GetOrdinal("BUSINESS_PHONE");
                    int oBusinessLegalRepre = reader.GetOrdinal("BUSINESS_LEGAL_REPRE");
                    int oLegalDocument = reader.GetOrdinal("LEGAL_DOCUMENT");
                    int oLegalFirm = reader.GetOrdinal("LEGAL_FIRM");
                    int oPricos = reader.GetOrdinal("PRICOS");
                    int oPerceptionAgent = reader.GetOrdinal("PERCEPTION_AGENT");
                    int oRetentionAgent = reader.GetOrdinal("RETENTION_AGENT");
                    int oLicense = reader.GetOrdinal("LICENSE");
                    int oFileRuc = reader.GetOrdinal("FILE_RUC");
                    int oFileComplanceCertificate = reader.GetOrdinal("FILE_COMPLIANCE_CERTIFICATE");
                    int oAbrv = reader.GetOrdinal("ABRV");
                    int oBusinessLogo = reader.GetOrdinal("BUSINESS_LOGO");
                    int oCompanyName = reader.GetOrdinal("COMPANY_NAME");
                    int oWebsite = reader.GetOrdinal("WEB_COMPANY");

                    var business = new Business
                    {
                        BusinessRuc = reader.GetString(oBusinessRuc),
                        BusinessName = reader.GetString(oBusinessName),
                        AboutBusiness = reader.IsDBNull(oAboutBusiness) ? null : reader.GetString(oAboutBusiness),
                        IsMain = reader.GetBoolean(oIsMain),
                        IsVerified = reader.GetBoolean(oIsVerified),
                        BusinessEmail = reader.IsDBNull(oBusinessEmail) ? null : reader.GetString(oBusinessEmail),
                        BusinessPhone = reader.IsDBNull(oBusinessPhone) ? null : reader.GetString(oBusinessPhone),
                        BusinessLegalRepre = reader.IsDBNull(oBusinessLegalRepre) ? null : reader.GetString(oBusinessLegalRepre),
                        LegalDocument = reader.IsDBNull(oLegalDocument) ? null : reader.GetString(oLegalDocument),
                        LegalFirm = reader.IsDBNull(oLegalFirm) ? null : reader.GetString(oLegalFirm),
                        Pricos = reader.IsDBNull(oPricos) ? null : reader.GetString(oPricos),
                        PerceptionAgent = reader.IsDBNull(oPerceptionAgent) ? null : reader.GetString(oPerceptionAgent),
                        RetentionAgent = reader.IsDBNull(oRetentionAgent) ? null : reader.GetString(oRetentionAgent),
                        License = reader.IsDBNull(oLicense) ? null : reader.GetString(oLicense),
                        FileRuc = reader.IsDBNull(oFileRuc) ? null : reader.GetString(oFileRuc),
                        FileComplanceCertificate = reader.IsDBNull(oFileComplanceCertificate) ? null : reader.GetString(oFileComplanceCertificate),
                        Abrv = reader.IsDBNull(oAbrv) ? null : reader.GetString(oAbrv),
                        BusinessLogo = reader.IsDBNull(oBusinessLogo) ? null : reader.GetString(oBusinessLogo),
                        CompanyName = reader.IsDBNull(oCompanyName) ? null : reader.GetString(oCompanyName),
                        Website = reader.IsDBNull(oWebsite) ? null : reader.GetString(oWebsite),
                        AddressBusiness = new List<AddressBusiness>()
                    };

                    await reader.CloseAsync();

                    // --- 2) Obtener direcciones ---
                    using var cmdAdd = new SqlCommand("SP_WS_BUSINESS_ADDRESS_VIEW", cn)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = 15
                    };
                    cmdAdd.Parameters.Add("@BUSINESS_ID", SqlDbType.BigInt).Value = businessId;

                    using var r2 = await cmdAdd.ExecuteReaderAsync();
                    if (r2.HasRows)
                    {
                        int oId = r2.GetOrdinal("ID");
                        int oLabel = r2.GetOrdinal("LABEL");
                        int oAddress = r2.GetOrdinal("ADRRESS");
                        int oDistrict = r2.GetOrdinal("DISTRICT");
                        int oProvince = r2.GetOrdinal("PROVINCE");
                        int oDepartment = r2.GetOrdinal("DEPARTMENT");
                        int oMainAddress = r2.GetOrdinal("MAIN_ADDRESS");

                        while (await r2.ReadAsync())
                        {
                            business.AddressBusiness.Add(new AddressBusiness
                            {
                                Id = r2.IsDBNull(oId) ? 0 : r2.GetInt64(oId),
                                Label = r2.IsDBNull(oLabel) ? null : r2.GetString(oLabel),
                                Address = r2.IsDBNull(oAddress) ? null : r2.GetString(oAddress),
                                District = r2.IsDBNull(oDistrict) ? null : r2.GetString(oDistrict),
                                Province = r2.IsDBNull(oProvince) ? null : r2.GetString(oProvince),
                                Department = r2.IsDBNull(oDepartment) ? null : r2.GetString(oDepartment),
                                MainAddress = !r2.IsDBNull(oMainAddress) && r2.GetBoolean(oMainAddress)
                            });
                        }
                    }

                    return business;
                }
            }
            catch (SqlException ex)
            {
               
                throw new DatabaseException("Error al obtener la empresa por ID.", ex.Message);
            }
        }

    }
}
