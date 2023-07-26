using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using Sungero.Core;
using DirRX.BarCodeHandler.Structures.Module;
using System.Text.RegularExpressions;

namespace DirRX.BarCodeHandler.Isolated.GeneratePublicBodyWithBarCode
{
  
  /// <summary>
  /// ����� ����������� ������� � pdf. ��������� ������ ��������� � ����������� �������, � ����� ������ ���� ��� ������� �������.
  /// </summary>
  public class PdfStamper
  {
    
    /// <summary>
    /// ����������� ����������� ������ PDF ��� ����������� ����������� �������.
    /// </summary>
    public const string MinCompatibleVersion = "1.4.0.0";
    
    
    /// <summary>
    /// �������� ������� �� �������� ���������.
    /// </summary>
    /// <param name="inputStream">����� � ������� ����������.</param>
    /// <param name="pageNumber">����� �������� ���������, �� ������� ����� ���������� �������.</param>
    /// <param name="stamp">�������.</param>
    /// <returns>�������� ��������� � ��������.</returns>
    public virtual Stream AddStampToDocument(Stream inputStream, Aspose.Pdf.PdfPageStamp stamp)
    {
      try
      {
        // �������� ������ ������, � ������� ����� ������� �������� � �������� (�� ������� ����� ���������� ������).
        var outputStream = new MemoryStream();
        var document = new Aspose.Pdf.Document(inputStream);
        // ��������� ������ � ������������ �������� �� ������,
        // ����� ������������� ���������� ������ ����� �������.
        using (var documentStream = this.GetUpgradedPdf(document))
        {
          document = new Aspose.Pdf.Document(documentStream);
          foreach (var documentPage in document.Pages)
          {
            var rectConsiderRotation = documentPage.GetPageRect(true);
            documentPage.AddStamp(stamp);
            document.Save(outputStream);
          }
        }
        return outputStream;
      }
      catch (Exception ex)
      {
        Logger.Error("Cannot add stamp to document page", ex);
        throw new AppliedCodeException("Cannot add stamp to document page");
      }
      finally
      {
        inputStream.Close();
      }
    }
    
    /// <summary>
    /// ��� ���������� ������ ���� 1.4 ������� ������ �� 1.4 ����� �������� �������.
    /// </summary>
    /// <param name="document">��������.</param>
    /// <returns>PDF ��������, ����������������� �� ������ 1.4, ��� ��������, ���� ������ ��������� �� �����������.</returns>
    /// <remarks>��� ������� ������� � pdf ������ ����, ��� 1.4, �������� ������ � ���������.
    /// � Adobe Reader ����� ��������� ���� �� ����������� ������, ���� ������������ �����������.
    /// ��� ����������� ����������� ������� pdf-�������� ����� �������������� �� ������ pdf 1.4.
    /// ��������� � ������� pdf/a �� ������������, �.�. ������ ������� �� ������ pdf 1.4 � �� ������� �����������.</remarks>
    public Stream GetUpgradedPdf(Aspose.Pdf.Document document)
    {
      if (!document.IsPdfaCompliant)
      {
        // �������� ������ ��������� PDF �� ������� ���������. ���������� ������ ���� �����, ���������� ������.
        var versionRegex = new Regex(@"^\d{1,2}\.\d{1,2}");
        var pdfVersionAsString = versionRegex.Match(document.Version).Value;
        var minCompatibleVersion = Version.Parse(MinCompatibleVersion);

        if (Version.TryParse(pdfVersionAsString, out Version version) && version < minCompatibleVersion)
          document.Convert(new Aspose.Pdf.PdfFormatConversionOptions(Aspose.Pdf.PdfFormat.v_1_4));
      }
      // ���������� ������������� �������� � �����, ����� ��������� ������ ����������� �� ����������� �������, � �� �����.
      var docStream = new MemoryStream();
      document.Save(docStream);
      return docStream;
    }
  }
  
}