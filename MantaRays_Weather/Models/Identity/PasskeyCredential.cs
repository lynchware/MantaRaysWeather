using System;

namespace MantaRays_Weather.Models.Identity
{
    public class PasskeyCredential
    {
        public int Id { get; set; }

        // Reference to AspNetUsers Id (IdentityUser.Id is string)
        public string UserId { get; set; } = null!;

        // Stored as base64 or binary depending on provider; use string for simplicity
        public string CredentialId { get; set; } = null!;

        // The public key (COSE or base64) used to verify signatures
        public string PublicKey { get; set; } = null!;

        // Signature counter
        public long SignCount { get; set; }

        // JSON blob with descriptor/attestation metadata
        public string DescriptorJson { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
