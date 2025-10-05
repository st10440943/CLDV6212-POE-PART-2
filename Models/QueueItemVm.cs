namespace ABC_Retail.Models
{
    public class QueueItemVm
    {
        public QueueItemVm(string id, string text, DateTimeOffset? inserted)
        {
            Id = id;
            Text = text;
            Inserted = inserted?.ToString("g");
        }

        public string Id { get; set; }
        public string Text { get; set; }
        public string Inserted { get; set; }
    }
}
