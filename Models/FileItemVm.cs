namespace ABC_Retail.Models
{
    public class FileItemVm
    {
        public string Name { get; set; }
        public string Url { get; set; }

        public FileItemVm(string name, string url)
        {
            Name = name;
            Url = url;
        }
    }
}
