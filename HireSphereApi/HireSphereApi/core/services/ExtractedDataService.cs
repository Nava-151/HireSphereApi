using HireSphereApi.Data;
using HireSphereApi.core.entities;
using HireSphereApi.Service.Iservice;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using HireSphereApi.api.Models;
using HireSphereApi.core.DTO;
using HireSphereApi.core.DTOs;

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

        var dataList = await _context.ExtractedData.Include(e => e.Response).Include(e => e.candidate).ToListAsync();
        return _mapper.Map<IEnumerable<ExtractedDataDto>>(dataList);
    }

    public async Task<ExtractedDataDto?> GetDataById(int id)
    {
        var data = await _context.ExtractedData.FindAsync(id);
        return data != null ? _mapper.Map<ExtractedDataDto>(data) : null;
    }
    public async Task<ExtractedDataDto> AddMark(decimal mark, int userId)
    {
        var extractedData = await _context.ExtractedData.FindAsync(userId);
        Console.WriteLine(extractedData);
        if (extractedData == null)
        {
            return null;
        }
        extractedData.Mark = mark;
        await _context.SaveChangesAsync();
        return _mapper.Map<ExtractedDataDto>(extractedData);

    }
    public async Task<ExtractedDataDto> CreateData(ExtractedDataPostModel dataModel)
    {
        var dataEntity = _mapper.Map<ExtractedDataEntity>(dataModel);
        _context.ExtractedData.Add(dataEntity);
        await _context.SaveChangesAsync();

        var loadedEntity = await _context.ExtractedData
       .Include(e => e.Response)
       .Include(e => e.candidate)
       .FirstOrDefaultAsync(e => e.Id == dataEntity.Id);

        return _mapper.Map<ExtractedDataDto>(loadedEntity);
    }
   

    public async Task<ExtractedDataDto?> UpdateData(int id, ExtractedDataPostModel updatedData)
    {
        var existingData = await _context.ExtractedData
            .Include(e => e.Response)
            .Include(e => e.candidate)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (existingData == null) return null;

        _mapper.Map(updatedData, existingData);
        existingData.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return _mapper.Map<ExtractedDataDto>(existingData);
    }


    public async Task<bool> DeleteData(int id)
    {
        var data = await _context.ExtractedData.FindAsync(id);
        if (data == null) return false;

        _context.ExtractedData.Remove(data);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<ExtractedDataDto>> GetFilteredReports(AiResponseDto filterParams)
    {
        var query = _context.ExtractedData.AsQueryable();

        if (
            !filterParams.Experience.HasValue &&
            string.IsNullOrWhiteSpace(filterParams.Languages) &&
            string.IsNullOrWhiteSpace(filterParams.EnglishLevel))
        {
            return _mapper.Map<IEnumerable<ExtractedDataDto>>(query).ToList();

        }

        if (!string.IsNullOrWhiteSpace(filterParams.Education))
        {
            Console.WriteLine("education is not empty");
            query = query.Where(r => r.Response.Education.Contains(filterParams.Education));
        }

        if (filterParams.Experience.HasValue)
        {
            Console.WriteLine("experience is not empty"+filterParams.Experience.Value);
            query = query.Where(r => r.Response.Experience >= filterParams.Experience.Value);
        }

        if (!string.IsNullOrWhiteSpace(filterParams.EnglishLevel))
        {
            Console.WriteLine("englishLevel is not empty");
            query = query.Where(r => r.Response.EnglishLevel == filterParams.EnglishLevel);
        }

        if (!string.IsNullOrWhiteSpace(filterParams.Languages))
        {
            Console.WriteLine("languages is not empty");


            var languagesArray = filterParams.Languages.Split(',')
                .Select(l => l.Trim())
                .ToList();
            query = query.Where(r => languagesArray.Any(lang => r.Response.Languages.Contains(lang)));
        }
        if (filterParams.Mark.HasValue)
        {
            Console.WriteLine("mark is not empty" + filterParams.Mark.Value);
            query = query.Where(r => r.Mark >= filterParams.Mark.Value);
        }
        return _mapper.Map<IEnumerable<ExtractedDataDto>>(query).ToList();
    }

    
}

