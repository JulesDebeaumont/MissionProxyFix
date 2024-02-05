using MissionDevBack.Db;
using MissionDevBack.Models;

namespace MissionDevBack.Services;

public class FileStorageService
{
    private readonly IConfiguration _config;
    private readonly MissionDevContext _dbContext;

    public FileStorageService(IConfiguration config, MissionDevContext dbContext)
    {
        _config = config;
        _dbContext = dbContext;
    }

    public async Task<FileStorageServiceResponse> WriteFileToDiskAsync(FormFile formfile)
    {
        var storageResponse = new FileStorageServiceResponse();
        var filePath = Path.Combine(GetMainPathStorage(), Path.GetRandomFileName());
        using (var stream = File.Create(filePath))
        {
            await formfile.CopyToAsync(stream);
        }
        storageResponse.IsSuccess = true;
        return storageResponse;
    }

    public async Task<FileStorageServiceResponse> WriteUserFileToDiskAsync(IFormFile formfile, string userId)
    {
        var storageResponse = new FileStorageServiceResponse();
        Console.WriteLine("ICI");
        Console.WriteLine(formfile.Length);
        if (formfile.Length == 0)
        {
            storageResponse.Errors.Add("File is tempty");
            return storageResponse;
        }
        if (!UserFileOptions.PermittedExtensions.Contains(Path.GetExtension(formfile.FileName)))
        {
            storageResponse.Errors.Add("File extension is not accepted");
            return storageResponse;
        }
        if (formfile.Length > UserFileOptions.MaxFileLength)
        {
            storageResponse.Errors.Add($"File is too big ( size > {UserFileOptions.MaxFileLength})");
            return storageResponse;
        }
        var fileTempFilename = Path.GetTempFileName();
        var filePath = Path.Combine(GetMainPathStorage(), UserFileOptions.UserFileFolder, fileTempFilename);
        using (var stream = File.Create(filePath))
        {
            Console.WriteLine(stream.Length);
            await formfile.CopyToAsync(stream);
        }
        var userFile = new UserFile()
        {
            UserId = userId,
            Filename = formfile.FileName,
            TempFilename = fileTempFilename,
            CreatedAt = DateTime.Now
        };
        _dbContext.UserFiles.Add(userFile);
        await _dbContext.SaveChangesAsync();

        storageResponse.IsSuccess = true;
        return storageResponse;
    }

    public async Task<FileStorageServiceResponse> WriteProjectFileToDiskAsync(IFormFile formfile, int projectId)
    {
        var storageResponse = new FileStorageServiceResponse();
        if (formfile.Length == 0)
        {
            storageResponse.Errors.Add("File is tempty");
            return storageResponse;
        }
        if (!ProjectFileOptions.PermittedExtensions.Contains(Path.GetExtension(formfile.FileName)))
        {
            storageResponse.Errors.Add("File extension is not accepted");
            return storageResponse;
        }
        if (formfile.Length > ProjectFileOptions.MaxFileLength)
        {
            storageResponse.Errors.Add($"File is too big ( size > {ProjectFileOptions.MaxFileLength})");
            return storageResponse;
        }
        var fileTempFilename = Path.GetTempFileName();
        var filePath = Path.Combine(GetMainPathStorage(), ProjectFileOptions.ProjectFileFolder, fileTempFilename);
        using (var stream = File.Create(filePath))
        {
            await formfile.CopyToAsync(stream);
        }
        var projectFile = new ProjectFile()
        {
            ProjectId = projectId,
            Filename = formfile.FileName,
            TempFilename = fileTempFilename,
            CreatedAt = DateTime.Now
        };
        _dbContext.ProjectFiles.Add(projectFile);
        await _dbContext.SaveChangesAsync();

        storageResponse.IsSuccess = true;
        return storageResponse;
    }

    public FileStorageServiceResponse EraseFileFromDisk(string tempFilename)
    {
        var storageResponse = new FileStorageServiceResponse();
        if (File.Exists(tempFilename))
        {
            File.Delete(tempFilename);
            storageResponse.IsSuccess = true;
        }
        else
        {
            storageResponse.Errors.Add("File does not exist");
        }
        return storageResponse;
    }

    public async Task<FileStorageServiceResponse> GetFileFromDiskAsync(string tempFilename)
    {
        var storageResponse = new FileStorageServiceResponse();
        if (File.Exists(tempFilename))
        {
            var fileBytes = await File.ReadAllBytesAsync(tempFilename);
            storageResponse.IsSuccess = true;
            storageResponse.FileBytes = fileBytes;
        }
        else
        {
            storageResponse.Errors.Add("File does not exist");
        }
        return storageResponse;
    }

    private string GetMainPathStorage()
    {
        return _config["MissionDevPathMainStorage"];
    }

    private static class UserFileOptions
    {
        public readonly static string UserFileFolder = "UserFile";
        public readonly static string[] PermittedExtensions = [".pdf", ".csv", ".docx"];
        public readonly static long MaxFileLength = 5L * 1024L * 1024L; // 5Mb
    }

    private static class ProjectFileOptions
    {
        public readonly static string ProjectFileFolder = "ProjectFile";
        public readonly static string[] PermittedExtensions = [".pdf", ".csv", ".docx", ".png", ".jpeg", ".jpg"];
        public readonly static long MaxFileLength = 5L * 1024L * 1024L; // 5Mb
    }

    private static class GlobalFileOption
    {
        public readonly static long MaxFileLength = 5L * 1024L * 1024L; // 5Mb
    }
}
