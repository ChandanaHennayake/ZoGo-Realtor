using zogo.Application.DTOs.Common;
using zogo.Application.Interfaces.Repositories;
using zogo.Application.Interfaces.Services;

namespace zogo.Application.Services;

public class CommonService : ICommonService
{
    private readonly ICommonRepository _commonRepository;

    private static readonly List<ProvinceResponse> StaticProvinces = new()
    {
        new ProvinceResponse { Id = 1, Code = "WP", Name = "Western" },
        new ProvinceResponse { Id = 2, Code = "CP", Name = "Central" },
        new ProvinceResponse { Id = 3, Code = "SP", Name = "Southern" },
        new ProvinceResponse { Id = 4, Code = "NP", Name = "Northern" },
        new ProvinceResponse { Id = 5, Code = "EP", Name = "Eastern" },
        new ProvinceResponse { Id = 6, Code = "NWP", Name = "North Western" },
        new ProvinceResponse { Id = 7, Code = "NCP", Name = "North Central" },
        new ProvinceResponse { Id = 8, Code = "UP", Name = "Uva" },
        new ProvinceResponse { Id = 9, Code = "SGP", Name = "Sabaragamuwa" }
    };

    private static readonly Dictionary<string, short> DistrictNameToProvinceId = new(StringComparer.OrdinalIgnoreCase)
    {
        { "Colombo", 1 }, { "Gampaha", 1 }, { "Kalutara", 1 },
        { "Kandy", 2 }, { "Matale", 2 }, { "Nuwara Eliya", 2 },
        { "Galle", 3 }, { "Matara", 3 }, { "Hambantota", 3 },
        { "Jaffna", 4 }, { "Kilinochchi", 4 }, { "Mannar", 4 }, { "Vavuniya", 4 }, { "Mullaitivu", 4 },
        { "Batticaloa", 5 }, { "Ampara", 5 }, { "Trincomalee", 5 },
        { "Kurunegala", 6 }, { "Puttalam", 6 },
        { "Anuradhapura", 7 }, { "Polonnaruwa", 7 },
        { "Badulla", 8 }, { "Monaragala", 8 },
        { "Ratnapura", 9 }, { "Kegalle", 9 }
    };

    private static readonly Dictionary<string, short> DistrictCodeToProvinceId = new(StringComparer.OrdinalIgnoreCase)
    {
        { "COL", 1 }, { "GAM", 1 }, { "KAL", 1 },
        { "KAN", 2 }, { "MTL", 2 }, { "NUW", 2 },
        { "GAL", 3 }, { "MAT", 3 }, { "HAM", 3 },
        { "JAF", 4 }, { "KIL", 4 }, { "MAN", 4 }, { "VAV", 4 }, { "MUL", 4 },
        { "BAT", 5 }, { "AMP", 5 }, { "TRI", 5 },
        { "KUR", 6 }, { "PUT", 6 },
        { "ANU", 7 }, { "POL", 7 },
        { "BAD", 8 }, { "MON", 8 },
        { "RAT", 9 }, { "KEG", 9 }
    };

    private static readonly List<DistrictResponse> StaticFallbackDistricts = new()
    {
        new DistrictResponse { Id = 1, ProvinceId = 1, Code = "COL", Name = "Colombo" },
        new DistrictResponse { Id = 2, ProvinceId = 1, Code = "GAM", Name = "Gampaha" },
        new DistrictResponse { Id = 3, ProvinceId = 1, Code = "KAL", Name = "Kalutara" },
        new DistrictResponse { Id = 4, ProvinceId = 2, Code = "KAN", Name = "Kandy" },
        new DistrictResponse { Id = 5, ProvinceId = 2, Code = "MTL", Name = "Matale" },
        new DistrictResponse { Id = 6, ProvinceId = 2, Code = "NUW", Name = "Nuwara Eliya" },
        new DistrictResponse { Id = 7, ProvinceId = 3, Code = "GAL", Name = "Galle" },
        new DistrictResponse { Id = 8, ProvinceId = 3, Code = "MAT", Name = "Matara" },
        new DistrictResponse { Id = 9, ProvinceId = 3, Code = "HAM", Name = "Hambantota" },
        new DistrictResponse { Id = 10, ProvinceId = 4, Code = "JAF", Name = "Jaffna" },
        new DistrictResponse { Id = 11, ProvinceId = 4, Code = "KIL", Name = "Kilinochchi" },
        new DistrictResponse { Id = 12, ProvinceId = 4, Code = "MAN", Name = "Mannar" },
        new DistrictResponse { Id = 13, ProvinceId = 4, Code = "VAV", Name = "Vavuniya" },
        new DistrictResponse { Id = 14, ProvinceId = 4, Code = "MUL", Name = "Mullaitivu" },
        new DistrictResponse { Id = 15, ProvinceId = 5, Code = "BAT", Name = "Batticaloa" },
        new DistrictResponse { Id = 16, ProvinceId = 5, Code = "AMP", Name = "Ampara" },
        new DistrictResponse { Id = 17, ProvinceId = 5, Code = "TRI", Name = "Trincomalee" },
        new DistrictResponse { Id = 18, ProvinceId = 6, Code = "KUR", Name = "Kurunegala" },
        new DistrictResponse { Id = 19, ProvinceId = 6, Code = "PUT", Name = "Puttalam" },
        new DistrictResponse { Id = 20, ProvinceId = 7, Code = "ANU", Name = "Anuradhapura" },
        new DistrictResponse { Id = 21, ProvinceId = 7, Code = "POL", Name = "Polonnaruwa" },
        new DistrictResponse { Id = 22, ProvinceId = 8, Code = "BAD", Name = "Badulla" },
        new DistrictResponse { Id = 23, ProvinceId = 8, Code = "MON", Name = "Monaragala" },
        new DistrictResponse { Id = 24, ProvinceId = 9, Code = "RAT", Name = "Ratnapura" },
        new DistrictResponse { Id = 25, ProvinceId = 9, Code = "KEG", Name = "Kegalle" }
    };

    private static readonly Dictionary<string, List<string>> DistrictNameToCities = new(StringComparer.OrdinalIgnoreCase)
    {
        { "Colombo", new List<string> {
            "Colombo 01 - Fort", "Colombo 02 - Slave Island", "Colombo 03 - Kollupitiya", "Colombo 04 - Bambalapitiya",
            "Colombo 05 - Havelock Town", "Colombo 06 - Wellawatte", "Colombo 07 - Cinnamon Gardens", "Colombo 08 - Borella",
            "Colombo 09 - Dematagoda", "Colombo 10 - Maradana", "Colombo 11 - Pettah", "Colombo 12 - Hultsdorf",
            "Colombo 13 - Kotahena", "Colombo 14 - Grandpass", "Colombo 15 - Mutwal", "Dehiwala", "Mount Lavinia",
            "Moratuwa", "Nugegoda", "Maharagama", "Sri Jayawardenepura Kotte", "Battaramulla", "Rajagiriya", "Malabe",
            "Thalawathugoda", "Kottawa", "Pannipitiya", "Homagama", "Piliyandala", "Kesbewa", "Boralesgamuwa",
            "Athurugiriya", "Kohuwala", "Ratmalana", "Nawala", "Angoda", "Kolonnawa", "Hanwella", "Padukka"
        }},
        { "Gampaha", new List<string> {
            "Negombo", "Gampaha", "Kelaniya", "Wattala", "Ja-Ela", "Kandana", "Ragama", "Kadawatha", "Kiribathgoda",
            "Minuwangoda", "Mirigama", "Divulapitiya", "Nittambuwa", "Veyangoda", "Yakkala", "Delgoda", "Biyagama",
            "Mahabage", "Seeduwa", "Katunayake", "Ganemulla"
        }},
        { "Kalutara", new List<string> {
            "Kalutara", "Panadura", "Horana", "Wadduwa", "Beruwala", "Aluthgama", "Matugama", "Bandaragama",
            "Ingiriya", "Agalawatta", "Dodangoda", "Bulathsinhala"
        }},
        { "Kandy", new List<string> {
            "Kandy", "Peradeniya", "Katugastota", "Kundasale", "Digana", "Gampola", "Nawalapitiya", "Akurana",
            "Gelioya", "Pilimathalawa", "Kadugannawa", "Wattegama", "Ampitiya"
        }},
        { "Matale", new List<string> {
            "Matale", "Dambulla", "Sigiriya", "Galewela", "Ukuwela", "Rattota", "Naula", "Yatawatta"
        }},
        { "Nuwara Eliya", new List<string> {
            "Nuwara Eliya", "Hatton", "Talawakelle", "Ginigathena", "Kotagala", "Maskeliya", "Ragala", "Walapane"
        }},
        { "Galle", new List<string> {
            "Galle", "Hikkaduwa", "Karapitiya", "Ambalangoda", "Bentota", "Unawatuna", "Baddegama", "Elpitiya",
            "Koggala", "Ahangama", "Batapola"
        }},
        { "Matara", new List<string> {
            "Matara", "Weligama", "Mirissa", "Dikwella", "Akuressa", "Kamburupitiya", "Hakmana", "Deniyaya", "Gandara"
        }},
        { "Hambantota", new List<string> {
            "Hambantota", "Tangalle", "Tissamaharama", "Beliatta", "Ambalantota", "Weeraketiya", "Walasmulla", "Ranna"
        }},
        { "Jaffna", new List<string> {
            "Jaffna", "Nallur", "Chavakachcheri", "Point Pedro", "Valvettithurai", "Chunnakam", "Kopay", "Tellippalai", "Karainagar"
        }},
        { "Kilinochchi", new List<string> {
            "Kilinochchi", "Pallai", "Paranthan", "Poonakary"
        }},
        { "Mannar", new List<string> {
            "Mannar", "Nanaddan", "Madhu", "Pesalai", "Thalaimannar"
        }},
        { "Vavuniya", new List<string> {
            "Vavuniya", "Cheddikulam", "Nedunkeni"
        }},
        { "Mullaitivu", new List<string> {
            "Mullaitivu", "Puthukkudiyiruppu", "Oddusuddan", "Mankulam"
        }},
        { "Batticaloa", new List<string> {
            "Batticaloa", "Kattankudy", "Eravur", "Valachchenai", "Kaluwanchikudy", "Chenkalady"
        }},
        { "Ampara", new List<string> {
            "Ampara", "Kalmunai", "Sammanthurai", "Akkaraipattu", "Pottuvil", "Sainthamaruthu", "Dehiattakandiya"
        }},
        { "Trincomalee", new List<string> {
            "Trincomalee", "Kinniya", "Mutur", "Kantale"
        }},
        { "Kurunegala", new List<string> {
            "Kurunegala", "Kuliyapitiya", "Narammala", "Wariyapola", "Pannala", "Giriulla", "Polgahawela",
            "Ibbagamuwa", "Alawwa", "Mawathagama"
        }},
        { "Puttalam", new List<string> {
            "Puttalam", "Chilaw", "Wennappuwa", "Marawila", "Dankotuwa", "Anamaduwa", "Kalpitiya", "Nattandiya"
        }},
        { "Anuradhapura", new List<string> {
            "Anuradhapura", "Kekirawa", "Medawachchiya", "Tambuttegama", "Eppawala", "Galnewa", "Mihintale", "Nochchiyagama"
        }},
        { "Polonnaruwa", new List<string> {
            "Polonnaruwa", "Kaduruwela", "Hingurakgoda", "Medirigiriya", "Minneriya"
        }},
        { "Badulla", new List<string> {
            "Badulla", "Bandarawela", "Ella", "Haputale", "Welimada", "Mahiyanganaya", "Hali Ela", "Passara", "Diyatalawa"
        }},
        { "Monaragala", new List<string> {
            "Monaragala", "Wellawaya", "Buttala", "Bibile", "Kataragama", "Siyambalanduwa"
        }},
        { "Ratnapura", new List<string> {
            "Ratnapura", "Balangoda", "Pelmadulla", "Embilipitiya", "Kuruwita", "Eheliyagoda", "Kahawatta"
        }},
        { "Kegalle", new List<string> {
            "Kegalle", "Mawanella", "Warakapola", "Ruwanwella", "Yatiyantota", "Dehiowita", "Deraniyagala", "Rambukkana"
        }}
    };

    public CommonService(ICommonRepository commonRepository)
    {
        _commonRepository = commonRepository;
    }

    public Task<IReadOnlyList<ProvinceResponse>> GetProvincesAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<ProvinceResponse>>(StaticProvinces);
    }

    public async Task<IReadOnlyList<DistrictResponse>> GetDistrictsAsync(
        short? provinceId = null,
        CancellationToken cancellationToken = default)
    {
        var dbDistricts = await _commonRepository.GetDistrictsAsync(cancellationToken);

        IReadOnlyList<DistrictResponse> results;

        if (dbDistricts.Count > 0)
        {
            results = dbDistricts.Select(d => new DistrictResponse
            {
                Id = d.Id,
                Code = d.Code,
                Name = d.Name,
                ProvinceId = ResolveProvinceId(d.Name, d.Code)
            }).ToList();
        }
        else
        {
            results = StaticFallbackDistricts;
        }

        if (provinceId.HasValue)
        {
            results = results.Where(d => d.ProvinceId == provinceId.Value).ToList();
        }

        return results;
    }

    public async Task<IReadOnlyList<CityResponse>> GetCitiesByDistrictAsync(
        short districtId,
        CancellationToken cancellationToken = default)
    {
        var allDistricts = await GetDistrictsAsync(cancellationToken: cancellationToken);
        var district = allDistricts.FirstOrDefault(d => d.Id == districtId);

        var cityNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if (district != null && DistrictNameToCities.TryGetValue(district.Name, out var cities))
        {
            foreach (var c in cities)
            {
                cityNames.Add(c);
            }
        }

        var dsList = await _commonRepository.GetDivisionalSecretariatsByDistrictAsync(districtId, cancellationToken);
        foreach (var ds in dsList)
        {
            cityNames.Add(ds.Name);
        }

        return cityNames
            .OrderBy(c => c)
            .Select(c => new CityResponse
            {
                DistrictId = districtId,
                Name = c
            })
            .ToList();
    }

    public async Task<IReadOnlyList<DivisionalSecretariatResponse>> GetDivisionalSecretariatsAsync(
        short districtId,
        CancellationToken cancellationToken = default)
    {
        var dsList = await _commonRepository.GetDivisionalSecretariatsByDistrictAsync(districtId, cancellationToken);

        return dsList.Select(ds => new DivisionalSecretariatResponse
        {
            Id = ds.Id,
            DistrictId = ds.DistrictId,
            Code = ds.Code,
            Name = ds.Name
        }).ToList();
    }

    public async Task<IReadOnlyList<GramaNiladhariDivisionResponse>> GetGramaNiladhariDivisionsAsync(
        int divisionalSecretariatId,
        CancellationToken cancellationToken = default)
    {
        var gnList = await _commonRepository.GetGnDivisionsByDsAsync(divisionalSecretariatId, cancellationToken);

        return gnList.Select(gn => new GramaNiladhariDivisionResponse
        {
            Id = gn.Id,
            DivisionalSecretariatId = gn.DivisionalSecretariatId,
            Code = gn.Code,
            Name = gn.Name
        }).ToList();
    }

    private static short ResolveProvinceId(string districtName, string districtCode)
    {
        if (DistrictNameToProvinceId.TryGetValue(districtName, out var pid))
            return pid;

        if (DistrictCodeToProvinceId.TryGetValue(districtCode, out pid))
            return pid;

        return 1;
    }
}
