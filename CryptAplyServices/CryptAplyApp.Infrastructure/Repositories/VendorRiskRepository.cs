using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CryptAplyApp.Application.Models;
using CryptAplyApp.Infrastructure.Data;
using CryptAplyApp.Infrastructure.Entities;

namespace CryptAplyApp.Infrastructure.Repositories
{
    public interface IVendorRiskRepository
    {
        Task<VendorProfile> GetVendorAsync(string vendorId);
        Task<VendorProfile> CreateVendorAsync(VendorProfile vendor);
        Task<VendorProfile> UpdateVendorAsync(VendorProfile vendor);
        Task<bool> DeleteVendorAsync(string vendorId);
        Task<List<VendorProfile>> GetAllVendorsAsync();
        Task<VendorRiskScore> GetVendorRiskScoreAsync(string vendorId);
        Task<VendorRiskScore> UpdateVendorRiskScoreAsync(string vendorId, VendorRiskScore riskScore);
    }

    public class VendorRiskRepository : IVendorRiskRepository
    {
        private readonly PciComplianceDbContext _context;

        public VendorRiskRepository(PciComplianceDbContext context)
        {
            _context = context;
        }

        public async Task<VendorProfile> GetVendorAsync(string vendorId)
        {
            var vendorEntity = await _context.Vendors
                .Include(v => v.Services)
                .Include(v => v.Contacts)
                .Include(v => v.Certifications)
                .Include(v => v.RiskScore)
                    .ThenInclude(r => r.Findings)
                .FirstOrDefaultAsync(v => v.VendorId == vendorId);

            return vendorEntity != null ? MapToVendorProfile(vendorEntity) : null;
        }

        public async Task<VendorProfile> CreateVendorAsync(VendorProfile vendor)
        {
            var vendorEntity = MapToVendorEntity(vendor);
            _context.Vendors.Add(vendorEntity);
            await _context.SaveChangesAsync();
            return await GetVendorAsync(vendorEntity.VendorId);
        }

        public async Task<VendorProfile> UpdateVendorAsync(VendorProfile vendor)
        {
            var existingVendor = await _context.Vendors
                .Include(v => v.Services)
                .Include(v => v.Contacts)
                .Include(v => v.Certifications)
                .Include(v => v.RiskScore)
                    .ThenInclude(r => r.Findings)
                .FirstOrDefaultAsync(v => v.VendorId == vendor.VendorId);

            if (existingVendor == null)
                return null;

            UpdateVendorEntity(existingVendor, vendor);
            await _context.SaveChangesAsync();
            return await GetVendorAsync(vendor.VendorId);
        }

        public async Task<bool> DeleteVendorAsync(string vendorId)
        {
            var vendor = await _context.Vendors.FindAsync(vendorId);
            if (vendor == null)
                return false;

            _context.Vendors.Remove(vendor);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<VendorProfile>> GetAllVendorsAsync()
        {
            var vendors = await _context.Vendors
                .Include(v => v.Services)
                .Include(v => v.Contacts)
                .Include(v => v.Certifications)
                .Include(v => v.RiskScore)
                    .ThenInclude(r => r.Findings)
                .ToListAsync();

            return vendors.Select(MapToVendorProfile).ToList();
        }

        public async Task<VendorRiskScore> GetVendorRiskScoreAsync(string vendorId)
        {
            var riskScoreEntity = await _context.VendorRiskScores
                .Include(r => r.Findings)
                .FirstOrDefaultAsync(r => r.VendorId == vendorId);

            return riskScoreEntity != null ? MapToVendorRiskScore(riskScoreEntity) : null;
        }

        public async Task<VendorRiskScore> UpdateVendorRiskScoreAsync(string vendorId, VendorRiskScore riskScore)
        {
            var riskScoreEntity = await _context.VendorRiskScores
                .Include(r => r.Findings)
                .FirstOrDefaultAsync(r => r.VendorId == vendorId);

            if (riskScoreEntity == null)
                return null;

            UpdateRiskScoreEntity(riskScoreEntity, riskScore);
            await _context.SaveChangesAsync();
            return await GetVendorRiskScoreAsync(vendorId);
        }

        private VendorProfile MapToVendorProfile(VendorEntity entity)
        {
            if (entity == null)
                return null;

            return new VendorProfile
            {
                VendorId = entity.VendorId,
                Name = entity.Name,
                Description = entity.Description,
                Type = entity.Type,
                Status = entity.Status,
                OnboardingDate = entity.OnboardingDate,
                LastAssessmentDate = entity.LastAssessmentDate,
                Services = entity.Services.Select(MapToVendorService).ToList(),
                Contacts = entity.Contacts.Select(MapToVendorContact).ToList(),
                Certifications = entity.Certifications.Select(MapToCertification).ToList(),
                RiskScore = MapToVendorRiskScore(entity.RiskScore)
            };
        }

        private VendorEntity MapToVendorEntity(VendorProfile profile)
        {
            if (profile == null)
                return null;

            return new VendorEntity
            {
                VendorId = string.IsNullOrEmpty(profile.VendorId) ? Guid.NewGuid().ToString() : profile.VendorId,
                Name = profile.Name,
                Description = profile.Description,
                Type = profile.Type,
                Status = profile.Status,
                OnboardingDate = profile.OnboardingDate,
                LastAssessmentDate = profile.LastAssessmentDate,
                Services = profile.Services?.Select(MapToVendorServiceEntity).ToList(),
                Contacts = profile.Contacts?.Select(MapToVendorContactEntity).ToList(),
                Certifications = profile.Certifications?.Select(MapToCertificationEntity).ToList(),
                RiskScore = MapToVendorRiskScoreEntity(profile.RiskScore)
            };
        }

        private void UpdateVendorEntity(VendorEntity entity, VendorProfile profile)
        {
            entity.Name = profile.Name;
            entity.Description = profile.Description;
            entity.Type = profile.Type;
            entity.Status = profile.Status;
            entity.LastAssessmentDate = profile.LastAssessmentDate;

            // Update services
            _context.VendorServices.RemoveRange(entity.Services);
            entity.Services = profile.Services?.Select(MapToVendorServiceEntity).ToList();

            // Update contacts
            _context.VendorContacts.RemoveRange(entity.Contacts);
            entity.Contacts = profile.Contacts?.Select(MapToVendorContactEntity).ToList();

            // Update certifications
            _context.VendorCertifications.RemoveRange(entity.Certifications);
            entity.Certifications = profile.Certifications?.Select(MapToCertificationEntity).ToList();

            // Update risk score
            if (profile.RiskScore != null)
            {
                if (entity.RiskScore == null)
                {
                    entity.RiskScore = MapToVendorRiskScoreEntity(profile.RiskScore);
                }
                else
                {
                    UpdateRiskScoreEntity(entity.RiskScore, profile.RiskScore);
                }
            }
        }

        private VendorService MapToVendorService(VendorServiceEntity entity)
        {
            return new VendorService
            {
                ServiceId = entity.ServiceId,
                Name = entity.Name,
                Category = entity.Category,
                Description = entity.Description,
                DataClassification = entity.DataClassification,
                InvolvesKeyManagement = entity.InvolvesKeyManagement,
                RequiresPciCompliance = entity.RequiresPciCompliance,
                AffectedSystems = JsonSerializer.Deserialize<List<string>>(entity.AffectedSystemsJson),
                Dependencies = JsonSerializer.Deserialize<List<string>>(entity.DependenciesJson)
            };
        }

        private VendorServiceEntity MapToVendorServiceEntity(VendorService service)
        {
            return new VendorServiceEntity
            {
                ServiceId = string.IsNullOrEmpty(service.ServiceId) ? Guid.NewGuid().ToString() : service.ServiceId,
                Name = service.Name,
                Category = service.Category,
                Description = service.Description,
                DataClassification = service.DataClassification,
                InvolvesKeyManagement = service.InvolvesKeyManagement,
                RequiresPciCompliance = service.RequiresPciCompliance,
                AffectedSystemsJson = JsonSerializer.Serialize(service.AffectedSystems),
                DependenciesJson = JsonSerializer.Serialize(service.Dependencies)
            };
        }

        private VendorContact MapToVendorContact(VendorContactEntity entity)
        {
            return new VendorContact
            {
                ContactId = entity.ContactId,
                Name = entity.Name,
                Role = entity.Role,
                Email = entity.Email,
                Phone = entity.Phone,
                IsPrimary = entity.IsPrimary,
                Type = entity.Type,
                ResponsibleFor = JsonSerializer.Deserialize<List<string>>(entity.ResponsibleForJson)
            };
        }

        private VendorContactEntity MapToVendorContactEntity(VendorContact contact)
        {
            return new VendorContactEntity
            {
                ContactId = string.IsNullOrEmpty(contact.ContactId) ? Guid.NewGuid().ToString() : contact.ContactId,
                Name = contact.Name,
                Role = contact.Role,
                Email = contact.Email,
                Phone = contact.Phone,
                IsPrimary = contact.IsPrimary,
                Type = contact.Type,
                ResponsibleForJson = JsonSerializer.Serialize(contact.ResponsibleFor)
            };
        }

        private ComplianceCertification MapToCertification(VendorCertificationEntity entity)
        {
            return new ComplianceCertification
            {
                CertificationId = entity.CertificationId,
                Name = entity.Name,
                Version = entity.Version,
                IssueDate = entity.IssueDate,
                ExpiryDate = entity.ExpiryDate,
                IssuingBody = entity.IssuingBody,
                DocumentUrl = entity.DocumentUrl,
                Status = entity.Status
            };
        }

        private VendorCertificationEntity MapToCertificationEntity(ComplianceCertification certification)
        {
            return new VendorCertificationEntity
            {
                CertificationId = string.IsNullOrEmpty(certification.CertificationId) ? Guid.NewGuid().ToString() : certification.CertificationId,
                Name = certification.Name,
                Version = certification.Version,
                IssueDate = certification.IssueDate,
                ExpiryDate = certification.ExpiryDate,
                IssuingBody = certification.IssuingBody,
                DocumentUrl = certification.DocumentUrl,
                Status = certification.Status
            };
        }

        private VendorRiskScore MapToVendorRiskScore(VendorRiskScoreEntity entity)
        {
            if (entity == null)
                return null;

            return new VendorRiskScore
            {
                OverallScore = entity.OverallScore,
                CategoryScores = JsonSerializer.Deserialize<Dictionary<string, double>>(entity.CategoryScoresJson),
                Findings = entity.Findings.Select(MapToRiskFinding).ToList(),
                LastUpdated = entity.LastUpdated,
                AssessedBy = entity.AssessedBy,
                Trend = entity.Trend
            };
        }

        private VendorRiskScoreEntity MapToVendorRiskScoreEntity(VendorRiskScore riskScore)
        {
            if (riskScore == null)
                return null;

            return new VendorRiskScoreEntity
            {
                RiskScoreId = Guid.NewGuid().ToString(),
                OverallScore = riskScore.OverallScore,
                CategoryScoresJson = JsonSerializer.Serialize(riskScore.CategoryScores),
                Findings = riskScore.Findings?.Select(MapToRiskFindingEntity).ToList(),
                LastUpdated = riskScore.LastUpdated,
                AssessedBy = riskScore.AssessedBy,
                Trend = riskScore.Trend
            };
        }

        private void UpdateRiskScoreEntity(VendorRiskScoreEntity entity, VendorRiskScore riskScore)
        {
            entity.OverallScore = riskScore.OverallScore;
            entity.CategoryScoresJson = JsonSerializer.Serialize(riskScore.CategoryScores);
            entity.LastUpdated = riskScore.LastUpdated;
            entity.AssessedBy = riskScore.AssessedBy;
            entity.Trend = riskScore.Trend;

            // Update findings
            _context.RiskFindings.RemoveRange(entity.Findings);
            entity.Findings = riskScore.Findings?.Select(MapToRiskFindingEntity).ToList();
        }

        private RiskFinding MapToRiskFinding(RiskFindingEntity entity)
        {
            return new RiskFinding
            {
                FindingId = entity.FindingId,
                Description = entity.Description,
                Severity = entity.Severity,
                Category = entity.Category,
                IdentificationDate = entity.IdentificationDate,
                RemediationDate = entity.RemediationDate,
                RemediationPlan = entity.RemediationPlan,
                Status = entity.Status
            };
        }

        private RiskFindingEntity MapToRiskFindingEntity(RiskFinding finding)
        {
            return new RiskFindingEntity
            {
                FindingId = string.IsNullOrEmpty(finding.FindingId) ? Guid.NewGuid().ToString() : finding.FindingId,
                Description = finding.Description,
                Severity = finding.Severity,
                Category = finding.Category,
                IdentificationDate = finding.IdentificationDate,
                RemediationDate = finding.RemediationDate,
                RemediationPlan = finding.RemediationPlan,
                Status = finding.Status
            };
        }
    }
}
