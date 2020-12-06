namespace Funda.Models
{
    public class EstateAgent
    {
        public int Id { get; }
        public string Name { get; }
        public int Count { get; }

        public EstateAgent(int id, string name, int count)
        {
            Id = id;
            Name = name;
            Count = count;
        }
    }
}
