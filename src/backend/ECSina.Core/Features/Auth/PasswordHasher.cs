using System.Buffers.Binary;
using System.Security.Cryptography;
using ECSina.Db.Entities;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Options;

namespace ECSina.Core.Features.Auth;

public sealed class PasswordHasher
{
    private readonly Options _options;

    public PasswordHasher(IOptions<Options>? options)
    {
        _options = options?.Value ?? new Options();
    }

    public byte[] GenerateHash(string password)
    {
        int saltSize = _options.SaltSize;

        var salt = new byte[saltSize];

        _options.Rng.GetBytes(salt);

        var subkey = KeyDerivation.Pbkdf2(
            password,
            salt,
            KeyDerivationPrf.HMACSHA512,
            _options.IterCount,
            256 / 8
        );

        var result = new byte[13 + salt.Length + subkey.Length];

        // Write header
        result[0] = 0x01; // format marker
        BinaryPrimitives.WriteUInt32BigEndian(result.AsSpan(1), (uint)KeyDerivationPrf.HMACSHA512);
        BinaryPrimitives.WriteUInt32BigEndian(result.AsSpan(5), (uint)_options.IterCount);
        BinaryPrimitives.WriteUInt32BigEndian(result.AsSpan(9), (uint)saltSize);

        salt.CopyTo(result.AsSpan(13));
        subkey.CopyTo(result.AsSpan(13 + saltSize));

        return result;
    }

    public bool VerifyHashedPassword(
        DataEntity entity,
        scoped Span<byte> hashedPassword,
        string providedPassword
    )
    {
        // read the format marker from the hashed password
        if (hashedPassword is not [0x01, ..])
            return false;

        try
        {
            // Read header information
            var prf = (KeyDerivationPrf)BinaryPrimitives.ReadUInt32BigEndian(hashedPassword[1..]);
            var iterCount = (int)BinaryPrimitives.ReadUInt32BigEndian(hashedPassword[5..]);
            var saltLength = (int)BinaryPrimitives.ReadUInt32BigEndian(hashedPassword[9..]);

            // if the salt is bigger than or equal to
            // the hash after the header info
            // then the hash is corrupted
            if (saltLength >= hashedPassword.Length - 13)
                return false;

            var salt = hashedPassword.Slice(13, saltLength).ToArray();
            var expectedSubkey = hashedPassword.Slice(13 + saltLength).ToArray();

            // Hash the incoming password and verify it
            var actualSubkey = KeyDerivation.Pbkdf2(
                providedPassword,
                salt,
                prf,
                iterCount,
                expectedSubkey.Length
            );
            return CryptographicOperations.FixedTimeEquals(actualSubkey, expectedSubkey);
        }
        catch
        {
            // This should never occur except in the case of a malformed payload, where
            // we might go off the end of the array. Regardless, a malformed payload
            // implies verification failed.
            return false;
        }
    }

    public sealed class Options
    {
        public int IterCount { get; set; } = 1000;
        public int SaltSize { get; set; } = 128 / 8;

        public RandomNumberGenerator Rng
        {
            get => _rng ??= RandomNumberGenerator.Create();
            set => _rng = value;
        }
        private RandomNumberGenerator? _rng;
    }
}
