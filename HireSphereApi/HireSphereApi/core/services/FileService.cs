using HireSphereApi.Data;
using HireSphereApi.entities;
using HireSphereApi.Service.Iservice;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using HireSphereApi.core.DTO;

public class FileService : IFileService
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public FileService(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<FileDto>> GetAllFiles()
    {
        var files = await _context.Files.ToListAsync();
        return _mapper.Map<IEnumerable<FileDto>>(files);
    }

    public async Task<FileDto?> GetFileById(int id)
    {
        var file = await _context.Files.FindAsync(id);
        return file != null ? _mapper.Map<FileDto>(file) : null;
    }

    public async Task<FileDto> UploadFile(FilesPostModel fileModel)
    {
        var fileEntity = _mapper.Map<FileEntity>(fileModel);
        _context.Files.Add(fileEntity);
        await _context.SaveChangesAsync();
        return _mapper.Map<FileDto>(fileEntity);
    }

    public async Task<bool> DeleteFile(int id)
    {
        var file = await _context.Files.FindAsync(id);
        if (file == null) return false;

        _context.Files.Remove(file);
        await _context.SaveChangesAsync();
        return true;
    }
}
