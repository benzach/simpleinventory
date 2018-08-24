namespace SimpleInventory.DL.Models
{
    public class Address
    {
        public int Id { get; set; }
        public string Note { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Zip { get; set; }
        public int Country_Code_Value { get; set; }
        public Code_Value Country { get; set; }
        public string ContactPhone { get; set; }
        public int State_Code_Value { get; set; }
        public Code_Value State { get; set; }
    }
}