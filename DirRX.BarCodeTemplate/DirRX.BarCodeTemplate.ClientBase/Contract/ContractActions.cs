using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using DirRX.BarCodeTemplate.Contract;

namespace DirRX.BarCodeTemplate.Client
{
  partial class ContractActions
  {
    public virtual void AddBarCode(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      // Проверить наличие версии.
      if (!_obj.HasVersions)
      {
        Dialogs.ShowMessage(Sungero.Docflow.OfficialDocuments.Resources.ConvertionErrorTitleBase,
                            Sungero.Docflow.OfficialDocuments.Resources.NoVersionError,
                            MessageType.Information);
        return;
      }
      
      // Формат не поддерживается.
      var versionExtension = _obj.LastVersion.BodyAssociatedApplication.Extension.ToLower();
      var versionExtensionIsSupported = DirRX.BarCodeHandler.PublicFunctions.Module.CheckIfExtensionIsSupported(versionExtension);
      if (!versionExtensionIsSupported)
      {
        Dialogs.ShowMessage(Sungero.Docflow.OfficialDocuments.Resources.ConvertionErrorTitleBase,
                            Sungero.Docflow.OfficialDocuments.Resources.ExtensionNotSupportedFormat(versionExtension),
                            MessageType.Information);
        return;
      }
      
      DirRX.BarCodeHandler.PublicFunctions.Module.Remote.ExecuteAsyncAddBarCode(_obj);
      
      // Преобразование "В процессе".
      var title = Sungero.Docflow.OfficialDocuments.Resources.ConvertionInProgress;
      var message = DirRX.BarCodeHandler.Resources.CloseDocumentAndOpenLater;
      Dialogs.ShowMessage(title, message, MessageType.Information);
    }

    public virtual bool CanAddBarCode(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return !_obj.State.IsInserted && _obj.HasVersions && !_obj.State.IsChanged &&
        _obj.AccessRights.CanUpdate() && !Locks.GetLockInfo(_obj).IsLockedByOther;
    }

  }

}