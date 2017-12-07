namespace ErgonomicAdvisor
{
    public class Gifsercise
    {
        public string text { get; set; }
        public string image_url { get; set; }

        public Gifsercise(string url, string text)
        {
            image_url = url;
            this.text = text ?? "Such Strength, Much Prowess - you the BEAST";
        }
    }
}
