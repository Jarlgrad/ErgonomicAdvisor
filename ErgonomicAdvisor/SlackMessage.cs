using System.Collections.Generic;

namespace ErgonomicAdvisor
{
    internal class SlackMessage
    {
        public string text { get; set; }
        public List<Gifsercise> attachments { get; set; }

        public SlackMessage(string url, string text)
        {
            attachments = new List<Gifsercise>();
            attachments.Add(new Gifsercise(url, text));
        }
    }
}
