namespace SimpleInventory.DL.Models
{
    public class Supplier
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Address Address { get; set; }
        public int AddressId { get; set; }
    }
}