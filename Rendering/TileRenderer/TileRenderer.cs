using Mapster.Common.MemoryMappedTypes;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Mapster.Rendering;

public static class TileRenderer
{
    public static BaseShape Tessellate(this MapFeatureData feature, ref BoundingBox boundingBox, ref PriorityQueue<BaseShape, int> shapes)
    {
        BaseShape? baseShape = null;

        var featureType = feature.Type;
        var coordinates = feature.Coordinates;
        // Used from RenderingTypes.
        // Assigning to baseShape a specific Property that is found.
        bool road = Road.ShouldBeRoad(feature);
        bool water = Waterway.ShouldBeWaterway(feature);
        bool border = Border.ShouldBeBorder(feature);
        bool place = PopulatedPlace.ShouldBePopulatedPlace(feature);
        bool railway = Railway.ShouldBeRailway(feature);
        bool natural = GeoFeature.ShouldBeNatural(feature);
        bool building = GeoFeature.ShouldBeBuilding(feature);
        bool forest = GeoFeature.ShouldBeForest(feature);
        bool public_amenity = GeoFeature.ShouldBePublicAmenity(feature);
        bool private_amenity = GeoFeature.ShouldBePrivateAmenity(feature);
        bool landuseForestOrOrchad = GeoFeature.ShouldBeLanduseForestOrOrchad(feature);
        bool landusePlain = GeoFeature.ShouldBeLandusePlain(feature);
        bool landuseResidential = GeoFeature.ShouldBeLanduseResidential(feature);

        switch (true)
        {

            case var value when value == natural:
                baseShape = new GeoFeature(coordinates, feature);
                break;

            case var value when value == railway:
                baseShape = new Railway(coordinates);
                break;

            case var value when value == road:
                baseShape = new Road(coordinates);
                break;

            case var value when value == water:
                baseShape = new Waterway(coordinates, feature.Type == GeometryType.Polygon);
                break;

            case var value when value == border:
                baseShape = new Border(coordinates);
                break;

            case var value when value == place:
                baseShape = new PopulatedPlace(coordinates, feature);
                break;

            case var value when value == building:
                baseShape = new GeoFeature(coordinates, GeoFeature.GeoFeatureType.Residential);
                break;

            case var value when value == forest:
                baseShape = new GeoFeature(coordinates, GeoFeature.GeoFeatureType.Forest);
                break;

            case var value when value == public_amenity:
                baseShape = new GeoFeature(coordinates, GeoFeature.GeoFeatureType.Unknown);
                break;

            case var value when value == private_amenity:
                baseShape = new GeoFeature(coordinates, GeoFeature.GeoFeatureType.Unknown);
                break;

            case var value when value == landuseForestOrOrchad:
                baseShape = new GeoFeature(coordinates, GeoFeature.GeoFeatureType.Forest);
                break;

            case var value when value == landusePlain:
                baseShape = new GeoFeature(coordinates, GeoFeature.GeoFeatureType.Plain);
                break;

            case var value when value == landuseResidential:
                baseShape = new GeoFeature(coordinates, GeoFeature.GeoFeatureType.Residential);
                break;
        }

        if (baseShape != null)
        {
            // Adding the found Property to the priority queue.
            shapes.Enqueue(baseShape, baseShape.ZIndex);

            for (var j = 0; j < baseShape.ScreenCoordinates.Length; ++j)
            {
                boundingBox.MinX = Math.Min(boundingBox.MinX, baseShape.ScreenCoordinates[j].X);
                boundingBox.MaxX = Math.Max(boundingBox.MaxX, baseShape.ScreenCoordinates[j].X);
                boundingBox.MinY = Math.Min(boundingBox.MinY, baseShape.ScreenCoordinates[j].Y);
                boundingBox.MaxY = Math.Max(boundingBox.MaxY, baseShape.ScreenCoordinates[j].Y);
            }
        }

        return baseShape;
    }

    public static Image<Rgba32> Render(this PriorityQueue<BaseShape, int> shapes, BoundingBox boundingBox, int width, int height)
    {
        var canvas = new Image<Rgba32>(width, height);

        // Calculate the scale for each pixel, essentially applying a normalization
        var scaleX = canvas.Width / (boundingBox.MaxX - boundingBox.MinX);
        var scaleY = canvas.Height / (boundingBox.MaxY - boundingBox.MinY);
        var scale = Math.Min(scaleX, scaleY);

        // Background Fill
        canvas.Mutate(x => x.Fill(Color.White));
        while (shapes.Count > 0)
        {
            var entry = shapes.Dequeue();
            entry.TranslateAndScale(boundingBox.MinX, boundingBox.MinY, scale, canvas.Height);
            canvas.Mutate(x => entry.Render(x));
        }

        return canvas;
    }

    public struct BoundingBox
    {
        public float MinX;
        public float MaxX;
        public float MinY;
        public float MaxY;
    }
}
