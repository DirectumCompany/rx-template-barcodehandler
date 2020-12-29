using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.AsposeExtensions;


namespace DirRX.BarCodeHandler.Server
{
  public class ModuleAsyncHandlers
  {

    public virtual void AddBarcodeToDocument(DirRX.BarCodeHandler.Server.AsyncHandlerInvokeArgs.AddBarcodeToDocumentInvokeArgs args)
    {
      int documentId = args.DocumentId;
      int versionId = args.VersionId;
      
      Logger.DebugFormat("AddBarcodeToDocument: start add barcode to pdf. Document id - {0}.", documentId);
      
      var document = Sungero.Docflow.OfficialDocuments.GetAll(x => x.Id == documentId).FirstOrDefault();
      if (document == null)
      {
        Logger.DebugFormat("AddBarcodeToDocument: not found document with id {0}.", documentId);
        return;
      }
      
      var version = document.Versions.SingleOrDefault(v => v.Id == versionId);
      if (version == null)
      {
        Logger.DebugFormat("AddBarcodeToDocument: not found version. Document id - {0}, version number - {1}.", documentId, versionId);
        return;
      }
      
      if (!Locks.TryLock(version.Body))
      {
        Logger.DebugFormat("AddBarcodeToDocument: version is locked. Document id - {0}, version number - {1}.", documentId, versionId);
        args.Retry = true;
        return;
      }
      
      var result = BarCodeHandler.Functions.Module.GeneratePublicBodyWithBarCode(document, version.Id);

      Locks.Unlock(version.Body);
      
      if (result.HasErrors)
      {
        Logger.DebugFormat("AddBarcodeToDocument: {0}", result.ErrorMessage);
        if (result.HasLockError)
        {
          args.Retry = true;
        }
        else
        {
          var operation = new Enumeration(Constants.Module.Operation.ConvertToPdf);
          document.History.Write(operation, operation, string.Empty, version.Number);
          document.Save();
        }
      }
      
      Logger.DebugFormat("AddBarcodeToDocument: convert document {0} to pdf successfully.", documentId);
    }

  }
}