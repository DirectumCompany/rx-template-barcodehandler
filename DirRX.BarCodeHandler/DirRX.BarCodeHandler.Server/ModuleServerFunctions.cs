using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace DirRX.BarCodeHandler.Server
{
  public class ModuleFunctions
  {
    /// <summary>
    /// Вызов обработчика для вставки штрихкода в документ.
    /// </summary>
    /// <param name="document">Документ.</param>
    [Public, Remote]
    public static void ExecuteAsyncAddBarCode(Sungero.Docflow.IOfficialDocument document)
    {
      var asyncAddBarCode = DirRX.BarCodeHandler.AsyncHandlers.AddBarcodeToDocument.Create();
      asyncAddBarCode.DocumentId = document.Id;
      asyncAddBarCode.VersionId = document.LastVersion.Id;
      asyncAddBarCode.ExecuteAsync();
    }
    
    #region
    
    /// <summary>
    /// Сгенерировать PublicBody документа со вставкой штрихкода.
    /// </summary>
    /// <param name="document">Документ для преобразования.</param>
    /// <param name="versionId">Id версии, для генерации.</param>
    /// <returns>Информация о результате генерации PublicBody для версии документа.</returns>
    public virtual Structures.Module.СonversionToPdfResult GeneratePublicBodyWithBarCode(Sungero.Docflow.IOfficialDocument document, long versionId)
    {
      var info = Structures.Module.СonversionToPdfResult.Create();
      info.HasErrors = true;
      var version = document.Versions.SingleOrDefault(v => v.Id == versionId);
      if (version == null)
      {
        info.HasConvertionError = true;
        info.ErrorMessage = Sungero.Docflow.OfficialDocuments.Resources.NoVersionWithNumberErrorFormat(versionId);
        return info;
      }
      
      System.IO.Stream pdfDocumentStream = null;
      using (var inputStream = new System.IO.MemoryStream())
      {
        version.Body.Read().CopyTo(inputStream);
        try
        {
          var extension = version.BodyAssociatedApplication.Extension;
          pdfDocumentStream = Sungero.Docflow.IsolatedFunctions.PdfConverter.GeneratePdf(inputStream, extension);
          
          var barCodePdf = Sungero.Docflow.IsolatedFunctions.PdfConverter.GeneratePdf(GetBarCode(document), GetBarCodeExtension());

          pdfDocumentStream = IsolatedFunctions.GeneratePublicBodyWithBarCode.SetStampForPdf(pdfDocumentStream, barCodePdf);
        }
        catch (Exception e)
        {
          if (e is AppliedCodeException)
            Logger.Error(Sungero.Docflow.Resources.PdfConvertErrorFormat(document.Id), e.InnerException);
          else
            Logger.Error(string.Format("{0} {1}", Sungero.Docflow.Resources.PdfConvertErrorFormat(document.Id), e.Message));
          
          info.HasConvertionError = true;
          info.HasLockError = false;
          info.ErrorMessage = Sungero.Docflow.Resources.DocumentBodyNeedsRepair;
        }
      }
      
      if (!string.IsNullOrWhiteSpace(info.ErrorMessage))
        return info;
      
      version.PublicBody.Write(pdfDocumentStream);
      version.AssociatedApplication = Sungero.Content.AssociatedApplications.GetByExtension("pdf");
      pdfDocumentStream.Close();

      try
      {
        document.Save();
        info.HasErrors = false;
      }
      catch (Sungero.Domain.Shared.Exceptions.RepeatedLockException e)
      {
        Logger.Error(e.Message);
        info.HasConvertionError = false;
        info.HasLockError = true;
        info.ErrorMessage = e.Message;
      }
      catch (Exception e)
      {
        Logger.Error(e.Message);
        info.HasConvertionError = true;
        info.HasLockError = false;
        info.ErrorMessage = e.Message;
      }
      
      return info;
    }
    
    /// <summary>
    /// Сгенерировать штрихкод по документу.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <returns>Штрихкод в виде потока.</returns>
    public virtual System.IO.MemoryStream GetBarCode(Sungero.Docflow.IOfficialDocument document)
    {
      var barCode = Sungero.Content.PublicFunctions.ElectronicDocument.Remote.GetBarcode(document);
      return new System.IO.MemoryStream(barCode);
    }
    
    /// <summary>
    /// Получить расширение штрихкода, для преобразования в pdf.
    /// Поддерживаемые форматы: jpg, jpeg, png, bmp, tif, tiff, gif, html, pdf, docx, doc, rtf, xls, xlsx, odt, ods, txt.
    /// </summary>
    /// <returns>Расширение в виде строки.</returns>
    public virtual string GetBarCodeExtension()
    {
      return "png";
    }
    
    #endregion
  }
}