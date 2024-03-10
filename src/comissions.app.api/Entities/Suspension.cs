    namespace comissions.app.database.Entities;

    public class Suspension
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime SuspensionDate { get; set; }
        public DateTime UnsuspensionDate { get; set; }
        public bool Voided { get; set; } = false;
        public string Reason { get; set; }
        public virtual User User { get; set; }
    }