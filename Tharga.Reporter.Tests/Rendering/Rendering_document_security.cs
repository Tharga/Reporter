using System.Text;
using AwesomeAssertions;
using PdfSharp;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using Tharga.Reporter.Entity;
using Tharga.Reporter.Entity.Element;
using Xunit;

namespace Tharga.Reporter.Tests.Rendering;

public class Rendering_document_security
{
    private const string OwnerPassword = "owner-secret";
    private const string UserPassword = "user-secret";

    [Fact]
    public void A_document_is_not_encrypted_unless_security_is_asked_for()
    {
        //Act
        var pdf = new Renderer(CreateTemplate()).GetPdfBinary(PageSize.A4);

        //Assert
        ContainsEncryptDictionary(pdf).Should().BeFalse();
        using var document = PdfReader.Open(new MemoryStream(pdf), PdfDocumentOpenMode.Modify);
        document.PageCount.Should().Be(1);
    }

    [Fact]
    public void A_document_with_an_owner_password_opens_for_reading_without_it_and_for_changes_only_with_it()
    {
        //Arrange
        var renderer = new Renderer(CreateTemplate()) { Security = new DocumentSecurity { OwnerPassword = OwnerPassword, PermitModifyDocument = false } };

        //Act
        var pdf = renderer.GetPdfBinary(PageSize.A4);

        //Assert
        ContainsEncryptDictionary(pdf).Should().BeTrue();
        using (var reading = PdfReader.Open(new MemoryStream(pdf), PdfDocumentOpenMode.Import))
        {
            reading.PageCount.Should().Be(1);
            reading.SecuritySettings.PermitModifyDocument.Should().BeFalse();
            reading.SecuritySettings.PermitPrint.Should().BeTrue();
        }

        var withoutPassword = () => PdfReader.Open(new MemoryStream(pdf), PdfDocumentOpenMode.Modify);
        withoutPassword.Should().Throw<Exception>();

        using var withPassword = PdfReader.Open(new MemoryStream(pdf), OwnerPassword, PdfDocumentOpenMode.Modify);
        withPassword.PageCount.Should().Be(1);
    }

    [Fact]
    public void A_document_with_a_user_password_cannot_be_opened_without_a_password()
    {
        //Arrange
        var renderer = new Renderer(CreateTemplate()) { Security = new DocumentSecurity { OwnerPassword = OwnerPassword, UserPassword = UserPassword } };

        //Act
        var pdf = renderer.GetPdfBinary(PageSize.A4);

        //Assert
        var withoutPassword = () => PdfReader.Open(new MemoryStream(pdf), PdfDocumentOpenMode.Import);
        withoutPassword.Should().Throw<Exception>();

        using var withPassword = PdfReader.Open(new MemoryStream(pdf), UserPassword, PdfDocumentOpenMode.Import);
        withPassword.PageCount.Should().Be(1);
    }

    [Theory]
    [InlineData(DocumentEncryption.Aes128, "/AESV2")]
    [InlineData(DocumentEncryption.Aes256, "/AESV3")]
    [InlineData(DocumentEncryption.Rc4With128Bits, "/V 2")]
    public void The_chosen_encryption_is_used(DocumentEncryption encryption, string expectedMarker)
    {
        //Arrange
        var renderer = new Renderer(CreateTemplate()) { Security = new DocumentSecurity { OwnerPassword = OwnerPassword, Encryption = encryption } };

        //Act
        var pdf = renderer.GetPdfBinary(PageSize.A4);

        //Assert
        Encoding.Latin1.GetString(pdf).Should().Contain(expectedMarker);
        using var document = PdfReader.Open(new MemoryStream(pdf), OwnerPassword, PdfDocumentOpenMode.Modify);
        document.PageCount.Should().Be(1);
    }

    [Fact]
    public void Aes_128_is_the_default_encryption()
    {
        //Act
        var security = new DocumentSecurity();

        //Assert
        security.Encryption.Should().Be(DocumentEncryption.Aes128);
    }

    [Fact]
    public void Security_without_a_password_is_rejected()
    {
        //Arrange
        var renderer = new Renderer(CreateTemplate()) { Security = new DocumentSecurity() };

        //Act
        var act = () => renderer.GetPdfBinary(PageSize.A4);

        //Assert
        act.Should().Throw<InvalidOperationException>().WithMessage("*password*");
    }

    private static Template CreateTemplate()
    {
        var section = new Section();
        section.Pane.ElementList.Add(new Text { Value = "Hello" });
        return new Template(section);
    }

    private static bool ContainsEncryptDictionary(byte[] pdf)
    {
        return Encoding.Latin1.GetString(pdf).Contains("/Encrypt");
    }
}
