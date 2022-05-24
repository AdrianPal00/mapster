using Mapster.Common.MemoryMappedTypes;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;

namespace Mapster.Rendering;

public struct GeoFeature : BaseShape
{
    public enum GeoFeatureType
    {
        Plain,
        Hills,
        Mountains,
        Forest,
        Desert,
        Unknown,
        Water,
        Residential
    }

    public int ZIndex
    {
        get
        {
            switch (Type)
            {
                case GeoFeatureType.Plain:
                    return 10;
                case GeoFeatureType.Hills:
                    return 12;
                case GeoFeatureType.Mountains:
                    return 13;
                case GeoFeatureType.Forest:
                    return 11;
                case GeoFeatureType.Desert:
                    return 9;
                case GeoFeatureType.Unknown:
                    return 8;
                case GeoFeatureType.Water:
                    return 40;
                case GeoFeatureType.Residential:
                    return 41;
            }

            return 7;
        }
        set { }
    }

    public bool IsPolygon { get; set; }
    public PointF[] ScreenCoordinates { get; set; }
    public GeoFeatureType Type { get; set; }

    public void Render(IImageProcessingContext context)
    {
        var color = Color.Magenta;
        switch (Type)
        {
            case GeoFeatureType.Plain:
                color = Color.LightGreen;
                break;
            case GeoFeatureType.Hills:
                color = Color.DarkGreen;
                break;
            case GeoFeatureType.Mountains:
                color = Color.LightGray;
                break;
            case GeoFeatureType.Forest:
                color = Color.Green;
                break;
            case GeoFeatureType.Desert:
                color = Color.SandyBrown;
                break;
            case GeoFeatureType.Unknown:
                color = Color.Magenta;
                break;
            case GeoFeatureType.Water:
                color = Color.LightBlue;
                break;
            case GeoFeatureType.Residential:
                color = Color.LightCoral;
                break;
        }

        if (!IsPolygon)
        {
            var pen = new Pen(color, 1.2f);
            context.DrawLines(pen, ScreenCoordinates);
        }
        else
        {
            context.FillPolygon(color, ScreenCoordinates);
        }
    }

    public GeoFeature(ReadOnlySpan<Coordinate> c, GeoFeatureType type)
    {
        IsPolygon = true;
        Type = type;
        ScreenCoordinates = new PointF[c.Length];
        for (var i = 0; i < c.Length; i++)
            ScreenCoordinates[i] = new PointF((float)MercatorProjection.lonToX(c[i].Longitude),
                (float)MercatorProjection.latToY(c[i].Latitude));
    }

    public GeoFeature(ReadOnlySpan<Coordinate> c, MapFeatureData feature)
    {
        IsPolygon = feature.Type == GeometryType.Polygon;
        var natural = feature.Properties.natural;
        Type = GeoFeatureType.Unknown;

        // If natural is not null;
        // Check the type of the nature property.
        if (natural != PropertySetter.Natural.NULL)
        {

            bool water = natural == PropertySetter.Natural.WATER;

            bool desert = natural == PropertySetter.Natural.BEACH ||
                     natural == PropertySetter.Natural.SAND;

            bool forest = natural == PropertySetter.Natural.WOOD ||
                     natural == PropertySetter.Natural.TREE_ROW;

            bool mountains = natural == PropertySetter.Natural.BARE_ROCK ||
                                 natural == PropertySetter.Natural.ROCK ||
                                 natural == PropertySetter.Natural.SCREE;

            bool plain = natural == PropertySetter.Natural.FELL ||
            natural == PropertySetter.Natural.GRASSLAND || natural == PropertySetter.Natural.HEATH ||
            natural == PropertySetter.Natural.MOOR || natural == PropertySetter.Natural.SCRUB || natural == PropertySetter.Natural.WETLAND;


            switch (true)
            {
                case var value when value == plain:
                    Type = GeoFeatureType.Plain;
                    break;

                case var value when value == forest:
                    Type = GeoFeatureType.Forest;
                    break;

                case var value when value == mountains:
                    Type = GeoFeatureType.Forest;
                    break;

                case var value when value == desert:
                    Type = GeoFeatureType.Desert;
                    break;

                case var value when value == water:
                    Type = GeoFeatureType.Water;
                    break;
            }
        }


        ScreenCoordinates = new PointF[c.Length];
        for (var i = 0; i < c.Length; i++)
            ScreenCoordinates[i] = new PointF((float)MercatorProjection.lonToX(c[i].Longitude),
                (float)MercatorProjection.latToY(c[i].Latitude));
    }

    // Add checkers for each property.
    // Will be used in the TileRenderer.
    public static bool ShouldBeNatural(MapFeatureData feature)
    {
        return feature.Type == GeometryType.Polygon && feature.Properties.natural != PropertySetter.Natural.NULL;
    }

    public static bool ShouldBeForest(MapFeatureData feature)
    {
        return feature.Properties.boundary == PropertySetter.Boundary.FOREST;
    }

    public static bool ShouldBeLanduseForestOrOrchad(MapFeatureData feature)
    {
        return feature.Properties.landUse == PropertySetter.Land_Use.FOREST || feature.Properties.landUse == PropertySetter.Land_Use.ORCHARD;
    }
    public static bool ShouldBeBuilding(MapFeatureData feature)
    {
        return feature.Type == GeometryType.Polygon && feature.Properties.building != PropertySetter.Building.NULL;
    }

    public static bool ShouldBePublicAmenity(MapFeatureData feature)
    {
        return feature.Type == GeometryType.Polygon && feature.Properties.publicAmenity != PropertySetter.Public_Amenity.NULL;
    }

    public static bool ShouldBePrivateAmenity(MapFeatureData feature)
    {
        return feature.Type == GeometryType.Polygon && feature.Properties.privateAmenity != PropertySetter.Private_Amenity.NULL;
    }

    public static bool ShouldBeLanduseResidential(MapFeatureData feature)
    {
        PropertySetter.Land_Use landuse = feature.Properties.landUse;
        return landuse == PropertySetter.Land_Use.RESIDENTIAL || landuse == PropertySetter.Land_Use.CEMETERY || landuse == PropertySetter.Land_Use.INDUSTRIAL ||
          landuse == PropertySetter.Land_Use.COMMERCIAL || landuse == PropertySetter.Land_Use.SQUARE || landuse == PropertySetter.Land_Use.CONSTRUCTION ||
          landuse == PropertySetter.Land_Use.MILITARY || landuse == PropertySetter.Land_Use.QUARRY || landuse == PropertySetter.Land_Use.BROWNFIELD;
    }

    public static bool ShouldBeLandusePlain(MapFeatureData feature)
    {
        PropertySetter.Land_Use Land_Use = feature.Properties.landUse;
        return Land_Use == PropertySetter.Land_Use.FARM || Land_Use == PropertySetter.Land_Use.MEADOW || Land_Use == PropertySetter.Land_Use.GRASS ||
          Land_Use == PropertySetter.Land_Use.GREENFIELD || Land_Use == PropertySetter.Land_Use.RECREATION_GROUND || Land_Use == PropertySetter.Land_Use.WINTER_SPORTS ||
          Land_Use == PropertySetter.Land_Use.ALLOTMENTS;
    }
}

public struct Railway : BaseShape
{
    public int ZIndex { get; set; } = 45;
    public bool IsPolygon { get; set; }
    public PointF[] ScreenCoordinates { get; set; }

    public void Render(IImageProcessingContext context)
    {
        var penA = new Pen(Color.DarkGray, 2.0f);
        var penB = new Pen(Color.LightGray, 1.2f, new[]
        {
            2.0f, 4.0f, 2.0f
        });
        context.DrawLines(penA, ScreenCoordinates);
        context.DrawLines(penB, ScreenCoordinates);
    }

    public Railway(ReadOnlySpan<Coordinate> c)
    {
        IsPolygon = false;
        ScreenCoordinates = new PointF[c.Length];
        for (var i = 0; i < c.Length; i++)
            ScreenCoordinates[i] = new PointF((float)MercatorProjection.lonToX(c[i].Longitude),
                (float)MercatorProjection.latToY(c[i].Latitude));
    }

    // Add the property using the PropertySetter (Railway)
    // Using comparison between enums, rather than string - string comparison.
    public static bool ShouldBeRailway(MapFeatureData feature)
    {
        return feature.Properties.railway != PropertySetter.Railway.NULL;
    }
}

public struct PopulatedPlace : BaseShape
{
    public int ZIndex { get; set; } = 60;
    public bool IsPolygon { get; set; }
    public PointF[] ScreenCoordinates { get; set; }
    public string Name { get; set; }
    public bool ShouldRender { get; set; }

    public void Render(IImageProcessingContext context)
    {
        if (!ShouldRender)
        {
            return;
        }
        var font = SystemFonts.Families.First().CreateFont(12, FontStyle.Bold);
        context.DrawText(Name, font, Color.Black, ScreenCoordinates[0]);
    }

    public PopulatedPlace(ReadOnlySpan<Coordinate> c, MapFeatureData feature)
    {
        IsPolygon = false;
        ScreenCoordinates = new PointF[c.Length];
        for (var i = 0; i < c.Length; i++)
            ScreenCoordinates[i] = new PointF((float)MercatorProjection.lonToX(c[i].Longitude),
                (float)MercatorProjection.latToY(c[i].Latitude));
        var name = feature.Properties.name;

        if (feature.Label.IsEmpty)
        {
            ShouldRender = false;
            Name = "Unknown";
        }
        else
        {
            Name = string.IsNullOrWhiteSpace(name) ? feature.Label.ToString() : name;
            ShouldRender = true;
        }
    }

    // Add the property using the PropertySetter (Place)
    // Using comparison between enums, rather than string - string comparison.
    public static bool ShouldBePopulatedPlace(MapFeatureData feature)
    {
        // https://wiki.openstreetmap.org/wiki/Key:place
        if (feature.Type != GeometryType.Point)
        {
            return false;
        }
        PropertySetter.Place place = feature.Properties.place;
        return place == PropertySetter.Place.CITY || place == PropertySetter.Place.TOWN || place == PropertySetter.Place.LOCALITY || place == PropertySetter.Place.HAMLET;
    }
}

public struct Border : BaseShape
{
    public int ZIndex { get; set; } = 30;
    public bool IsPolygon { get; set; }
    public PointF[] ScreenCoordinates { get; set; }

    public void Render(IImageProcessingContext context)
    {
        var pen = new Pen(Color.Gray, 2.0f);
        context.DrawLines(pen, ScreenCoordinates);
    }

    public Border(ReadOnlySpan<Coordinate> c)
    {
        IsPolygon = false;
        ScreenCoordinates = new PointF[c.Length];
        for (var i = 0; i < c.Length; i++)
            ScreenCoordinates[i] = new PointF((float)MercatorProjection.lonToX(c[i].Longitude),
                (float)MercatorProjection.latToY(c[i].Latitude));
    }

    // Add the property using the PropertySetter (Border)
    // Using comparison between enums, rather than string - string comparison.
    public static bool ShouldBeBorder(MapFeatureData feature)
    {
        // https://wiki.openstreetmap.org/wiki/Key:admin_level
        return feature.Properties.boundary == PropertySetter.Boundary.ADMINISTRATIVE && feature.Properties.adminLevel == PropertySetter.AdminLevel.LEVEL2;

    }
}

public struct Waterway : BaseShape
{
    public int ZIndex { get; set; } = 40;
    public bool IsPolygon { get; set; }
    public PointF[] ScreenCoordinates { get; set; }

    public void Render(IImageProcessingContext context)
    {
        if (!IsPolygon)
        {
            var pen = new Pen(Color.LightBlue, 1.2f);
            context.DrawLines(pen, ScreenCoordinates);
        }
        else
        {
            context.FillPolygon(Color.LightBlue, ScreenCoordinates);
        }
    }

    public Waterway(ReadOnlySpan<Coordinate> c, bool isPolygon = false)
    {
        IsPolygon = isPolygon;
        ScreenCoordinates = new PointF[c.Length];
        for (var i = 0; i < c.Length; i++)
            ScreenCoordinates[i] = new PointF((float)MercatorProjection.lonToX(c[i].Longitude),
                (float)MercatorProjection.latToY(c[i].Latitude));
    }

    // Add the property using the PropertySetter (Water)
    // Using comparison between enums, rather than string - string comparison.
    public static bool ShouldBeWaterway(MapFeatureData feature)
    {
        return feature.Properties.water != PropertySetter.Water.NULL && feature.Type != GeometryType.Point;
    }


}

public struct Road : BaseShape
{
    public int ZIndex { get; set; } = 50;
    public bool IsPolygon { get; set; }
    public PointF[] ScreenCoordinates { get; set; }

    public void Render(IImageProcessingContext context)
    {
        if (!IsPolygon)
        {
            var pen = new Pen(Color.Coral, 2.0f);
            var pen2 = new Pen(Color.Yellow, 2.2f);
            context.DrawLines(pen2, ScreenCoordinates);
            context.DrawLines(pen, ScreenCoordinates);
        }
    }

    public Road(ReadOnlySpan<Coordinate> c, bool isPolygon = false)
    {
        IsPolygon = isPolygon;
        ScreenCoordinates = new PointF[c.Length];
        for (var i = 0; i < c.Length; i++)
            ScreenCoordinates[i] = new PointF((float)MercatorProjection.lonToX(c[i].Longitude),
                (float)MercatorProjection.latToY(c[i].Latitude));
    }

    // Add the property using the PropertySetter (Road)
    // Using comparison between enums, rather than string - string comparison.
    public static bool ShouldBeRoad(MapFeatureData feature)
    {
        return feature.Properties.highway != PropertySetter.Highway.NULL && feature.Type != GeometryType.Point;
    }
}

public interface BaseShape
{
    public int ZIndex { get; set; }
    public bool IsPolygon { get; set; }
    public PointF[] ScreenCoordinates { get; set; }

    public void Render(IImageProcessingContext context);

    public void TranslateAndScale(float minX, float minY, float scale, float height)
    {
        for (var i = 0; i < ScreenCoordinates.Length; i++)
        {
            var coord = ScreenCoordinates[i];
            var newCoord = new PointF((coord.X + minX * -1) * scale, height - (coord.Y + minY * -1) * scale);
            ScreenCoordinates[i] = newCoord;
        }
    }
}
