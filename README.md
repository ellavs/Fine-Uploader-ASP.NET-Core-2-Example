# Fine Uploader ASP.NET Core 2 Example
Server side example (ASP.NET CORE 2) for using javascript file upload plugin - [FineUploader](https://fineuploader.com/)

This project author site - https://www.e-du.ru/

This example shows how to using fineuploader with ASP.NET CORE 2. Example controller:

```c#
[HttpPost]
[Produces("application/json")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Upload(string qquuid, string qqfilename, int qqtotalfilesize, IFormFile qqfile)
{
    int intFileDBID = 0;
    string strRealFilePath = "";
    string strCurrentPath = @"c:\tmp\";

    // original file name - qqfile.FileName
    // user file name - qqfilename

    if (qqfile.Length > 0)
    {
        var filePath = Path.Combine(strCurrentPath, qqfilename); // never do it without validation in real project!!!
        try
        {
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                //throw new ApplicationException("Some Error"); // for testing errors
                await qqfile.CopyToAsync(stream);

                // DO SOMETHING ELSE, for example save file info to database

                intFileDBID = 1; // change on file DB ID, if needed
                strRealFilePath = filePath;  // change on file real path, if needed
            }
        }
        catch (Exception ex)
        {
            // return to JS fail and error
            return Ok(new { success = false, error = ex.Message });
        }
    }

    // return to JS success and extra information about uploded file
    return Ok(new { success = true, fileid = intFileDBID, fpath = strRealFilePath, error = "" });
}
```

And also you see some customization option in js.
For example:

```js
$(document).ready(function () {
    $('#fine-uploader-manual-trigger').fineUploader({
        template: 'qq-template-manual-trigger',
        request: {
            endpoint: '/Home/Upload',
            customHeaders: {
                'X-CSRF-TOKEN': $('input:hidden[name="__RequestVerificationToken"]').val()
            },  // for token validation
            params: {
                pid: 1,
                aid: 1
            }  // any custom params about file
        },
        thumbnails: {
            placeholders: {
                waitingPath: '/lib/fineuploader/placeholders/waiting-generic.png',
                notAvailablePath: '/lib/fineuploader/placeholders/not_available-generic.png'
            }
        },
        autoUpload: false,
        multiple: false,    // for uploading ONLY ONE file
        validation: {
            allowedExtensions: ['jpeg', 'jpg', 'txt', 'png', 'msi', 'exe'],
            sizeLimit: 200512000 // in bytes
        },
        text: {
            // localize text, for example - RUSSIAN (comment this for english)
            defaultResponseError: 'Ошибка загрузки!',
            formatProgress: 'Загрузка...',
            waitingForResponse: 'Переместите файл сюда...'
        },
        messages: {
            // localize text, for example - RUSSIAN (comment this for english)
            //tooManyItemsError: "Too many items ({netItems}) would be uploaded.  Item limit is {itemLimit}.",
            tooManyFilesError: 'Слишком много файлов ({netItems}), допустим только один.'                   
        },
        showMessage: function (message) {
            // for show error in some div instead of modal message
            $('#MessageUploaderError').html(message);
        },
        dragAndDrop: {
            extraDropzones: [$(".body-content")] // set where is another drag&drop place
        }
    }).on('complete', function (event, id, fileName, responseJSON) {
        // set what to do after upload
        // alert("completed");
        if (responseJSON.success) {                    
            $('#MessageUploaderDone').html($('#MessageUploaderDone').html() + '<br>Loaded file: ' + fileName + ". Real path - " + responseJSON.fpath + ". File DB ID - " + responseJSON.fileid);
            // clear uploader if load success
            $('#fine-uploader-manual-trigger').fineUploader('clearStoredFiles');
        } else {
            $('#MessageUploaderError').html("Error - " + responseJSON.error);
        }
        }).on('submitted', function (event, fileId, fileName) {
            // set upload button when file is entered
            $('#trigger-upload').click(function () {
                $('#fine-uploader-manual-trigger').fineUploader('uploadStoredFiles');
            });
        });
})
```

