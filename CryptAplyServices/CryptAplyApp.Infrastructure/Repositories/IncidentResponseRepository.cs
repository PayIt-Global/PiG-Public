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
    public interface IIncidentResponseRepository
    {
        Task<SecurityIncident> GetIncidentAsync(string incidentId);
        Task<SecurityIncident> CreateIncidentAsync(SecurityIncident incident);
        Task<SecurityIncident> UpdateIncidentAsync(SecurityIncident incident);
        Task<bool> DeleteIncidentAsync(string incidentId);
        Task<List<SecurityIncident>> GetAllIncidentsAsync();
        Task<AutomatedResponse> CreateAutomatedResponseAsync(string incidentId, AutomatedResponse response);
        Task<AutomatedResponse> UpdateAutomatedResponseAsync(string responseId, AutomatedResponse response);
    }

    public class IncidentResponseRepository : IIncidentResponseRepository
    {
        private readonly PciComplianceDbContext _context;

        public IncidentResponseRepository(PciComplianceDbContext context)
        {
            _context = context;
        }

        public async Task<SecurityIncident> GetIncidentAsync(string incidentId)
        {
            var incidentEntity = await _context.SecurityIncidents
                .Include(i => i.ContainmentActions)
                .Include(i => i.Evidence)
                .Include(i => i.Timeline)
                    .ThenInclude(t => t.Events)
                .FirstOrDefaultAsync(i => i.IncidentId == incidentId);

            return incidentEntity != null ? MapToSecurityIncident(incidentEntity) : null;
        }

        public async Task<SecurityIncident> CreateIncidentAsync(SecurityIncident incident)
        {
            var incidentEntity = MapToSecurityIncidentEntity(incident);
            _context.SecurityIncidents.Add(incidentEntity);
            await _context.SaveChangesAsync();
            return await GetIncidentAsync(incidentEntity.IncidentId);
        }

        public async Task<SecurityIncident> UpdateIncidentAsync(SecurityIncident incident)
        {
            var existingIncident = await _context.SecurityIncidents
                .Include(i => i.ContainmentActions)
                .Include(i => i.Evidence)
                .Include(i => i.Timeline)
                    .ThenInclude(t => t.Events)
                .FirstOrDefaultAsync(i => i.IncidentId == incident.IncidentId);

            if (existingIncident == null)
                return null;

            UpdateSecurityIncidentEntity(existingIncident, incident);
            await _context.SaveChangesAsync();
            return await GetIncidentAsync(incident.IncidentId);
        }

        public async Task<bool> DeleteIncidentAsync(string incidentId)
        {
            var incident = await _context.SecurityIncidents.FindAsync(incidentId);
            if (incident == null)
                return false;

            _context.SecurityIncidents.Remove(incident);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<SecurityIncident>> GetAllIncidentsAsync()
        {
            var incidents = await _context.SecurityIncidents
                .Include(i => i.ContainmentActions)
                .Include(i => i.Evidence)
                .Include(i => i.Timeline)
                    .ThenInclude(t => t.Events)
                .ToListAsync();

            return incidents.Select(MapToSecurityIncident).ToList();
        }

        public async Task<AutomatedResponse> CreateAutomatedResponseAsync(string incidentId, AutomatedResponse response)
        {
            var responseEntity = MapToAutomatedResponseEntity(response);
            responseEntity.IncidentId = incidentId;
            _context.AutomatedResponses.Add(responseEntity);
            await _context.SaveChangesAsync();
            return MapToAutomatedResponse(responseEntity);
        }

        public async Task<AutomatedResponse> UpdateAutomatedResponseAsync(string responseId, AutomatedResponse response)
        {
            var existingResponse = await _context.AutomatedResponses
                .Include(r => r.Actions)
                .FirstOrDefaultAsync(r => r.ResponseId == responseId);

            if (existingResponse == null)
                return null;

            UpdateAutomatedResponseEntity(existingResponse, response);
            await _context.SaveChangesAsync();
            return MapToAutomatedResponse(existingResponse);
        }

        private SecurityIncident MapToSecurityIncident(SecurityIncidentEntity entity)
        {
            if (entity == null)
                return null;

            return new SecurityIncident
            {
                IncidentId = entity.IncidentId,
                Title = entity.Title,
                Type = entity.Type,
                Severity = entity.Severity,
                Status = entity.Status,
                DetectionTime = entity.DetectionTime,
                ResolutionTime = entity.ResolutionTime,
                DetectedBy = entity.DetectedBy,
                Description = entity.Description,
                RequiresDisclosure = entity.RequiresDisclosure,
                AffectedSystems = JsonSerializer.Deserialize<List<string>>(entity.AffectedSystemsJson),
                AffectedKeys = JsonSerializer.Deserialize<List<string>>(entity.AffectedKeysJson),
                NotifiedParties = JsonSerializer.Deserialize<List<string>>(entity.NotifiedPartiesJson),
                ContainmentActions = entity.ContainmentActions.Select(MapToContainmentAction).ToList(),
                Evidence = entity.Evidence.Select(MapToForensicEvidence).ToList(),
                Timeline = MapToIncidentTimeline(entity.Timeline)
            };
        }

        private SecurityIncidentEntity MapToSecurityIncidentEntity(SecurityIncident incident)
        {
            if (incident == null)
                return null;

            return new SecurityIncidentEntity
            {
                IncidentId = string.IsNullOrEmpty(incident.IncidentId) ? Guid.NewGuid().ToString() : incident.IncidentId,
                Title = incident.Title,
                Type = incident.Type,
                Severity = incident.Severity,
                Status = incident.Status,
                DetectionTime = incident.DetectionTime,
                ResolutionTime = incident.ResolutionTime,
                DetectedBy = incident.DetectedBy,
                Description = incident.Description,
                RequiresDisclosure = incident.RequiresDisclosure,
                AffectedSystemsJson = JsonSerializer.Serialize(incident.AffectedSystems),
                AffectedKeysJson = JsonSerializer.Serialize(incident.AffectedKeys),
                NotifiedPartiesJson = JsonSerializer.Serialize(incident.NotifiedParties),
                ContainmentActions = incident.ContainmentActions?.Select(MapToContainmentActionEntity).ToList(),
                Evidence = incident.Evidence?.Select(MapToForensicEvidenceEntity).ToList(),
                Timeline = MapToIncidentTimelineEntity(incident.Timeline)
            };
        }

        private void UpdateSecurityIncidentEntity(SecurityIncidentEntity entity, SecurityIncident incident)
        {
            entity.Title = incident.Title;
            entity.Type = incident.Type;
            entity.Severity = incident.Severity;
            entity.Status = incident.Status;
            entity.ResolutionTime = incident.ResolutionTime;
            entity.Description = incident.Description;
            entity.RequiresDisclosure = incident.RequiresDisclosure;
            entity.AffectedSystemsJson = JsonSerializer.Serialize(incident.AffectedSystems);
            entity.AffectedKeysJson = JsonSerializer.Serialize(incident.AffectedKeys);
            entity.NotifiedPartiesJson = JsonSerializer.Serialize(incident.NotifiedParties);

            // Update containment actions
            _context.ContainmentActions.RemoveRange(entity.ContainmentActions);
            entity.ContainmentActions = incident.ContainmentActions?.Select(MapToContainmentActionEntity).ToList();

            // Update evidence
            _context.ForensicEvidence.RemoveRange(entity.Evidence);
            entity.Evidence = incident.Evidence?.Select(MapToForensicEvidenceEntity).ToList();

            // Update timeline
            if (incident.Timeline != null)
            {
                if (entity.Timeline == null)
                {
                    entity.Timeline = MapToIncidentTimelineEntity(incident.Timeline);
                }
                else
                {
                    UpdateIncidentTimelineEntity(entity.Timeline, incident.Timeline);
                }
            }
        }

        private ContainmentAction MapToContainmentAction(ContainmentActionEntity entity)
        {
            return new ContainmentAction
            {
                ActionId = entity.ActionId,
                Description = entity.Description,
                Type = entity.Type,
                Status = entity.Status,
                InitiationTime = entity.InitiationTime,
                CompletionTime = entity.CompletionTime,
                ExecutedBy = entity.ExecutedBy,
                IsAutomated = entity.IsAutomated,
                Result = entity.Result,
                AffectedComponents = JsonSerializer.Deserialize<List<string>>(entity.AffectedComponentsJson)
            };
        }

        private ContainmentActionEntity MapToContainmentActionEntity(ContainmentAction action)
        {
            return new ContainmentActionEntity
            {
                ActionId = string.IsNullOrEmpty(action.ActionId) ? Guid.NewGuid().ToString() : action.ActionId,
                Description = action.Description,
                Type = action.Type,
                Status = action.Status,
                InitiationTime = action.InitiationTime,
                CompletionTime = action.CompletionTime,
                ExecutedBy = action.ExecutedBy,
                IsAutomated = action.IsAutomated,
                Result = action.Result,
                AffectedComponentsJson = JsonSerializer.Serialize(action.AffectedComponents)
            };
        }

        private ForensicEvidence MapToForensicEvidence(ForensicEvidenceEntity entity)
        {
            return new ForensicEvidence
            {
                EvidenceId = entity.EvidenceId,
                Description = entity.Description,
                Type = entity.Type,
                Location = entity.Location,
                CollectionTime = entity.CollectionTime,
                CollectedBy = entity.CollectedBy,
                HashValue = entity.HashValue,
                ChainOfCustody = entity.ChainOfCustody,
                IsSealed = entity.IsSealed,
                RetentionDate = entity.RetentionDate
            };
        }

        private ForensicEvidenceEntity MapToForensicEvidenceEntity(ForensicEvidence evidence)
        {
            return new ForensicEvidenceEntity
            {
                EvidenceId = string.IsNullOrEmpty(evidence.EvidenceId) ? Guid.NewGuid().ToString() : evidence.EvidenceId,
                Description = evidence.Description,
                Type = evidence.Type,
                Location = evidence.Location,
                CollectionTime = evidence.CollectionTime,
                CollectedBy = evidence.CollectedBy,
                HashValue = evidence.HashValue,
                ChainOfCustody = evidence.ChainOfCustody,
                IsSealed = evidence.IsSealed,
                RetentionDate = evidence.RetentionDate
            };
        }

        private IncidentTimeline MapToIncidentTimeline(IncidentTimelineEntity entity)
        {
            if (entity == null)
                return null;

            return new IncidentTimeline
            {
                TimelineId = entity.TimelineId,
                Events = entity.Events.Select(MapToTimelineEvent).ToList(),
                FirstEventTime = entity.FirstEventTime,
                LastEventTime = entity.LastEventTime,
                TotalResolutionTime = entity.TotalResolutionTime,
                KeyMilestones = JsonSerializer.Deserialize<List<string>>(entity.KeyMilestonesJson)
            };
        }

        private IncidentTimelineEntity MapToIncidentTimelineEntity(IncidentTimeline timeline)
        {
            if (timeline == null)
                return null;

            return new IncidentTimelineEntity
            {
                TimelineId = string.IsNullOrEmpty(timeline.TimelineId) ? Guid.NewGuid().ToString() : timeline.TimelineId,
                Events = timeline.Events?.Select(MapToTimelineEventEntity).ToList(),
                FirstEventTime = timeline.FirstEventTime,
                LastEventTime = timeline.LastEventTime,
                TotalResolutionTime = timeline.TotalResolutionTime,
                KeyMilestonesJson = JsonSerializer.Serialize(timeline.KeyMilestones)
            };
        }

        private void UpdateIncidentTimelineEntity(IncidentTimelineEntity entity, IncidentTimeline timeline)
        {
            entity.FirstEventTime = timeline.FirstEventTime;
            entity.LastEventTime = timeline.LastEventTime;
            entity.TotalResolutionTime = timeline.TotalResolutionTime;
            entity.KeyMilestonesJson = JsonSerializer.Serialize(timeline.KeyMilestones);

            // Update events
            _context.TimelineEvents.RemoveRange(entity.Events);
            entity.Events = timeline.Events?.Select(MapToTimelineEventEntity).ToList();
        }

        private TimelineEvent MapToTimelineEvent(TimelineEventEntity entity)
        {
            return new TimelineEvent
            {
                EventId = entity.EventId,
                Timestamp = entity.Timestamp,
                Description = entity.Description,
                Actor = entity.Actor,
                Category = entity.Category,
                Evidence = entity.Evidence,
                Metadata = JsonSerializer.Deserialize<Dictionary<string, string>>(entity.MetadataJson)
            };
        }

        private TimelineEventEntity MapToTimelineEventEntity(TimelineEvent timelineEvent)
        {
            return new TimelineEventEntity
            {
                EventId = string.IsNullOrEmpty(timelineEvent.EventId) ? Guid.NewGuid().ToString() : timelineEvent.EventId,
                Timestamp = timelineEvent.Timestamp,
                Description = timelineEvent.Description,
                Actor = timelineEvent.Actor,
                Category = timelineEvent.Category,
                Evidence = timelineEvent.Evidence,
                MetadataJson = JsonSerializer.Serialize(timelineEvent.Metadata)
            };
        }

        private AutomatedResponse MapToAutomatedResponse(AutomatedResponseEntity entity)
        {
            return new AutomatedResponse
            {
                ResponseId = entity.ResponseId,
                Type = entity.Type,
                Actions = entity.Actions.Select(MapToAutomatedAction).ToList(),
                Trigger = entity.Trigger,
                ExecutionTime = entity.ExecutionTime,
                Status = entity.Status,
                ExecutionLog = entity.ExecutionLog
            };
        }

        private AutomatedResponseEntity MapToAutomatedResponseEntity(AutomatedResponse response)
        {
            return new AutomatedResponseEntity
            {
                ResponseId = string.IsNullOrEmpty(response.ResponseId) ? Guid.NewGuid().ToString() : response.ResponseId,
                Type = response.Type,
                Actions = response.Actions?.Select(MapToAutomatedActionEntity).ToList(),
                Trigger = response.Trigger,
                ExecutionTime = response.ExecutionTime,
                Status = response.Status,
                ExecutionLog = response.ExecutionLog
            };
        }

        private void UpdateAutomatedResponseEntity(AutomatedResponseEntity entity, AutomatedResponse response)
        {
            entity.Type = response.Type;
            entity.Trigger = response.Trigger;
            entity.ExecutionTime = response.ExecutionTime;
            entity.Status = response.Status;
            entity.ExecutionLog = response.ExecutionLog;

            // Update actions
            _context.AutomatedActions.RemoveRange(entity.Actions);
            entity.Actions = response.Actions?.Select(MapToAutomatedActionEntity).ToList();
        }

        private AutomatedAction MapToAutomatedAction(AutomatedActionEntity entity)
        {
            return new AutomatedAction
            {
                ActionId = entity.ActionId,
                Command = entity.Command,
                Parameters = JsonSerializer.Deserialize<Dictionary<string, string>>(entity.ParametersJson),
                Sequence = entity.Sequence,
                RequiresApproval = entity.RequiresApproval,
                ApprovedBy = entity.ApprovedBy,
                Status = entity.Status,
                Result = entity.Result
            };
        }

        private AutomatedActionEntity MapToAutomatedActionEntity(AutomatedAction action)
        {
            return new AutomatedActionEntity
            {
                ActionId = string.IsNullOrEmpty(action.ActionId) ? Guid.NewGuid().ToString() : action.ActionId,
                Command = action.Command,
                ParametersJson = JsonSerializer.Serialize(action.Parameters),
                Sequence = action.Sequence,
                RequiresApproval = action.RequiresApproval,
                ApprovedBy = action.ApprovedBy,
                Status = action.Status,
                Result = action.Result
            };
        }
    }
}
