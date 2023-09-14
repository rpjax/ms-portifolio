using MongoDB.Bson;

namespace ModularSystem.Core
{
    public class Image
    {
        public ObjectId Id { get; set; }

        public string Owner { get; set; } = "";

        public ImageMetadata Metadata { get; set; } = new ImageMetadata();
    }

    public class ImageMetadata
    {

    }
}
