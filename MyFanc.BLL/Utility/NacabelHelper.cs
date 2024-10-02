
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using MyFanc.BusinessObjects;
using MyFanc.Contracts.BLL;
using MyFanc.Contracts.DAL;
using MyFanc.DTO.Internal.Data;
using MyFanc.DTO.Internal.Wizards;
using System.Linq;

namespace MyFanc.BLL.Utility
{
    public class NacabelHelper : INacabelHelper
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Nacabel> _nacabelRepository;
        private readonly IGenericRepository<NacabelsEntityMap> _nacabelEntityMapRepository;
        private readonly IMapper _mapper;

        public NacabelHelper(IUnitOfWork unitOfWork, IMapper mapper) 
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _nacabelRepository = unitOfWork.GetGenericRepository<Nacabel>();
            _nacabelEntityMapRepository = unitOfWork.GetGenericRepository<NacabelsEntityMap>();
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        
        public IEnumerable<string> GetDescription(IEnumerable<string> nacabelCode, string languageCode)
        {
            if(nacabelCode != null && !string.IsNullOrEmpty(languageCode))
            {
                var nacabel = _nacabelRepository.Find(n => nacabelCode.Contains(n.NacabelCode))
                    .Include(n => n.NacabelTranslation.Where(t => t.LanguageCode == languageCode))
                    .ToList();
                if (nacabel.Any())
                {
                    return nacabel.SelectMany(n => n.NacabelTranslation).Select(t => t.Description).ToList();
                }
            }
            return new List<string>();
        }

        public IEnumerable<SectorDTO> GetSectors(string languageCode, string nacabelCode, int pageSize)
        {
            if (!string.IsNullOrEmpty(languageCode))
            {
                var sectors = _nacabelRepository.Find(n => !n.DeletedTime.HasValue && (string.IsNullOrEmpty(nacabelCode) || n.NacabelTranslation.Any(t => t.LanguageCode == languageCode && t.Description.Contains(nacabelCode))))
                    .Include(n => n.NacabelTranslation.Where(t => t.LanguageCode == languageCode))
                    .OrderBy(n => n.NacabelTranslation.Where(t => t.LanguageCode == languageCode).Select(t => t.Description).First())
                    .Take(pageSize);
                return _mapper.Map<List<SectorDTO>>(sectors.ToList());
            }
            return new List<SectorDTO>();
        }

        public async Task InsertOrUpdateNacabelSector(IEnumerable<int> sectors, string enterpriseCBENumber)
        {
            var oldData = _nacabelEntityMapRepository.Find(n => n.CbeNumber == enterpriseCBENumber).ToList();
            var deletedData = oldData.Where(o => !sectors.Contains(o.NacabelId)).ToList();
            foreach (var toDelete in deletedData) { 
                _nacabelEntityMapRepository.Delete(toDelete);
                oldData.Remove(toDelete);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            }
            foreach (var item in sectors)
            {
                if (!oldData.Any(o => o.NacabelId == item))
                {
                    var nacabelsEntityMap = new NacabelsEntityMap();
                    nacabelsEntityMap.NacabelId = item;
                    nacabelsEntityMap.CbeNumber = enterpriseCBENumber;
                    nacabelsEntityMap.CreationTime = DateTime.Now;
                    _nacabelEntityMapRepository.Add(nacabelsEntityMap);
                    await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                }
            }
        }

        public IEnumerable<NacabelsCodeDTO> GetMappedSectors(IEnumerable<string> nacabelCodes, string languageCode, string cbeNumber)
        {
            languageCode = languageCode ?? "en";
            // get yang dari nacebels dan yang sudah ada di mapping
            var result = new List<NacabelsCodeDTO>();
            if (!string.IsNullOrEmpty(languageCode))
            {
                var sectors = _nacabelRepository.Find(n => nacabelCodes.Contains(n.NacabelCode))
                    .Include(n => n.NacabelTranslation.Where(t => t.LanguageCode == languageCode))
                    .OrderBy(n => n.NacabelTranslation.Where(t => t.LanguageCode == languageCode).Select(t => t.Description).First()).ToList();
                if (sectors.Any())
                    result.AddRange(_mapper.Map<List<NacabelsCodeDTO>>(sectors));

                var sectorsEntityMap = _nacabelEntityMapRepository
                    .Find(n => !sectors.Select(x=>x.Id).Contains(n.NacabelId) && n.CbeNumber == cbeNumber)
                    .Include(m => m.Nacabel).ThenInclude(n => n.NacabelTranslation.Where(t => t.LanguageCode == languageCode))
                    .ToList();

                if(sectorsEntityMap.Any())
                    result.AddRange(_mapper.Map<List<NacabelsCodeDTO>>(sectorsEntityMap));

                return result;
            }
            return new List<NacabelsCodeDTO>();
        }
    }
}
