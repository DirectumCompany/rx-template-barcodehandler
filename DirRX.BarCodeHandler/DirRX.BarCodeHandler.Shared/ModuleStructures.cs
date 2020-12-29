using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace DirRX.BarCodeHandler.Structures.Module
{
  /// <summary>
  /// Результат преобразования документа в PDF.
  /// </summary>
  partial class СonversionToPdfResult
  {    
    public bool IsOnConvertion { get; set; }
    
    public bool HasErrors { get; set; }
    
    public bool HasConvertionError { get; set; }
    
    public bool HasLockError { get; set; }
    
    public string ErrorTitle { get; set; }
    
    public string ErrorMessage { get; set; }
  }
}