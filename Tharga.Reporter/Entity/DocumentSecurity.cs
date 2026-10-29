namespace Tharga.Reporter.Entity;

/// <summary>
/// Encrypts the produced PDF, with passwords and permissions chosen by the caller. Set it on
/// <see cref="Renderer.Security"/>; without it the PDF is not encrypted.
/// </summary>
/// <remarks>
/// At least one password is required. With only an <see cref="OwnerPassword"/>, anyone can open and read the
/// document, and the permissions limit what a reader that honours them lets the user do. A
/// <see cref="UserPassword"/> is needed to open the document at all.
/// </remarks>
public sealed class DocumentSecurity
{
    /// <summary>
    /// The encryption algorithm. The default is <see cref="DocumentEncryption.Aes128"/>.
    /// </summary>
    public DocumentEncryption Encryption { get; init; } = DocumentEncryption.Aes128;

    /// <summary>
    /// The password that gives full access, including changing the permissions.
    /// </summary>
    public string OwnerPassword { get; init; }

    /// <summary>
    /// The password needed to open the document. Leave it empty to let anyone open it.
    /// </summary>
    public string UserPassword { get; init; }

    public bool PermitPrint { get; init; } = true;
    public bool PermitFullQualityPrint { get; init; } = true;
    public bool PermitExtractContent { get; init; } = true;
    public bool PermitModifyDocument { get; init; } = true;
    public bool PermitAnnotations { get; init; } = true;
    public bool PermitFormsFill { get; init; } = true;
    public bool PermitAssembleDocument { get; init; } = true;
}

/// <summary>
/// The encryption algorithm of a PDF produced with <see cref="DocumentSecurity"/>.
/// </summary>
public enum DocumentEncryption
{
    /// <summary>
    /// AES with a 128-bit key (PDF 1.6). Read by practically every PDF reader.
    /// </summary>
    Aes128,

    /// <summary>
    /// AES with a 256-bit key (PDF 2.0). The strongest option; older readers may not open it.
    /// </summary>
    Aes256,

    /// <summary>
    /// RC4 with a 128-bit key (PDF 1.4), what every PDF was encrypted with before 3.1. RC4 is weak; use it only for
    /// readers that cannot open AES.
    /// </summary>
    Rc4With128Bits
}
