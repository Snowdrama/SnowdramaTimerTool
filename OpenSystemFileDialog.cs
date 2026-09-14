using Godot;
using System;

public partial class OpenSystemFileDialog : Node
{
    private static Action<(string contents, string fileName, string filePath)> fileStringCallback;
    private static void OpenFileDialog_Confirmed()
    {
        fileStringCallback = null;
        Debug.Pink($"Closing Menu!");
        fd.QueueFree();
        fd = null;
    }
    private static void OpenFileDialog_Canceled()
    {
        fileStringCallback = null;
        Debug.Pink($"Closing Menu!");
        fd.QueueFree();
    }

    //private static void OpenFileDialog_DirSelected(string dir)
    //{
    //    throw new NotImplementedException();
    //}

    //private static void OpenFileDialog_FilesSelected(string[] paths)
    //{
    //    throw new NotImplementedException();
    //}

    //private static void OpenFileDialog_GetFilePath(string path)
    //{
    //    //split file name from path 
    //    var fif = new System.IO.FileInfo(path);
    //    var fileName = $"{fif.Name}";
    //    Debug.Log($"File Name: {fileName}");
    //    var filePath = $"{fif.DirectoryName}";
    //    Debug.Log($"File Path: {filePath}");


    //    fileStringCallback?.Invoke((path, fileName, filePath));
    //    fileStringCallback = null;
    //}

    private static void OpenFileDialog_FileSelectedCallback_GetText(string path)
    {
        using (var file = FileAccess.Open(path, FileAccess.ModeFlags.Read))
        {
            //split file name from path 
            var fif = new System.IO.FileInfo(path);
            var fileName = $"{fif.Name}";
            Debug.Log($"File Name: {fileName}");
            var filePath = $"{fif.DirectoryName}";
            Debug.Log($"File Path: {filePath}");
            if (file != null)
            {
                fileStringCallback?.Invoke((file.GetAsText(), fileName, filePath));
            }
            fileStringCallback = null;

            file.Close();
        }
        Debug.Pink($"Closing Menu!");
        fd.QueueFree();
    }

    private static FileDialog fd = null;
    public static void GetFileStringContents(Action<(string, string, string)> callback, string directory = "user://", string file = null, string[] filters = null)
    {
        if (IsInstanceValid(fd))
        {
            Debug.LogError($"File Menu already open");
            return;
        }

        //ensure the directory we request exists
        if (!DirAccess.DirExistsAbsolute(directory))
        {
            Debug.LogWarning($"Directory {directory} doesn't exist, Creating! DirAccess.DirExistsAbsolute: {DirAccess.DirExistsAbsolute(directory)} ");
            DirAccess.MakeDirRecursiveAbsolute(directory);
        }

        //set callback
        fileStringCallback = callback;

        fd = new FileDialog
        {
            Access = FileDialog.AccessEnum.Filesystem,
            FileMode = FileDialog.FileModeEnum.OpenFile,
            CurrentDir = directory,
            CurrentFile = file,
            UseNativeDialog = true,
            Filters = filters
        };

        //callbacks
        fd.FileSelected += OpenFileDialog_FileSelectedCallback_GetText;
        fd.Confirmed += OpenFileDialog_Confirmed;
        fd.Canceled += OpenFileDialog_Canceled;
        fd.Popup();
    }

    //public static void GetFilePath(Action<(string, string, string)> callback, string[] filters = null)
    //{
    //    fileStringCallback = callback;
    //    var fd = new FileDialog();
    //    fd.FileMode = FileDialog.FileModeEnum.OpenFile;
    //    fd.Access = FileDialog.AccessEnum.Resources & FileDialog.AccessEnum.Userdata & FileDialog.AccessEnum.Filesystem;
    //    fd.FileSelected += OpenFileDialog_GetFilePath;

    //    //always clear callback if canceled or confirmed
    //    fd.Confirmed += OpenFileDialog_Confirmed;
    //    fd.Canceled += OpenFileDialog_Canceled;

    //    fd.UseNativeDialog = true;
    //    if (filters != null)
    //    {
    //        fd.Filters = filters;
    //    }
    //    fd.Popup();
    //}

    //public static void GetFileSavePath(Action<(string, string, string)> callback, string[] filters = null)
    //{
    //    fileStringCallback = callback;
    //    var fd = new FileDialog();
    //    fd.Access = FileDialog.AccessEnum.Resources & FileDialog.AccessEnum.Userdata & FileDialog.AccessEnum.Filesystem;

    //    fd.FileSelected += OpenFileDialog_GetFilePath;

    //    //always clear callback if canceled or confirmed
    //    fd.Confirmed += OpenFileDialog_Confirmed;
    //    fd.Canceled += OpenFileDialog_Canceled;

    //    fd.FileMode = FileDialog.FileModeEnum.SaveFile;
    //    fd.UseNativeDialog = true;
    //    if (filters != null)
    //    {
    //        fd.Filters = filters;
    //    }
    //    fd.Popup();
    //}
    public static void SaveTextFileDialog(string fileContents, string directory = "user://", string file = null, string[] filters = null)
    {
        if (IsInstanceValid(fd))
        {
            Debug.LogError($"File Menu already open");
            return;
        }
        //ensure the folder exists
        if (!DirAccess.DirExistsAbsolute(directory))
        {
            Debug.LogWarning($"Directory {directory} doesn't exist, Creating! DirAccess.DirExistsAbsolute: {DirAccess.DirExistsAbsolute(directory)} ");
            DirAccess.MakeDirRecursiveAbsolute(directory);
        }

        //create a file dialog
        fd = new FileDialog
        {
            Access = FileDialog.AccessEnum.Filesystem,
            FileMode = FileDialog.FileModeEnum.SaveFile,
            CurrentDir = directory,
            CurrentFile = file,
            UseNativeDialog = true,
            Filters = filters
        };

        fd.Confirmed += OpenFileDialog_Confirmed;
        fd.Canceled += OpenFileDialog_Canceled;
        fd.FileSelected += (string path) =>
        {
            using (var file = FileAccess.Open(path, FileAccess.ModeFlags.WriteRead))
            {
                file.StoreString(fileContents);
                file.Close();
            }
            Debug.Pink($"Closing Menu!");
            fd.QueueFree();
        };

        fd.Popup();
    }
}
