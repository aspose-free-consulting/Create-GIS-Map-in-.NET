using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Web.Mvc;
using Aspose.Gis;
using Aspose.Gis.Rendering;
using Aspose.Gis.Rendering.Labelings;
using Aspose.Gis.Rendering.Symbolizers;
using Web.Map.Models;

namespace Web.Map.Controllers
{
    public class DefaultController : Controller
    {
        // !!! Please use your connection string
        private static string _connectionString =
            "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=W:\\temp\\db\\gisdb.mdf;Integrated Security=True;Connect Timeout=30";

        public ActionResult Index()
        {
            var model = new MapModel();
            return View(model);
        }

        [HttpGet]
        public ActionResult Draw()
        {
            using (var layers = OpenSpatialDataset())
            {
                // add spatial tables in database if they are missed
                CreateTablesIfNeed(layers);

                // path and stream for image
                var imageStream = new MemoryStream();
                var path = AbstractPath.FromStream(imageStream);

                // draw map
                using (var map = new Aspose.Gis.Rendering.Map(400, 400))
                {
                    map.Padding = new Measurement(15, Unit.Pixels);
                    map.BackgroundColor = Color.AliceBlue;


                    var shopsLayer = layers.OpenLayer(TableNames.Shops);
                    var buildingLayer = layers.OpenLayer(TableNames.Building);


                    // replace labels names and shift labels
                    var labeling = new SimpleLabeling()
                    {
                        Placement = new PointLabelPlacement() {VerticalOffset = -10},
                        FontSize = 12
                    };
                    labeling.LabelExpression = feature =>
                    {
                        var title = feature.GetValue<string>("Title");
                        switch (title)
                        {
                            case "Shops_0":
                                return "NEW NAME 0";
                            case "Shops_3":
                                return "NEW NAME 3";
                            case "Shops_4":
                                return "NEW NAME 4";
                            case "Shops_7":
                                return "NEW NAME 7";
                            default:
                                return title;
                        }
                    };

                    // custom style for 'shops'
                    var markerStyle = new SimpleMarker() {FillColor = Color.Violet};
                    map.Add(shopsLayer, markerStyle, labeling);

                    // default style for other 'building'
                    map.Add(buildingLayer);

                    map.Render(path, Renderers.Png);
                }

                // convert to base64 string
                var base64 = Convert.ToBase64String(imageStream.ToArray());

                // view result
                var model = new MapModel()
                {
                    Base64Image = base64
                };
                return View(nameof(Index), model);
            }
        }

        private static Dataset OpenSpatialDataset()
        {
            var connection = new SqlConnection(_connectionString);
            connection.Open();
            return Drivers.SqlServer.OpenDataset(connection);
        }

        private static List<string> GetLayerNames(Dataset layers)
        {
            var names = new List<string>();
            for (int i = 0; i < layers.LayersCount; i++)
            {
                names.Add(layers.GetLayerName(i));
            }

            return names;
        }

        private static void CreateTablesIfNeed(Dataset layers)
        {
            var names = GetLayerNames(layers);

            // points 1
            if (!names.Contains(TableNames.Shops))
            {
                using (var linesLayer = layers.CreateLayer(TableNames.Shops))
                {
                    linesLayer.Attributes.Add(new FeatureAttribute("Id", AttributeDataType.Guid));
                    linesLayer.Attributes.Add(new FeatureAttribute("Title", AttributeDataType.String));

                    for (int i = 0; i < 10; i++)
                    {
                        Feature feature = linesLayer.ConstructFeature();
                        feature.SetValue("Id", Guid.NewGuid());
                        feature.SetValue("Title", $"Shops_{i}");
                        feature.Geometry = new Aspose.Gis.Geometries.Point(i, i);
                        linesLayer.Add(feature);
                    }
                }
            }

            if (!names.Contains(TableNames.Building))
            {
                using (var linesLayer = layers.CreateLayer(TableNames.Building))
                {
                    linesLayer.Attributes.Add(new FeatureAttribute("Id", AttributeDataType.Guid));
                    linesLayer.Attributes.Add(new FeatureAttribute("Title", AttributeDataType.String));

                    for (int i = 0; i < 10; i++)
                    {
                        Feature feature = linesLayer.ConstructFeature();
                        feature.SetValue("Id", Guid.NewGuid());
                        feature.SetValue("Title", $"Buildings_{i}");
                        feature.Geometry = new Aspose.Gis.Geometries.Point(9 - i, i);
                        linesLayer.Add(feature);
                    }
                }
            }
        }

        private static class TableNames
        {
            public const string Building = "Building";
            public const string Shops = "Shops";
        }
    }
}