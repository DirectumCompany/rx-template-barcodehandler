using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Sungero.Core;
using DirRX.BarCodeHandler.Structures.Module;
using System.IO;
using Aspose.Pdf;

namespace DirRX.BarCodeHandler.Isolated.GeneratePublicBodyWithBarCode
{
  public class IsolatedFunctions
  {

    /// <summary>
    /// �������� ����� � Pdf ������ ���������.
    /// </summary>
    /// <param name="pdfDocumentStream">������ ��������� � ������� PDF.</param>
    /// <param name="barCodePdf">��������.</param>
    [Public]
    public virtual Stream SetStampForPdf(Stream pdfDocumentStream, Stream barCodePdf)
    {
      var pdfStamper = this.CreatePdfStamper();
      
      var stamp = new Aspose.Pdf.PdfPageStamp(barCodePdf, 1);
      stamp.XIndent = 0;
      stamp.YIndent = -50;
      stamp.Height = 120;
      stamp.Width = 180;
      
      try
      {
        pdfDocumentStream = pdfStamper.AddStampToDocument(pdfDocumentStream, stamp);
      }
      catch (Exception e)
      {
        Logger.Error("Cannot add stamp", e);
        throw new AppliedCodeException("Cannot add stamp");
      }
      return pdfDocumentStream;
    }
    
    /// <summary>
    /// ������� ��������� ������ ��� ����������� �������.
    /// </summary>
    /// <returns>��������� PdfStamper.</returns>
    public virtual PdfStamper CreatePdfStamper()
    {
      return new PdfStamper();
    }
    
  }
}