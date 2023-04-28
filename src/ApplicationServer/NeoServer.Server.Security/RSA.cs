using System.IO;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.OpenSsl;

namespace NeoServer.Server.Security;

public static class Rsa
{
    private static RsaEngine RsaEngine { get; set; }

    public static byte[] Decrypt(byte[] data)
    {
        return RsaEngine.ProcessBlock(data, 0, data.Length);
    }

    public static void LoadPem(string basePath)
    {
        using var reader = File.OpenText(@$"{basePath}/key.pem");
        var keyPair = (AsymmetricCipherKeyPair)new PemReader(reader).ReadObject();

        RsaEngine = new RsaEngine();
        RsaEngine.Init(false, keyPair.Private);
    }
}