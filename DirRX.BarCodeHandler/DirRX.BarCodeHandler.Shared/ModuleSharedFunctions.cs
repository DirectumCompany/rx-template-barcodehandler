using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities; 

namespace DirRX.BarCodeHandler.Shared
{
  public class ModuleFunctions
  {    
    /// <summary>
    /// Проверить, поддерживается ли формат файла по его расширению.
    /// </summary>
    /// <param name="extension">Расширение файла.</param>
    /// <returns>True/false.</returns>
    [Public]
    public static bool CheckIfExtensionIsSupported(string extension)
    {
      var supportedFormatsList = new List<string>() { "pdf", "docx", "doc" };

      return supportedFormatsList.Contains(extension.ToLower());
    }
  }
}