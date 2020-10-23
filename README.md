## Draw Layers on Web from SQL Server
This Asp .Net MVC web application draws a map on the web page. The spatial layers are loaded from SQL Server. And we are using [Aspose.GIS](https://products.aspose.com/gis) to implement this functionality.

## System Requirements and Notes
We need to establish an environment where MS SQL Server is installed.

Once the MS SQL Server is ready, a database is to be created. You may create this database using Microsoft SQL Server Management Studio. 

Note, we need to replace the connection string (see DefaultController.cs). Also, the application creates two new tables (Building, Shops) in the database, but you can use any others.

## ScreenShots of Usage

Below is the main screen of the application which shows the result.
* We load layers from the database (for more details see [this link](https://docs.aspose.com/gis/net/databases/))
* The first layer is drawn with black dots using the [default style](https://docs.aspose.com/gis/net/map-rendering/#render-with-default-settings)
* The second layer is painted with colored dots using a [custom style](https://docs.aspose.com/gis/net/marker-symbolizer/#examples). Also, we have displayed [labels](https://docs.aspose.com/gis/net/simple-labeling/) for the second layer.

![After](draw_layers.png)

