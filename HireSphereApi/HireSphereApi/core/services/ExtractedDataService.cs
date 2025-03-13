using HireSphereApi.Data;
using HireSphereApi.core.entities;
using HireSphereApi.Service.Iservice;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using HireSphereApi.api.Models;
using HireSphereApi.core.DTO;

public class ExtractedDataService : IExtractedDataService
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public ExtractedDataService(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ExtractedDataDto>> GetAllData()
    {
        var dataList = await _context.ExtractedData.ToListAsync();
        return _mapper.Map<IEnumerable<ExtractedDataDto>>(dataList);
    }

    public async Task<ExtractedDataDto?> GetDataById(int id)
    {
        var data = await _context.ExtractedData.FindAsync(id);
        return data != null ? _mapper.Map<ExtractedDataDto>(data) : null;
    }

    public async Task<ExtractedDataDto> CreateData(ExtractedDataPostModel dataModel)
    {
        var dataEntity = _mapper.Map<ExtractedDataEntity>(dataModel);
        _context.ExtractedData.Add(dataEntity);
        await _context.SaveChangesAsync();
        return _mapper.Map<ExtractedDataDto>(dataEntity);
    }

    public async Task<bool> UpdateData(int id, ExtractedDataPostModel updatedData)
    {
        var existingData = await _context.ExtractedData.FindAsync(id);
        if (existingData == null) return false;

        _mapper.Map(updatedData, existingData);
        existingData.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteData(int id)
    {
        var data = await _context.ExtractedData.FindAsync(id);
        if (data == null) return false;

        _context.ExtractedData.Remove(data);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<ExtractedDataDto>> IExtractedDataService.GetFilteredReports(AIResponse filterParams)
    {
        var query = _context.ExtractedData.AsQueryable();

        // ✅ Return all reports if no filter is provided
        if (string.IsNullOrWhiteSpace(filterParams.Title) &&
            string.IsNullOrWhiteSpace(filterParams.Description) &&
            !filterParams.Experience.HasValue &&
            string.IsNullOrWhiteSpace(filterParams.WorkPlace) &&
            string.IsNullOrWhiteSpace(filterParams.Languages) &&
            !filterParams.RemoteWork.HasValue &&
            string.IsNullOrWhiteSpace(filterParams.EnglishLevel))
        {
            return await query.ToListAsync();
        }

        // Filtering logic
        if (!string.IsNullOrWhiteSpace(filterParams.Title))
            query = query.Where(r => r.Title.Contains(filterParams.Title));

        if (!string.IsNullOrWhiteSpace(filterParams.Description))
            query = query.Where(r => r.Description.Contains(filterParams.Description));

        if (filterParams.Experience.HasValue)
            query = query.Where(r => r.Experience == filterParams.Experience.Value);

        if (!string.IsNullOrWhiteSpace(filterParams.WorkPlace))
            query = query.Where(r => r.WorkPlace == filterParams.WorkPlace);

        if (!string.IsNullOrWhiteSpace(filterParams.EnglishLevel))
            query = query.Where(r => r.EnglishLevel == filterParams.EnglishLevel);

        if (filterParams.RemoteWork.HasValue)
            query = query.Where(r => r.RemoteWork == filterParams.RemoteWork.Value);

        // ✅ Filtering for multiple languages
        if (!string.IsNullOrWhiteSpace(filterParams.Languages))
        {
            var languagesArray = filterParams.Languages.Split(',')
                .Select(l => l.Trim())
                .ToList();
            query = query.Where(r => languagesArray.Any(lang => r.Languages.Contains(lang)));
        }

        return await query.ToListAsync();
    }
}

