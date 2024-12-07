using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CryptAplyApp.Application.Models;

namespace CryptAplyApp.Application.Interfaces
{
    public interface IKeyManagementService
    {
        // Key Creation and Management
        Task<CryptoKeyDto> CreateKeyAsync(CreateKeyRequest request);
        Task<CryptoKeyDto> GetKeyAsync(int keyId);
        Task<IEnumerable<CryptoKeyDto>> GetTeamKeysAsync(int teamId);
        Task<bool> RotateKeyAsync(int keyId, RotateKeyRequest request);
        Task<bool> MarkKeyCompromisedAsync(int keyId, CompromiseKeyRequest request);
        Task<bool> ArchiveKeyAsync(int keyId);
        
        // Key Actions and Voting
        Task<KeyActionDto> InitiateKeyActionAsync(CreateKeyActionRequest request);
        Task<KeyActionDto> GetKeyActionAsync(int actionId);
        Task<bool> VoteOnKeyActionAsync(int actionId, KeyActionVoteRequest request);
        Task<KeyActionStatusDto> GetKeyActionStatusAsync(int actionId);
        Task<IEnumerable<KeyActionDto>> GetPendingActionsAsync(int teamId);
        
        // Key Usage and Metrics
        Task<KeyMetricsDto> GetKeyMetricsAsync(int keyId);
        Task<IEnumerable<KeyUsageLogDto>> GetKeyUsageHistoryAsync(int keyId, DateTime startDate, DateTime endDate);
        Task<bool> ValidateKeyUsageAsync(int keyId, ValidateKeyUsageRequest request);
        Task<UsageStatisticsDto> GetUsageStatisticsAsync(int keyId, DateTime startDate, DateTime endDate);
        
        // Emergency Procedures
        Task<bool> InitiateEmergencyProcedureAsync(int keyId, EmergencyProcedureRequest request);
        Task<bool> CancelEmergencyProcedureAsync(int keyId);
    }
}
