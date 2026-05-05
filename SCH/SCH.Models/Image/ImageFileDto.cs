namespace SCH.Models.Image
{
    public class ImageFileDto
    {
        public required FileStream FileStream { get; set; }
        public required string ContentType { get; set; }
    }
}
