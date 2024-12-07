using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using CryptAplyApp.Application.Services;
using CryptAplyApp.Application.Models;

namespace CryptAplyApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class KeyManagementController : ControllerBase
    {
        private readonly ILogger<KeyManagementController> _logger;
        private readonly KeyRotationService _keyRotationService;
        private readonly IEncryptionService _encryptionService;
        private readonly AuditLoggingService _auditLoggingService;

        public KeyManagementController(
            ILogger<KeyManagementController> logger,
            KeyRotationService keyRotationService,
            IEncryptionService encryptionService,
            AuditLoggingService auditLoggingService)
        {
            _logger = logger;
            _keyRotationService = keyRotationService;
            _encryptionService = encryptionService;
            _auditLoggingService = auditLoggingService;
        }

        [HttpPost("rotate")]
        [Authorize(Roles = "KeyAdmin")]
        public async Task<IActionResult> InitiateKeyRotation([FromBody] KeyRotationRequest request)
        {
            try
            {
                var userId = User.Identity.Name;
                await _auditLoggingService.LogActivityAsync(new AuditLogEntry
                {
                    UserId = userId,
                    Action = "InitiateKeyRotation",
                    ResourceId = request.KeyId,
                    ResourceType = "EncryptionKey",
                    Timestamp = DateTime.UtcNow,
                    Sensitivity = ActivitySensitivity.High
                });

                var result = await _keyRotationService.InitiateKeyRotationAsync(request.KeyId, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initiating key rotation for key {KeyId}", request.KeyId);
                return StatusCode(500, "An error occurred while initiating key rotation");
            }
        }

        [HttpPost("rotate/{keyId}/approve")]
        [Authorize(Roles = "KeyAdmin")]
        public async Task<IActionResult> ApproveKeyRotation(string keyId)
        {
            try
            {
                var userId = User.Identity.Name;
                await _auditLoggingService.LogActivityAsync(new AuditLogEntry
                {
                    UserId = userId,
                    Action = "ApproveKeyRotation",
                    ResourceId = keyId,
                    ResourceType = "EncryptionKey",
                    Timestamp = DateTime.UtcNow,
                    Sensitivity = ActivitySensitivity.High
                });

                var result = await _keyRotationService.ApproveRotationAsync(keyId, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving key rotation for key {KeyId}", keyId);
                return StatusCode(500, "An error occurred while approving key rotation");
            }
        }

        [HttpPost("encrypt")]
        [Authorize(Roles = "KeyUser")]
        public async Task<IActionResult> EncryptData([FromBody] EncryptionRequest request)
        {
            try
            {
                var userId = User.Identity.Name;
                await _auditLoggingService.LogActivityAsync(new AuditLogEntry
                {
                    UserId = userId,
                    Action = "EncryptData",
                    ResourceId = request.KeyId,
                    ResourceType = "EncryptionKey",
                    Timestamp = DateTime.UtcNow,
                    Sensitivity = ActivitySensitivity.Medium
                });

                var encryptedText = await _encryptionService.EncryptTextAsync(request.PlainText, request.KeyId);
                return Ok(new { EncryptedText = encryptedText });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error encrypting data with key {KeyId}", request.KeyId);
                return StatusCode(500, "An error occurred while encrypting data");
            }
        }

        [HttpPost("decrypt")]
        [Authorize(Roles = "KeyUser")]
        public async Task<IActionResult> DecryptData([FromBody] DecryptionRequest request)
        {
            try
            {
                var userId = User.Identity.Name;
                await _auditLoggingService.LogActivityAsync(new AuditLogEntry
                {
                    UserId = userId,
                    Action = "DecryptData",
                    ResourceId = request.KeyId,
                    ResourceType = "EncryptionKey",
                    Timestamp = DateTime.UtcNow,
                    Sensitivity = ActivitySensitivity.Medium
                });

                var plainText = await _encryptionService.DecryptTextAsync(request.EncryptedText, request.KeyId);
                return Ok(new { PlainText = plainText });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error decrypting data with key {KeyId}", request.KeyId);
                return StatusCode(500, "An error occurred while decrypting data");
            }
        }
    }

    public class KeyRotationRequest
    {
        public string KeyId { get; set; }
    }

    public class EncryptionRequest
    {
        public string KeyId { get; set; }
        public string PlainText { get; set; }
    }

    public class DecryptionRequest
    {
        public string KeyId { get; set; }
        public string EncryptedText { get; set; }
    }
}
