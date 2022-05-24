using System.IO.MemoryMappedFiles;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Mapster.Common.MemoryMappedTypes;

/// <summary>
///     Action to be called when iterating over <see cref="MapFeature" /> in a given bounding box via a call to
///     <see cref="DataFile.ForeachFeature" />
/// </summary>
/// <param name="feature">The current <see cref="MapFeature" />.</param>
/// <param name="label">The label of the feature, <see cref="string.Empty" /> if not available.</param>
/// <param name="coordinates">The coordinates of the <see cref="MapFeature" />.</param>
/// <returns></returns>
public delegate bool MapFeatureDelegate(MapFeatureData featureData);

/// <summary>
///     Aggregation of all the data needed to render a map feature
/// </summary>
public class PropertySetter
{
    // Create the attributes (all properties, initialize them with NULL).
    private Highway highwayField = Highway.NULL;
    public Highway highway
    {
        get { return highwayField; }
        set { highwayField = value; }
    }

    private Water waterField = Water.NULL;
    public Water water
    {
        get { return waterField; }
        set { waterField = value; }
    }

    private Place placeField = Place.NULL;
    public Place place
    {
        get { return placeField; }
        set { placeField = value; }
    }

    private Railway railwayField = Railway.NULL;
    public Railway railway
    {
        get { return railwayField; }
        set { railwayField = value; }
    }

    private Natural naturalField = Natural.NULL;
    public Natural natural
    {
        get { return naturalField; }
        set { naturalField = value; }
    }

    private Land_Use landuseField = Land_Use.NULL;
    public Land_Use landUse
    {
        get { return landuseField; }
        set { landuseField = value; }
    }

    private Building buildingField = Building.NULL;
    public Building building
    {
        get { return buildingField; }
        set { buildingField = value; }
    }

    private Boundary boundaryField = Boundary.NULL;
    public Boundary boundary
    {
        get { return boundaryField; }
        set { boundaryField = value; }
    }

    private Public_Amenity publicAmenityField = Public_Amenity.NULL;
    public Public_Amenity publicAmenity
    {
        get { return publicAmenityField; }
        set { publicAmenityField = value; }
    }



    private Private_Amenity privateAmenityField = Private_Amenity.NULL;
    public Private_Amenity privateAmenity
    {
        get { return privateAmenityField; }
        set { privateAmenityField = value; }
    }

    private AdminLevel adminLevelField = AdminLevel.NULL;
    public AdminLevel adminLevel
    {
        get { return adminLevelField; }
        set { adminLevelField = value; }
    }

    public string? name = null;



    // Constructor
    public PropertySetter(Dictionary<string, string> properties)
    {
        setDictionaries(properties);
    }

    // Iterate through all the properties;
    // Check which entry is given.
    private void setDictionaries(Dictionary<string, string> properties)
    {

        foreach (KeyValuePair<string, string> entry in properties)
        {

            switch (true)
            {
                case var value when value == entry.Key.StartsWith("highway"):
                    string keyHighway = highwayDictionary.Keys.Where(k => entry.Value.StartsWith(k)).FirstOrDefault("");
                    highwayDictionary.TryGetValue(keyHighway, out highwayField);
                    break;

                case var value when value == entry.Key.StartsWith("boundary"):
                    string keyBoundary = BoundaryDictionary.Keys.Where(k => entry.Value.StartsWith(k)).FirstOrDefault("");
                    BoundaryDictionary.TryGetValue(keyBoundary, out boundaryField);
                    break;

                case var value when value == entry.Key.StartsWith("natural"):
                    string keyNatural = NaturalDictionary.Keys.Where(k => entry.Value.StartsWith(k)).FirstOrDefault("");
                    NaturalDictionary.TryGetValue(keyNatural, out naturalField);
                    break;

                case var value when value == entry.Key.StartsWith("landuse"):
                    string keyLanduse = LandUseDictionary.Keys.Where(k => entry.Value.StartsWith(k)).FirstOrDefault("");
                    LandUseDictionary.TryGetValue(keyLanduse, out landuseField);
                    break;

                case var value when value == entry.Key.StartsWith("admin_level"):
                    string keyAdmin_level = AdminLevelDictionary.Keys.Where(k => entry.Value.StartsWith(k)).FirstOrDefault("");
                    AdminLevelDictionary.TryGetValue(keyAdmin_level, out adminLevelField);
                    break;

                case var value when value == entry.Key.StartsWith("place"):
                    string keyPlace = PlaceDictionary.Keys.Where(k => entry.Value.StartsWith(k)).FirstOrDefault("");
                    PlaceDictionary.TryGetValue(keyPlace, out placeField);
                    break;

            }
        }
    }

    // Creating enums for each property and mapping them to the specific sting <String,Property>.
    public enum Highway
    {
        NULL,
        MOTORWAY,
        TRUNK,
        PRIMARY,
        SECONDARY,
        TERTIARY,
        UNCLASSIFIED,
        RESIDENTIAL,
        ROAD,

    };
    protected static readonly Dictionary<string, Highway> highwayDictionary = new Dictionary<string, Highway> {
      { "motorway", Highway.MOTORWAY },
      { "trunk", Highway.TRUNK },
      { "primary", Highway.PRIMARY },
      { "secondary", Highway.SECONDARY },
      { "tertiary", Highway.TERTIARY },
      { "unclassified", Highway.UNCLASSIFIED },
      { "residential", Highway.RESIDENTIAL },
      { "road", Highway.ROAD },
    };


    public enum Place
    {
        NULL,
        CITY,
        LOCALITY,
        HAMLET,
        TOWN,

    };
    protected static readonly Dictionary<string, Place> PlaceDictionary = new Dictionary<string, Place>() {
    { "city", Place.CITY },
    { "town", Place.TOWN },
    { "locality", Place.LOCALITY },
    { "hamlet", Place.HAMLET }
  };


    public enum Land_Use
    {
        NULL,
        RESIDENTIAL,
        INDUSTRIAL,
        FOREST,
        COMMERCIAL,
        CONSTRUCTION,
        MILITARY,
        QUARRY,
        BROWNFIELD,
        FARM,
        GRASS,
        GREENFIELD,
        RECREATION_GROUND,
        ALLOTMENTS,
        RECREATIONAL,
        TRANSPORT,
        AGRICULTURAL,
        ORCHARD,
        CEMETERY,
        SQUARE,
        MEADOW,
        WINTER_SPORTS,
        RESERVOIR,
        BASIN,

    };
    protected static readonly Dictionary<string, Land_Use> LandUseDictionary = new Dictionary<string, Land_Use>() {
    { "forest", Land_Use.FOREST },
    { "orchard", Land_Use.ORCHARD },
    { "residential", Land_Use.RESIDENTIAL },
    { "cemetery", Land_Use.CEMETERY },
    { "industrial", Land_Use.INDUSTRIAL },
    { "commercial", Land_Use.COMMERCIAL },
    { "square", Land_Use.SQUARE },
    { "construction", Land_Use.CONSTRUCTION },
    { "military", Land_Use.MILITARY },
    { "quarry", Land_Use.QUARRY },
    { "brownfield", Land_Use.BROWNFIELD },
    { "farm", Land_Use.FARM },
    { "meadow", Land_Use.MEADOW },
    { "grass", Land_Use.GRASS },
    { "greenfield", Land_Use.GREENFIELD },
    { "recreation_ground", Land_Use.RECREATION_GROUND },
    { "winter_sports", Land_Use.WINTER_SPORTS },
    { "allotments", Land_Use.ALLOTMENTS },
    { "reservoir", Land_Use.RESERVOIR },
    { "basin", Land_Use.BASIN },
  };

    public enum Natural
    {
        NULL,
        GRASSLAND,
        HEATH,
        MOOR,
        SCRUB,
        FELL,
        WETLAND,
        WOOD,
        TREE_ROW,
        BARE_ROCK,
        ROCK,
        SCREE,
        BEACH,
        SAND,
        WATER,
        MINERAL,
        FOREST,
        WILDLIFE,
    };
    protected static readonly Dictionary<string, Natural> NaturalDictionary = new Dictionary<string, Natural>() {
    { "fell", Natural.FELL },
    { "grassland", Natural.GRASSLAND },
    { "heath", Natural.HEATH },
    { "moor", Natural.MOOR },
    { "scrub", Natural.SCRUB },
    { "wetland", Natural.WETLAND },
    { "wood", Natural.WOOD },
    { "tree_row", Natural.TREE_ROW },
    { "bare_rock", Natural.BARE_ROCK },
    { "rock", Natural.ROCK },
    { "scree", Natural.SCREE },
    { "beach", Natural.BEACH },
    { "sand", Natural.SAND },
    { "water", Natural.WATER },
  };

    public enum AdminLevel
    {
        NULL,
        LEVEL2,
    };

    private static Dictionary<string, AdminLevel> AdminLevelDictionary = new Dictionary<string, AdminLevel>() {
    { "2", AdminLevel.LEVEL2 },
  };

    public enum Boundary
    {
        NULL,
        ADMINISTRATIVE,
        FOREST,
    };
    protected static readonly Dictionary<string, Boundary> BoundaryDictionary = new Dictionary<string, Boundary>(){
    { "administrative", Boundary.ADMINISTRATIVE },
    { "forest", Boundary.FOREST },
  };

    /* 
        Further perspectives of adding more properties.
        Need to be implemented;
        After Implementation these can mapped accordingly <String,SpecificProperty>
     */
    public enum Water
    {
        NULL,
        RIVER,
        LAKE,
        OCEAN,
        STREAM,
        GLACIER,
    };
    public enum Railway
    {
        NULL,
        HEAVY,
        INTERCITY,
        HIGHSPEED,
        HERITAGE,
        MONORAIL,
        MOUNTAIN,
        PLATEWAY,
        RACK,
        TOURIST,
        WAGONWAY,
    };
    public enum Building
    {
        NULL,
        RESIDENTIAL,
        EDUCATIONAL,
        INSTITUTIONAL,
        ASSEMBLY,
        BUSINESS,
        MERCANTILE,
        INDUSTRIAL,
        STORAGE,
    };

    public enum Public_Amenity
    {
        NULL,
        RESIDENTIAL_HOME,
        PARK,
        SCHOOL,
        SHOPPING_CENTER,
        POST_OFFICE,
    };

    public enum Private_Amenity
    {
        NULL,
        PRIVATE_ROAD,
        PRIVATE_DRIVEWAY,
        PRIVATE_CLUB,
        PRIVATE_LAND,
    };

}
public readonly ref struct MapFeatureData
{
    public long Id { get; init; }
    public GeometryType Type { get; init; }
    public ReadOnlySpan<char> Label { get; init; }
    public ReadOnlySpan<Coordinate> Coordinates { get; init; }
    // Add the properties.
    public PropertySetter Properties { get; init; }

}

/// <summary>
///     Represents a file with map data organized in the following format:<br />
///     <see cref="FileHeader" /><br />
///     Array of <see cref="TileHeaderEntry" /> with <see cref="FileHeader.TileCount" /> records<br />
///     Array of tiles, each tile organized:<br />
///     <see cref="TileBlockHeader" /><br />
///     Array of <see cref="MapFeature" /> with <see cref="TileBlockHeader.FeaturesCount" /> at offset
///     <see cref="TileHeaderEntry.OffsetInBytes" /> + size of <see cref="TileBlockHeader" /> in bytes.<br />
///     Array of <see cref="Coordinate" /> with <see cref="TileBlockHeader.CoordinatesCount" /> at offset
///     <see cref="TileBlockHeader.CharactersOffsetInBytes" />.<br />
///     Array of <see cref="StringEntry" /> with <see cref="TileBlockHeader.StringCount" /> at offset
///     <see cref="TileBlockHeader.StringsOffsetInBytes" />.<br />
///     Array of <see cref="char" /> with <see cref="TileBlockHeader.CharactersCount" /> at offset
///     <see cref="TileBlockHeader.CharactersOffsetInBytes" />.<br />
/// </summary>
public unsafe class DataFile : IDisposable
{
    private readonly FileHeader* _fileHeader;
    private readonly MemoryMappedViewAccessor _mma;
    private readonly MemoryMappedFile _mmf;

    private readonly byte* _ptr;
    private readonly int CoordinateSizeInBytes = Marshal.SizeOf<Coordinate>();
    private readonly int FileHeaderSizeInBytes = Marshal.SizeOf<FileHeader>();
    private readonly int MapFeatureSizeInBytes = Marshal.SizeOf<MapFeature>();
    private readonly int StringEntrySizeInBytes = Marshal.SizeOf<StringEntry>();
    private readonly int TileBlockHeaderSizeInBytes = Marshal.SizeOf<TileBlockHeader>();
    private readonly int TileHeaderEntrySizeInBytes = Marshal.SizeOf<TileHeaderEntry>();

    private bool _disposedValue;

    public DataFile(string path)
    {
        _mmf = MemoryMappedFile.CreateFromFile(path);
        _mma = _mmf.CreateViewAccessor();
        _mma.SafeMemoryMappedViewHandle.AcquirePointer(ref _ptr);
        _fileHeader = (FileHeader*)_ptr;
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _mma?.SafeMemoryMappedViewHandle.ReleasePointer();
                _mma?.Dispose();
                _mmf?.Dispose();
            }

            _disposedValue = true;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private TileHeaderEntry* GetNthTileHeader(int i)
    {
        return (TileHeaderEntry*)(_ptr + i * TileHeaderEntrySizeInBytes + FileHeaderSizeInBytes);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private (TileBlockHeader? Tile, ulong TileOffset) GetTile(int tileId)
    {
        ulong tileOffset = 0;
        for (var i = 0; i < _fileHeader->TileCount; ++i)
        {
            var tileHeaderEntry = GetNthTileHeader(i);
            if (tileHeaderEntry->ID == tileId)
            {
                tileOffset = tileHeaderEntry->OffsetInBytes;
                return (*(TileBlockHeader*)(_ptr + tileOffset), tileOffset);
            }
        }

        return (null, 0);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private MapFeature* GetFeature(int i, ulong offset)
    {
        return (MapFeature*)(_ptr + offset + TileBlockHeaderSizeInBytes + i * MapFeatureSizeInBytes);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private ReadOnlySpan<Coordinate> GetCoordinates(ulong coordinateOffset, int ithCoordinate, int coordinateCount)
    {
        return new ReadOnlySpan<Coordinate>(_ptr + coordinateOffset + ithCoordinate * CoordinateSizeInBytes, coordinateCount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private void GetString(ulong stringsOffset, ulong charsOffset, int i, out ReadOnlySpan<char> value)
    {
        var stringEntry = (StringEntry*)(_ptr + stringsOffset + i * StringEntrySizeInBytes);
        value = new ReadOnlySpan<char>(_ptr + charsOffset + stringEntry->Offset * 2, stringEntry->Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private void GetProperty(ulong stringsOffset, ulong charsOffset, int i, out ReadOnlySpan<char> key, out ReadOnlySpan<char> value)
    {
        if (i % 2 != 0)
        {
            throw new ArgumentException("Properties are key-value pairs and start at even indices in the string list (i.e. i % 2 == 0)");
        }

        GetString(stringsOffset, charsOffset, i, out key);
        GetString(stringsOffset, charsOffset, i + 1, out value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void ForeachFeature(BoundingBox b, MapFeatureDelegate? action)
    {
        if (action == null)
        {
            return;
        }

        var tiles = TiligSystem.GetTilesForBoundingBox(b.MinLat, b.MinLon, b.MaxLat, b.MaxLon);
        for (var i = 0; i < tiles.Length; ++i)
        {
            var header = GetTile(tiles[i]);
            if (header.Tile == null)
            {
                continue;
            }
            for (var j = 0; j < header.Tile.Value.FeaturesCount; ++j)
            {
                var feature = GetFeature(j, header.TileOffset);
                var coordinates = GetCoordinates(header.Tile.Value.CoordinatesOffsetInBytes, feature->CoordinateOffset, feature->CoordinateCount);
                var isFeatureInBBox = false;

                for (var k = 0; k < coordinates.Length; ++k)
                {
                    if (b.Contains(coordinates[k]))
                    {
                        isFeatureInBBox = true;
                        break;
                    }
                }

                var label = ReadOnlySpan<char>.Empty;
                if (feature->LabelOffset >= 0)
                {
                    GetString(header.Tile.Value.StringsOffsetInBytes, header.Tile.Value.CharactersOffsetInBytes, feature->LabelOffset, out label);
                }

                if (isFeatureInBBox)
                {
                    var properties = new Dictionary<string, string>(feature->PropertyCount);
                    for (var p = 0; p < feature->PropertyCount; ++p)
                    {
                        GetProperty(header.Tile.Value.StringsOffsetInBytes, header.Tile.Value.CharactersOffsetInBytes, p * 2 + feature->PropertiesOffset, out var key, out var value);
                        properties.Add(key.ToString(), value.ToString());
                    }

                    if (!action(new MapFeatureData
                    {
                        // Initialize the PropertySetter
                        Properties = new PropertySetter(properties),
                        Id = feature->Id,
                        Label = label,
                        Coordinates = coordinates,
                        Type = feature->GeometryType
                    }))
                    {
                        break;
                    }
                }
            }
        }
    }
}
