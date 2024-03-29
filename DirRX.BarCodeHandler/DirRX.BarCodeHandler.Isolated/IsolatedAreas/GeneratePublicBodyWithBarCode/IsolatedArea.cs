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
  // TODO: Скопирована логика стандартного класса из модуля docflow, используется до исправления замечания.
  /// <summary>
  /// Класс простановки штампов в pdf. Реализует логику генерации и простановки штампов, а также поиска мест для вставки штампов.
  /// </summary>
  public class PdfStamper
  {
    
    /// <summary>
    /// Минимальная совместимая версия PDF для корректного отображения отметки.
    /// </summary>
    public const string MinCompatibleVersion = "1.4.0.0";
    
    
    /// <summary>
    /// Добавить отметку на страницу документа.
    /// </summary>
    /// <param name="inputStream">Поток с входным документом.</param>
    /// <param name="pageNumber">Номер страницы документа, на которую нужно проставить отметку.</param>
    /// <param name="stamp">Отметка.</param>
    /// <returns>Страница документа с отметкой.</returns>
    public virtual Stream AddStampToDocument(Stream inputStream, Aspose.Pdf.PdfPageStamp stamp)
    {
      try
      {
        // Создание нового потока, в который будет записан документ с отметкой (во входной поток записывать нельзя).
        var outputStream = new MemoryStream();
        var document = new Aspose.Pdf.Document(inputStream);
        // Поднимаем версию и переполучаем документ из потока,
        // чтобы гарантировать читаемость штампа после вставки.
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
    /// Для документов версии ниже 1.4 поднять версию до 1.4 перед вставкой отметки.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <returns>PDF документ, сконвертированный до версии 1.4, или исходный, если версию поднимать не требовалось.</returns>
    /// <remarks>При вставке отметки в pdf версии ниже, чем 1.4, портятся шрифты в документе.
    /// В Adobe Reader такие документы либо не открываются совсем, либо отображаются некорректно.
    /// Для корректного отображения отметки pdf-документ будет сконвертирован до версии pdf 1.4.
    /// Документы в формате pdf/a не конвертируем, т.к. формат основан на версии pdf 1.4 и не требует конвертации.</remarks>
    public Stream GetUpgradedPdf(Aspose.Pdf.Document document)
    {
      if (!document.IsPdfaCompliant)
      {
        // Получить версию стандарта PDF из свойств документа. Достаточно первых двух чисел, разделённых точкой.
        var versionRegex = new Regex(@"^\d{1,2}\.\d{1,2}");
        var pdfVersionAsString = versionRegex.Match(document.Version).Value;
        var minCompatibleVersion = Version.Parse(MinCompatibleVersion);

        if (Version.TryParse(pdfVersionAsString, out Version version) && version < minCompatibleVersion)
          document.Convert(new Aspose.Pdf.PdfFormatConversionOptions(Aspose.Pdf.PdfFormat.v_1_4));
      }
      // Необходимо пересохранить документ в поток, чтобы изменение версии применилось до простановки отметки, а не после.
      var docStream = new MemoryStream();
      document.Save(docStream);
      return docStream;
    }
  }
  
}