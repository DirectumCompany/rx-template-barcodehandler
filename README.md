# rx-template-barcodehandler
Репозиторий с шаблоном разработки «Вставка штрихкодов с конвертацией в PDF».

## Описание
Решение производит конвертацию документа в pdf и вставку штрихкода в формате Code128 , на каждую страницу последней версии документа. Вставка производится в версии в форматах doc, docx и pdf.

## Состав объектов разработки:
1.	Решение BarCode, включающее асинхронный обработчик по вставке штрихкодов и конвертации в pdf. 
Поскольку решение BarCode  не содержит перекрытий объектов коробочного решения, конфликты при публикации не возникнут. Это позволяет использовать функциональность, как при старте нового проекта, так и в ходе сопровождения существующих инсталляций системы. 
2.	Решение BarCodeTemplate, включающее вызов вставки штрихкода по действию на ленте в карточке Договора.  Служит в качестве примера вызова асинхронного обработчика, не предназначено для публикации в рамках проектов.

## Варианты расширения функциональности на проектах
1.	Реализация вызова асинхронного обработчика на различных событиях в зависимости от бизнес-кейсов. Например, в рамках согласования перед этапом печать последняя версия конвертируется в pdf с простановкой штрихкода. 
2.	Изменение координат проставления штрихкода.
3.	Изменение формата штрихкода, в т.ч. на QR-code.
4.	Изменение состава страниц, на которые устанавливается штрихкод.
5.	Добавление новых форматов документов.

## Порядок установки
Для работы требуется установленный Directum RX версии 3.5 и выше.

## Установка для ознакомления
1. Склонировать репозиторий с rx-template-barcodehandler в папку.
2. Указать в _ConfigSettings.xml DDS:
<block name="REPOSITORIES">
  <repository folderName="Base" solutionType="Base" url="" /> 
  <repository folderName="<Папка из п.1>" solutionType="Work" 
     url="https://github.com/DirectumCompany/rx-template-barcodehandler " />
</block>

## Установка для использования на проекте
Возможные варианты

### A. Fork репозитория.
1. Сделать fork репозитория rx-template-barcodehandler для своей учетной записи.
2. Склонировать созданный в п. 1 репозиторий в папку.
3. Указать в _ConfigSettings.xml DDS:
<block name="REPOSITORIES">
  <repository folderName="Base" solutionType="Base" url="" /> 
  <repository folderName="<Папка из п.2>" solutionType="Work" 
     url="<Адрес репозитория gitHub учетной записи пользователя из п. 1>" />
</block>

### B. Подключение на базовый слой.
Вариант не рекомендуется, так как при выходе версии шаблона разработки не гарантируется обратная совместимость.
1. Склонировать репозиторий rx-template-barcodehandler в папку.
2. Указать в _ConfigSettings.xml DDS:
<block name="REPOSITORIES">
  <repository folderName="Base" solutionType="Base" url="" /> 
  <repository folderName="<Папка из п.1>" solutionType="Base" 
     url="https://github.com/DirectumCompany/rx-template-barcodehandler " />
  <repository folderName="<Папка для рабочего слоя>" solutionType="Work" 
     url ="<Адрес репозитория для рабочего слоя>" />
</block>

### C. Копирование репозитория в систему контроля версий.
Рекомендуемый вариант для проектов внедрения.
1. В системе контроля версий с поддержкой git создать новый репозиторий.
2. Склонировать репозиторий rx-template-barcodehandler в папку с ключом --mirror.
3. Перейти в папку из п. 2.
4. Импортировать клонированный репозиторий в систему контроля версий командой:
git push –mirror <Адрес репозитория из п. 1>
