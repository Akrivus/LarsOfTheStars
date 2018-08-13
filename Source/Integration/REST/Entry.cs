namespace LarsOfTheStars.Source.Integration.REST
{
    public class Entry
    {
        public ulong ID { get; set; }
        public string Username { get; set; }
        public string Discriminator { get; set; }
        public int Score { get; set; }
    }
}
